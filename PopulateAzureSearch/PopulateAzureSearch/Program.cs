using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateAzureSearch
{
    /// <summary>
    /// This program retrieves book data from the Gutenberg database and then passes the book data to the Azure 
    /// Search Service to index. 
    /// </summary>
    class Program
    {
        private const int batchSize = 100;
        private const int minBatchId = 41000;
        private const int maxBatchId = 80000;

        private static int bookId;

        /// <summary>
        /// Retrieves a batch of books from Gutenberg and writes them to the search index. 
        /// Specifically retrieves the books in the range of the given id to id + batchSize.
        /// </summary>
        static void ExecuteBatch(int id)
        {
            var bookObjs = new List<Book>();

            int limit = id + batchSize;

            for (bookId = id; bookId < limit; bookId++)
            {
                // TODO: We should deploy the gutenberg-http service and run it somewhere as right now
                // we need to run it locally for this code to work. 
                string requestUrl = "http://localhost:8000/texts/" + bookId;
                var client = new RestClient(requestUrl);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                Book book = ParseResponse(response.Content);
                bookObjs.Add(book);
                Console.WriteLine(book);
            }

            WriteToSiAsync(bookObjs).GetAwaiter().GetResult();           
        }

        static void Main(string[] args)
        {
            for (int batchId = minBatchId; batchId < maxBatchId; batchId += batchSize)
            {
                ExecuteBatch(batchId);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Writes a list of books to the search index. 
        /// </summary>
        private static async Task WriteToSiAsync(IEnumerable<Book> listBook)
        {
            // TODO: It would be better to move the apiKey 471B10EC0E9A898A71401CB6770A16AA to a config file.
            SearchServiceClient serviceClient = new SearchServiceClient("librarysearchservice", new SearchCredentials("471B10EC0E9A898A71401CB6770A16AA"));
            var indexClient = serviceClient.Indexes.GetClient("booksindex");
            var act = listBook.Select(x => IndexAction.Upload<Book>(x));
            var batch = IndexBatch.New(act);
            await indexClient.Documents.IndexAsync(batch);
        }

        /// <summary>
        /// Parses Gutenberg json into a Book object. 
        /// </summary>
        private static Book ParseResponse(string content)
        {
            Book book = new Book();
            JObject jObject = JObject.Parse(content);
            JToken jBook = jObject["metadata"];
            book.id = bookId.ToString();
            book.Title = string.Join(", ", jBook["title"]);
            book.Author = string.Join(", ", jBook["author"]);
            book.Language = string.Join(", ", jBook["language"]);
            string category = string.Join(", ", jBook["subject"]);
            book.Category = category.Split(",");
            string links = string.Join(", ", jBook["formaturi"]);
            book.Links = links.Split(",");

            return book;
        }
    }
}

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
    class Program
    {
        private static int bookId;

        static void ExecuteBatch(int id)
        {
            var bookObjs = new List<Book>();

            int limit = id + 100;

            for (bookId = id; bookId < limit; bookId++)
            {
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
            for (int batchId = 41000; batchId < 80000; batchId += 100)
            {
                ExecuteBatch(batchId);
            }

            Console.ReadKey();
        }

        private static async Task WriteToSiAsync(IEnumerable<Book> listBook)
        {
            SearchServiceClient serviceClient = new SearchServiceClient("librarysearchservice", new SearchCredentials("<api-key>"));
            var indexClient = serviceClient.Indexes.GetClient("booksindex");
            var act = listBook.Select(x => IndexAction.Upload<Book>(x));
            await indexClient.Documents.IndexAsync(IndexBatch.New(act));
        }
        private static Book ParseResponse(string content)
        {
            Book book = new Book();
            JObject jObject = JObject.Parse(content);
            JToken jBook = jObject["metadata"];
            book.id = bookId.ToString();
            book.BookName = string.Join(", ", jBook["title"]);
            book.AuthorName = string.Join(", ", jBook["author"]);
            book.Language = string.Join(", ", jBook["language"]);
            string category = string.Join(", ", jBook["subject"]);
            book.Category = category.Split(",");
            string links = string.Join(", ", jBook["formaturi"]);
            book.Links = links.Split(",");

            return book;
        }
    }
}

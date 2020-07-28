using Microsoft.Azure.Search;

namespace PopulateAzureSearch
{
    /// <summary>
    /// This class represents the Book Index which is used by the Azure Search Service. 
    /// We pull books from the Gutenberg database and extract the data into this model
    /// in order to pass it to the Azure Search Service. 
    /// </summary>
    public class Book
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string id { get; set; }

        [IsSearchable, IsSortable, IsFilterable]
        public string Title { get; set; }

        [IsSearchable, IsSortable, IsFilterable]
        public string Author { get; set; }

        [IsSearchable, IsFilterable]
        public string[] Category { get; set; }

        public string[] Links { get; set; }

        [IsFilterable, IsSearchable]
        public string Language { get; set; }

        public override string ToString()
        {
            return "id:" + id + "\nTitle:" + Title + "\nAuthor:" + Author + "\nSubject:" + string.Join(", ", Category) + "\nLanguage:" + Language + "\nLinks:" + string.Join(", ", Links);
        }
    }
}
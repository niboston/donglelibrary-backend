using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace PopulateAzureSearch
{

    public class Book
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string id { get; set; }

        [IsSearchable, IsSortable, IsFilterable]
        public string BookName { get; set; }

        [IsSearchable, IsSortable, IsFilterable]
        public string AuthorName { get; set; }

        [IsSearchable, IsFilterable]
        public string[] Category { get; set; }

        public string[] Links { get; set; }

        [IsFilterable, IsSearchable]
        public string Language { get; set; }

        public override string ToString()
        {
            return "id:"+ id + "\nBookName:"+ BookName + "\nAuthorName:" + AuthorName + "\nCategory:" + string.Join(", ", Category) + "\nLanguage:"+ Language + "\nLinks:" + string.Join(", ", Links);
        }
    }
}
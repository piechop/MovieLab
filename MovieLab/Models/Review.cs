using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieLab.Models
{
    public class Review
    {
        public Review()
        {
            MovieID = 0;
            ReviewTime = DateTime.Now;
            ReviewRating = 0;
        }

        public int ID { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Content")]
        public string ReviewText { get; set; }

        [Display(Name = "Movie Rating")]
        public byte MovieRating { get; set; }

        [Display(Name = "Review Rating")]
        public byte ReviewRating { get; set; }

        [Display(Name = "Date Created")]
        public DateTime ReviewTime { get; set; }

        public string Author { get; set; }

        [Display(Name = "Movie")]
        public int MovieID { get; set; }

        [Display(Name = "Review Title")]
        public string ReviewTitle { get; set; }
    }
}
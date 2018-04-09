using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieLab.Models
{
    public class Review
    {
        public Review()
        {
            MovieID = 0;
        }

        public int ID { get; set; }

        public string ReviewText { get; set; }

        public byte MovieRating { get; set; }

        public byte ReviewRating { get; set; }

        public DateTime ReviewTime { get; set; }

        public ApplicationUser Author { get; set; }

        public int MovieID { get; set; }

        public string ReviewTitle { get; set; }
    }
}
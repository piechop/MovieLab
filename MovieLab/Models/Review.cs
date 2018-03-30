using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieLab.Models
{
    public class Review
    {
        public int ID { get; set; }

        public string ReviewText { get; set; }

        public byte MovieRating { get; set; }

        public byte ReviewRating { get; set; }

        public DateTime ReviewTime { get; set; }

        public ApplicationUser Author { get; set; }

        public int MovieTitle
        {
            get => default(int);
            set
            {
            }
        }

        public int ReviewTitle
        {
            get => default(int);
            set
            {
            }
        }
    }
}
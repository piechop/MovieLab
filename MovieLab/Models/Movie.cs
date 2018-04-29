using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieLab.Models
{
    public class Movie
    {
        public Movie()
        {
            Reviews = new List<Review>();
        }

        public int ID { get; set; }

        public List<Review> Reviews { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Movie titles are limited to 50 characters.")]
        public string Title { get; set; }

        public Genre Genre { get; set; }

        [Display(Name = "Release Date (mm/dd/yyyy)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        //[RegularExpression(@"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d", ErrorMessage = "Date invalid. Please try again using the indicated format.")]
        public DateTime Release { get; set; }

        [Range(1,320, ErrorMessage = "Length in minutes must be between 1 and 320.")]
        [Display(Name = "Movie Length (min.)")]
        public int MinuteLength { get; set; }

        [MaxLength(100, ErrorMessage = "Director names are limited to 100 characters.")]
        public string Director { get; set; }

        [MaxLength(100, ErrorMessage = "Producer names are limited to 100 characters.")]
        public string Producer { get; set; }

        public string Photo { get; set; }

        public string Synopsis { get; set; }
    }
}
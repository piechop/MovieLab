using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieLab.Models
{
    public class Rating
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public byte Rate { get; set; }

        public int ReviewID { get; set; }
    }
}
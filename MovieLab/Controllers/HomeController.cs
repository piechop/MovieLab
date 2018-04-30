using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieLab.CustomAttributes;

namespace MovieLab.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "More about the Movie Manager";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us here with any of your questions or concerns.";

            return View();
        }

        [AuthorizeOrRedirectAttribute(Roles = "Site Admin, Movie Admin")]
        public ActionResult Admin()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieLab.Models;
using PagedList;
using MovieLab.CustomAttributes;
using System.IO;

namespace MovieLab.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int _pageSize;
        private int _pageNumber;
        private static readonly int MAX_FILE_SIZE = (int)Math.Pow(10, 7);

        [HttpGet]
        public ActionResult Index(string sortOrder, int? page)
        {
            IEnumerable<Movie> movies = SetupMovieList(page);

            return View(MoviesToPagedList(movies, sortOrder, page));
        }

        [HttpPost]
        public ActionResult Index(string searchCriteria, int? yearFilter, int? page)
        {
            IEnumerable<Movie> movies = SetupMovieList(page);

            return View(MoviesToPagedList(movies, searchCriteria, yearFilter, page));
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Create([Bind(Include = "ID,Title,Genre,Release,MinuteLength,Director,Producer, Photo")] Movie movie, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    movie.Photo = UploadFile(fileUpload);
                }

                db.movie.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Edit([Bind(Include = "ID,Title,Genre,Release,MinuteLength,Director,Producer, Photo")] Movie movie, HttpPostedFileBase fileUpload, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (collection["removeImage"] != null && Convert.ToBoolean(collection["removeImage"].Split(',')[0]))
                {
                    if ((System.IO.File.Exists(Server.MapPath("~") + movie.Photo)))
                    {
                        System.IO.File.Delete(Server.MapPath("~") + movie.Photo);
                    }
                    movie.Photo = "";
                }

                if (fileUpload != null)
                {
                    movie.Photo = UploadFile(fileUpload);
                }

                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.movie.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.movie.Find(id);

            if ((System.IO.File.Exists(Server.MapPath("~") + movie.Photo)))
            {
                System.IO.File.Delete(Server.MapPath("~") + movie.Photo);
            }

            db.movie.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        private IEnumerable<Movie> SetupMovieList(int? page)
        {
            _pageSize = 10;
            _pageNumber = (page ?? 1);

            IEnumerable<Movie> movies = db.movie.ToList();

            ViewBag.Years = ListOfYears(movies);

            return movies;
        }

        [NonAction]
        private IPagedList<Movie> MoviesToPagedList(IEnumerable<Movie> movies, string searchCriteria, int? yearFilter, int? page)
        {

            movies = FilterMoviesByCriteria(searchCriteria, movies);
            movies = FilterMoviesByYear(yearFilter, movies);

            return movies.ToPagedList(_pageNumber, _pageSize);
        }

        [NonAction]
        private IPagedList<Movie> MoviesToPagedList(IEnumerable<Movie> movies, string sortOrder, int? page)
        {
            return SortMovies(sortOrder, movies).ToPagedList(_pageNumber, _pageSize);
        }

        [NonAction]
        private IPagedList<Movie> MoviesToPagedList(IEnumerable<Movie> movies, string searchCriteria, int? yearFilter, string sortOrder, int? page)
        {

            movies = FilterMoviesByCriteria(searchCriteria, movies);
            movies = FilterMoviesByYear(yearFilter, movies);
            movies = SortMovies(sortOrder, movies);

            return movies.ToPagedList(_pageNumber, _pageSize);
        }

        [NonAction]
        private IEnumerable<Movie> FilterMoviesByCriteria(string searchCriteria, IEnumerable<Movie> movies)
        {
            if (searchCriteria != null)
            {
                ViewBag.CurrentCriteria = searchCriteria;
                movies = movies.Where(m => m.Title.ToUpper().Contains(searchCriteria.ToUpper()));
            }

            return movies;
        }

        [NonAction]
        private IEnumerable<Movie> FilterMoviesByYear(int? yearFilter, IEnumerable<Movie> movies)
        {
            if (yearFilter != null && yearFilter >= 0)
            {
                ViewBag.CurrentFilter = yearFilter;
                movies = movies.Where(m => m.Release.Year == yearFilter);
            }
            return movies;
        }

        [NonAction]
        private IEnumerable<Movie> SortMovies(string sortOrder, IEnumerable<Movie> movies)
        {
            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "Title":
                    movies = movies.OrderBy(m => m.Title);
                    break;
                case "Year":
                    movies = movies.OrderBy(m => m.Release.Year).Reverse();
                    break;
                case "Director":
                    movies = movies.OrderBy(m => m.Director);
                    break;
                default:
                    movies = movies.OrderBy(m => m.Title);
                    break;
            }

            return movies;
        }

        [NonAction]
        private IEnumerable<int> ListOfYears(IEnumerable<Movie> movies)
        {
            var years = movies.Select(m => m.Release.Year).Distinct().OrderBy(x => x);

            return years;
        }

        [NonAction]
        public string UploadFile(HttpPostedFileBase postedFile)
        {
            string pathName = "";

            if (postedFile != null && postedFile.ContentLength > 0 && postedFile.ContentLength <= MAX_FILE_SIZE)
            {
                bool canUpload = false;
                string[] validExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                canUpload = validExtensions.Any(item => postedFile.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));

                if (canUpload)
                {
                    pathName = DateTime.Now.ToBinary().ToString() + Path.GetFileName(postedFile.FileName);
                    postedFile.SaveAs(Server.MapPath("~/Content/") + pathName);

                    pathName = "/Content/" + pathName;
                }
                else
                {
                    throw new ArgumentException("File type is not supported");
                }
            }
            else if (postedFile != null)
            {
                throw new ArgumentException("File size is not valid");
            }
            else
            {
                throw new ArgumentException("File does not exist");
            }

            return pathName;
        }
    }
}

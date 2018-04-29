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
            //List<Movie> mov = db.movie.ToList();
            //foreach (Movie m in mov)
            //{
            //    m.Synopsis = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            //    db.Entry(m).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            IEnumerable<Movie> movies = SetupMovieList(page);

            return View(MoviesToPagedList(movies, sortOrder, page));
        }

        [HttpPost]
        public ActionResult Index(string searchCriteria, int? yearFilter, string genreFilter, int? page)
        {
            IEnumerable<Movie> movies = SetupMovieList(page);

            return View(MoviesToPagedList(movies, searchCriteria, yearFilter, "", genreFilter, page));
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
        public ActionResult Create([Bind(Include = "ID,Title,Genre,Release,MinuteLength,Director,Producer, Photo, Synopsis")] Movie movie, HttpPostedFileBase fileUpload)
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
        public ActionResult Edit([Bind(Include = "ID,Title,Genre,Release,MinuteLength,Director,Producer, Photo, Synopsis")] Movie movie, HttpPostedFileBase fileUpload, FormCollection collection)
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
                    if ((System.IO.File.Exists(Server.MapPath("~") + movie.Photo)))
                    {
                        System.IO.File.Delete(Server.MapPath("~") + movie.Photo);
                    }
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

            var genreList = new List<Genre>() { Genre.Action, Genre.Adventure, Genre.Animation,
                Genre.Comedy, Genre.Documentary, Genre.Drama, Genre.Fantasy, Genre.Horror,
                Genre.Mystery, Genre.Romance, Genre.Science_Fiction, Genre.Thriller, Genre.Western};
            ViewBag.Genres = genreList;

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
        private IPagedList<Movie> MoviesToPagedList(IEnumerable<Movie> movies, string searchCriteria, int? yearFilter, string sortOrder, string genreFilter, int? page)
        {

            movies = FilterMoviesByCriteria(searchCriteria, movies);
            movies = FilterMoviesByYear(yearFilter, movies);
            movies = FilterMoviesByGenre(genreFilter, movies);
            movies = SortMovies(sortOrder, movies);

            return movies.ToPagedList(_pageNumber, _pageSize);
        }

        [NonAction]
        private IEnumerable<Movie> FilterMoviesByCriteria(string searchCriteria, IEnumerable<Movie> movies)
        {
            if (searchCriteria != null && searchCriteria != "")
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
        private IEnumerable<Movie> FilterMoviesByGenre(string genreFilter, IEnumerable<Movie> movies)
        {
            if(genreFilter != null && genreFilter != "")
            {
                ViewBag.CurrentGenre = genreFilter;

                movies = movies.Where(m => m.Genre.ToString() == genreFilter);
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

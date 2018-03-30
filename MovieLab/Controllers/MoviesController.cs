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

namespace MovieLab.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int _pageSize;
        private int _pageNumber;

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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Genre,Release,MinuteLength,Director,Producer")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.movie.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
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
        public ActionResult Edit([Bind(Include = "ID,Title,Genre,Release,MinuteLength,Director,Producer")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
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
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.movie.Find(id);
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
    }
}

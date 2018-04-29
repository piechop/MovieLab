using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MovieLab.CustomAttributes;
using MovieLab.Models;
using PagedList;

namespace MovieLab.Controllers
{
    public class ReviewsController : Controller
    {
        private int _pageSize;
        private int _pageNumber;
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Index(int? page)
        {
            _pageSize = 10;
            _pageNumber = (page ?? 1);

            var movieList = db.movie.Select(m => m.Title);
            ViewBag.SelectMovieList = movieList;

            return View(BuildReviewViewModelList(db.review.ToList()).ToPagedList(_pageNumber, _pageSize));
        }

        [HttpPost]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Index(int? page, string search, string filter)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentFilter = filter;

            var movieList = db.movie.Select(m => m.Title);
            ViewBag.SelectMovieList = movieList;

            List<ReviewViewModel> rvm = BuildReviewViewModelList(db.review.ToList());

            _pageSize = 10;
            _pageNumber = (page ?? 1);

            if (search != null && search != "")
            {
                rvm = rvm.Where(a => a.Author.ToUpper().Contains(search.ToUpper())).ToList();
            }

            if (filter != null)
            {
                rvm = rvm.Where(r => r.MovieTitle.ToUpper().Contains(filter.ToUpper())).ToList();
            }

            return View(rvm.ToPagedList(_pageNumber, _pageSize));
        }

        [HttpGet]
        public ActionResult ListOfReviewsByAuthor(int? page)
        {
            _pageSize = 10;
            _pageNumber = (page ?? 1);

            var movieList = db.movie.Select(m => m.Title);
            ViewBag.SelectMovieList = movieList;

            string user = User.Identity.GetUserName();
            var reviews = db.review.Where(r => r.Author == user).ToList();
            ViewBag.User = user;

            return View(BuildReviewViewModelList(reviews).ToPagedList(_pageNumber, _pageSize));
        }

        [HttpPost]
        public ActionResult ListOfReviewsByAuthor(int? page, string filter, string search)
        {
            _pageSize = 10;
            _pageNumber = (page ?? 1);

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentFilter = filter;

            var movieList = db.movie.Select(m => m.Title);
            ViewBag.SelectMovieList = movieList;

            string user = User.Identity.GetUserName();
            var reviews = db.review.Where(r => r.Author == user).ToList();
            ViewBag.User = user;

            List<ReviewViewModel> rvm = BuildReviewViewModelList(reviews);

            if (search != null && search != "")
            {
                rvm = rvm.Where(a => a.ReviewTitle.ToUpper().Contains(search.ToUpper())).ToList();
            }

            if (filter != null && filter != "")
            {
                rvm = rvm.Where(r => r.MovieTitle.ToUpper().Contains(filter.ToUpper())).ToList();
            }

            return View(rvm.ToPagedList(_pageNumber, _pageSize));
        }

        [HttpGet]
        public ActionResult ListOfReviewsByMovie(int movieID, int? page)
        {
            _pageSize = 10;
            _pageNumber = (page ?? 1);

            var reviews = db.review.Where(r => r.MovieID == movieID).ToList();

            var movie = db.movie.FirstOrDefault(m => m.ID == movieID);
            ViewBag.Movie = movie;

            if (movie != null)
            {
                return View(reviews.ToPagedList(_pageNumber, _pageSize));
            }
            else
            {
                ViewBag.ErrorMessage = "Movie not found.";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ListOfReviewsByMovie(int movieID, int? page, string search, int? filter, int? filter2)
        {
            _pageSize = 10;
            _pageNumber = (page ?? 1);

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentFilter2 = filter2;

            var reviews = db.review.Where(r => r.MovieID == movieID).ToList();

            var movie = db.movie.FirstOrDefault(m => m.ID == movieID);
            ViewBag.Movie = movie;

            if (search != null && search != "")
            {
                reviews = reviews.Where(a => a.Author.ToUpper().Contains(search.ToUpper())).ToList();
            }

            if (filter != null && filter >= 0 && filter <= 100)
            {
                reviews = reviews.Where(r => r.MovieRating >= filter).ToList();
            }

            if(filter2 != null && filter2 >= 0 && filter2 <= 100)
            {
                reviews = reviews.Where(r => r.MovieRating <= filter2).ToList();
            }

            if (movie != null)
            {
                return View(reviews.ToPagedList(_pageNumber, _pageSize));
            }
            else
            {
                ViewBag.ErrorMessage = "Movie not found.";
                return View("Error");
            }
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id, string redirect = "Index")
        {
            ViewBag.Action = redirect;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.review.Find(id);
            ReviewViewModel reviewViewModel = BuildReviewViewModel(review);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(reviewViewModel);
        }

        // GET: Reviews/Create
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Create()
        {
            var movieList = db.movie.Select(m => m);
            ViewBag.SelectMovieList = new SelectList(movieList, "Id", "Title");

            return View();
        }

        // POST: Reviews/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin")]
        public ActionResult Create([Bind(Include = "Author, Ratings,ID,ReviewText,MovieRating,ReviewRating,ReviewTime,MovieID,ReviewTitle")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.review.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(review);
        }

        // GET: Reviews/Edit/5
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var movieList = db.movie.Select(m => m);

            Review review = db.review.Find(id);

            if (User.Identity.GetUserName() == review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                SelectList list = new SelectList(movieList, "Id", "Title");

                ViewBag.SelectMovieList = list;

                ReviewViewModel reviewViewModel = BuildReviewViewModel(review);

                if (review == null)
                {
                    return HttpNotFound();
                }
                return View(reviewViewModel);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult UserEdit(int? id, string redirect = "ListOfReviewsByMovie")
        {
            ViewBag.Redirect = redirect;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var movieList = db.movie.Select(m => m);

            Review review = db.review.Find(id);

            if (User.Identity.GetUserName() == review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                SelectList list = new SelectList(movieList, "Id", "Title");

                ViewBag.SelectMovieList = list;

                ReviewViewModel reviewViewModel = BuildReviewViewModel(review);

                if (review == null)
                {
                    return HttpNotFound();
                }
                return View(reviewViewModel);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult UserEdit([Bind(Include = "Ratings,Author,ID,ReviewText,MovieRating,ReviewRating,ReviewTime,MovieID,ReviewTitle")] Review review, string redirect = "ListOfReviewsByMovie")
        {
            if (User.Identity.GetUserName() == review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(review).State = EntityState.Modified;
                    db.SaveChanges();
                    if(redirect == "ListOfReviewsByMovie")
                    {
                        return RedirectToAction(redirect, new { movieID = review.MovieID });
                    }
                    else
                    {
                        return RedirectToAction(redirect);
                    }
                }
                return View(review);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        // POST: Reviews/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult Edit([Bind(Include = "Author,ID,ReviewText,MovieRating,ReviewRating,ReviewTime,MovieID,ReviewTitle")] Review review)
        {
            if (User.Identity.GetUserName() == review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(review).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(review);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        public ActionResult Rate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var movieList = db.movie.Select(m => m);

            Review review = db.review.Find(id);

            if (User.Identity.GetUserName() != review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                ReviewViewModel reviewViewModel = BuildReviewViewModel(review);

                if (review == null)
                {
                    return HttpNotFound();
                }
                return View(reviewViewModel);
            }
            else if(User.Identity.GetUserName() == review.Author)
            {
                ViewBag.Error = "You cannot rate your own review.";
                return RedirectToAction("ListOfReviewsByAuthor");
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult Rate([Bind(Include = "Ratings,Author,ID,ReviewText,MovieRating,ReviewRating,ReviewTime,MovieID,ReviewTitle")] Review review, FormCollection collection)
        {
            string userID = User.Identity.GetUserId();
            var rated = db.rating.Where(r => r.ReviewID == review.ID).Where(u => u.UserID == userID);
            if (rated.Count() == 0 && (User.Identity.GetUserName() != review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin")))
            {
                int rating = int.Parse(collection["ReviewRating"].Split(',')[1]);
                if (ModelState.IsValid && (rating <= 100 && rating > 0))
                {
                    review.Ratings = (review.Ratings == null? 1 : review.Ratings + 1);
                    review.ReviewRating = (byte)((rating + review.ReviewRating) / (review.Ratings > 1 ? 2 : 1));
                    
                    db.rating.Add(new Rating { UserID = User.Identity.GetUserId(), Rate = (byte)rating, ReviewID = review.ID });
                    
                    db.Entry(review).State = EntityState.Modified;
                    db.SaveChanges();

                    var userReviews = db.review.Where(u => u.Author == review.Author);
                    int userRating = review.ReviewRating;
                    foreach (Review rev in userReviews)
                    {
                        userRating += rev.ReviewRating;
                    }

                    userRating = userRating / (userReviews.Count() + 1);

                    var user = db.Users.Where(us => us.UserName == review.Author).FirstOrDefault();
                    if (user != null)
                    {
                        user.UserRating = userRating;
                    }

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("ListOfReviewsByMovie", new { movieID = review.MovieID});
                }
                else if(rating > 100 || rating < 0)
                {
                    ViewBag.Error = "Your rating must be between 0 and 100";
                }
                return View(BuildReviewViewModel(review));
            }
            else if(rated.Count() > 0)
            {
                ViewBag.Error = "You've already rated this review.";
                return View(BuildReviewViewModel(review));
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        // GET: Reviews/Delete/5
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult Delete(int? id, string redirect = "Index")
        {
            ViewBag.Action = redirect;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Review review = db.review.Find(id);

            if (User.Identity.GetUserName() == review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                ReviewViewModel reviewViewModel = BuildReviewViewModel(review);

                if (review == null)
                {
                    return HttpNotFound();
                }
                return View(reviewViewModel);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult DeleteConfirmed(int id, string redirect = "Index")
        {
            Review review = db.review.Find(id);
            if (User.Identity.GetUserName() == review.Author || User.IsInRole("Site Admin") || User.IsInRole("Movie Admin"))
            {
                var user = db.Users.Where(u => u.UserName == review.Author).FirstOrDefault();
                if(user != null)
                {
                    user.ReviewCount = user.ReviewCount - 1;
                    db.Entry(user).State = EntityState.Modified;
                }

                db.review.Remove(review);
                db.SaveChanges();
                if(redirect == "ListOfReviewsByMovie")
                {
                    return RedirectToAction(redirect, new { movieID = review.MovieID});
                }
                else
                {
                    return RedirectToAction(redirect);
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }

        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOrRedirectAttribute(Roles = "Movie Admin, Site Admin, Reviewer")]
        public ActionResult UserCreate([Bind(Include = "Ratings,ID, Author, ReviewTime, ReviewTitle, MovieRating, ReviewRating, ReviewText, MovieID")] Review review, int movieID)
        {
            string userName = User.Identity.GetUserName();
            var previousReviews = db.review.Where(r => r.MovieID == movieID).Where(u => u.Author == userName);
            if(previousReviews.Count() == 0 && ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                user.ReviewCount = user.ReviewCount + 1;
                db.SaveChanges();

                review.MovieID = movieID;
                db.review.Add(review);
                db.SaveChanges();
                return RedirectToAction("ListOfReviewsByMovie", new { movieID = review.MovieID });
            }
            else if(previousReviews.Count() > 0)
            {
                ViewBag.Error = "You've already written a review for this movie.";
            }

            return View(review);
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
        private ReviewViewModel BuildReviewViewModel(Review review)
        {
            var movieTitles = db.movie.ToDictionary(m => m.ID, m => m.Title);

            return new ReviewViewModel()
            {
                ID = review.ID,
                Ratings = review.Ratings,
                ReviewRating = review.ReviewRating,
                MovieRating = review.MovieRating,
                ReviewTitle = review.ReviewTitle,
                ReviewTime = review.ReviewTime,
                ReviewText = review.ReviewText,
                Author = review.Author,
                MovieID = review.MovieID,
                MovieTitle = movieTitles[review.MovieID]
            };
        }

        [NonAction]
        private List<ReviewViewModel> BuildReviewViewModelList(List<Review> reviews)
        {
            List<ReviewViewModel> reviewViewModel = new List<ReviewViewModel>();

            var movieTitles = db.movie.ToDictionary(m => m.ID, m => m.Title);
            foreach(var review in reviews)
            {
                reviewViewModel.Add(new ReviewViewModel
                {
                    ID = review.ID,
                    ReviewRating = review.ReviewRating,
                    MovieRating = review.MovieRating,
                    ReviewTitle = review.ReviewTitle,
                    ReviewTime = review.ReviewTime,
                    ReviewText = review.ReviewText,
                    Author = review.Author,
                    MovieID = review.MovieID,
                    MovieTitle = movieTitles[review.MovieID]
                });
            }

            return reviewViewModel;
        }
    }
}

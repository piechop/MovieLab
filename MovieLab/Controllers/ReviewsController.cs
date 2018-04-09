﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieLab.Models;

namespace MovieLab.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public ActionResult Index()
        {
            return View(BuildReviewViewModelList(db.review.ToList()));
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
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
        public ActionResult Create([Bind(Include = "ID,ReviewText,MovieRating,ReviewRating,ReviewTime,MovieID,ReviewTitle")] Review review)
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
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var movieList = db.movie.Select(m => m);

            Review review = db.review.Find(id);

            SelectList list = new SelectList(movieList, "Id", "Title");
            
            ViewBag.SelectMovieList = list;

            ReviewViewModel reviewViewModel = BuildReviewViewModel(review);

            if (review == null)
            {
                return HttpNotFound();
            }
            return View(reviewViewModel);
        }

        // POST: Reviews/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ReviewText,MovieRating,ReviewRating,ReviewTime,MovieID,ReviewTitle")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.review.Find(id);
            db.review.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserCreate([Bind(Include = "ID, ReviewTime, ReviewTitle, MovieRating, ReviewRating, ReviewText, MovieID")] Review review, int movieID)
        {
            if(ModelState.IsValid)
            {
                review.MovieID = movieID;
                db.review.Add(review);
                db.SaveChanges();
                return RedirectToAction("ListOfReviewsByMovie", new { movieID = review.MovieID });
            }

            return View(review);
        }

        public ActionResult ListOfReviewsByMovie(int movieID)
        {
            var reviews = db.review.Where(r => r.MovieID == movieID).ToList();

            var movie = db.movie.FirstOrDefault(m => m.ID == movieID);
            ViewBag.Movie = movie;

            if(movie != null)
            {
                return View(reviews);
            }
            else
            {
                ViewBag.ErrorMessage = "Movie not found.";
                return View("Error");
            }
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
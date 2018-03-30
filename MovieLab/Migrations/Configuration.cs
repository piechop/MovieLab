namespace MovieLab.Migrations
{
    using MovieLab.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MovieLab.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MovieLab.Models.ApplicationDbContext context)
        {
            context.movie.AddOrUpdate(m => m.Title,
                new Models.Movie()
                {
                    ID = 1,
                    Title = "Movie 1",
                    Genre = Models.Genre.Action,
                    Release = new DateTime(2000,12,31),
                    Director = "Director 1",
                    Producer = "Producer 1",
                    MinuteLength = 2
                },
                new Models.Movie()
                {
                    ID = 2,
                    Title = "Movie 2",
                    Genre = Models.Genre.Action,
                    Release = new DateTime(1999,1,1),
                    Director = "Director 2",
                    Producer = "Producer 2",
                    MinuteLength = 15
                }
            );
        }
    }
}

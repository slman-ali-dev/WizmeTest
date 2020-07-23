using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WizmeTest.Models;

namespace WizmeTest.DbContexts
{
    public class DatabaseContext : DbContext
    {

        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MovieActors> MovieActors { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=MyCustomTestDB");
            }
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieActors>()
                .HasKey(ma => new { ma.ActorId, ma.MovieId });

            modelBuilder.Entity<MovieActors>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.Movies)
                .HasForeignKey(ma => ma.ActorId);

            modelBuilder.Entity<MovieActors>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.Actors)
                .HasForeignKey(ma => ma.MovieId);

            // seeding database with dummy data
            DummySeed(ref modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void DummySeed(ref ModelBuilder modelBuilder)
        {
            List<Actor> ActorsSeed = new List<Actor>();
            List<Movie> MoviesSeed = new List<Movie>();
            List<MovieActors> MovieActorsSeed = new List<MovieActors>();
            for (int i = 1; i <= 200; ++i)
            {
                ActorsSeed.Add(new Actor()
                {
                    Id = i,
                    FirstName = $"FNameActor-{i}",
                    LastName = $"LNameActor-{i}"
                });
                MoviesSeed.Add(new Movie()
                {
                    Id = i,
                    Title = $"<Movie - {i} title",
                    Description = $"Description of movie - {i}"
                });
            }

            for (int i = 1; i <= ActorsSeed.Count; ++i)
            {
                Random r = new Random();
                int actor = i;
                int moviesNO = r.Next(1, 10);
                for (int m = 0; m < moviesNO; ++m)
                {
                    int mov = r.Next(1, MoviesSeed.Count);
                    while (MovieActorsSeed.Exists(mm => { return mm.ActorId == actor && mm.MovieId == mov; }))
                    {
                        mov = r.Next(1, MoviesSeed.Count);
                    }
                    MovieActorsSeed.Add(new MovieActors()
                    {
                        ActorId = actor,
                        MovieId = mov
                    });
                }
            }
            modelBuilder.Entity<Actor>().HasData(ActorsSeed);
            modelBuilder.Entity<Movie>().HasData(MoviesSeed);
            modelBuilder.Entity<MovieActors>().HasData(MovieActorsSeed);
        }
    }
}

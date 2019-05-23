using Data.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameReview> GameReviews { get; set; }
        public DbSet<GuideReview> GuideReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(x => x.Guides)
                .WithOne(x => x.Author)
                .HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<Game>()
                .HasMany(x => x.Guides)
                .WithOne(x => x.Game)
                .HasForeignKey(x => x.GameId);

            modelBuilder.Entity<GameReview>()
                .HasOne(x => x.Game)
                .WithMany(x => x.GameReviews)
                .HasForeignKey(x => x.GameId);

            modelBuilder.Entity<GameReview>()
                .HasOne(x => x.User)
                .WithMany(x => x.GameReviews)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<GuideReview>()
                .HasOne(x => x.Guide)
                .WithMany(x => x.GuideReviews)
                .HasForeignKey(x => x.GuideId);

            modelBuilder.Entity<GuideReview>()
                .HasOne(x => x.User)
                .WithMany(x => x.GuideReviews)
                .HasForeignKey(x => x.UserId);
        }
    }
}

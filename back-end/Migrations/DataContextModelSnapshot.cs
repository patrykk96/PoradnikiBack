﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository;

namespace back_end.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("Data.DbModels.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.Property<double>("Rating");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Data.DbModels.GameReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GameId");

                    b.Property<int>("Rating");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("GameReviews");
                });

            modelBuilder.Entity("Data.DbModels.Guide", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuthorId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("Created");

                    b.Property<int>("GameId");

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.Property<double>("Rating");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GameId");

                    b.ToTable("Guides");
                });

            modelBuilder.Entity("Data.DbModels.GuideReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GuideId");

                    b.Property<int>("Rating");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GuideId");

                    b.HasIndex("UserId");

                    b.ToTable("GuideReviews");
                });

            modelBuilder.Entity("Data.DbModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ConfirmedAccount");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<string>("LoginProvider");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("ProfileImage");

                    b.Property<string>("ProviderKey");

                    b.Property<int>("Role");

                    b.Property<string>("Username");

                    b.Property<Guid>("VerificationCode");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Data.DbModels.GameReview", b =>
                {
                    b.HasOne("Data.DbModels.Game", "Game")
                        .WithMany("GameReviews")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.DbModels.User", "User")
                        .WithMany("GameReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data.DbModels.Guide", b =>
                {
                    b.HasOne("Data.DbModels.User", "Author")
                        .WithMany("Guides")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.DbModels.Game", "Game")
                        .WithMany("Guides")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data.DbModels.GuideReview", b =>
                {
                    b.HasOne("Data.DbModels.Guide", "Guide")
                        .WithMany("GuideReviews")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.DbModels.User", "User")
                        .WithMany("GuideReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

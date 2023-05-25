﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StarWars.Data;

#nullable disable

namespace StarWars.Migrations
{
    [DbContext(typeof(StarWarsContext))]
    partial class StarWarsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("CharacterMovie", b =>
                {
                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoviesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CharacterId", "MoviesId");

                    b.HasIndex("MoviesId");

                    b.ToTable("CharacterMovie");
                });

            modelBuilder.Entity("StarWars.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BirthDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EyeColorID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HairColorID")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Height")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("History")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PlanetID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RaceID")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EyeColorID");

                    b.HasIndex("HairColorID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PlanetID");

                    b.HasIndex("RaceID");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("StarWars.Models.EyeColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("EyeColor");
                });

            modelBuilder.Entity("StarWars.Models.HairColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("HairColor");
                });

            modelBuilder.Entity("StarWars.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("Movie");
                });

            modelBuilder.Entity("StarWars.Models.Planet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Planet");
                });

            modelBuilder.Entity("StarWars.Models.Race", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Race");
                });

            modelBuilder.Entity("CharacterMovie", b =>
                {
                    b.HasOne("StarWars.Models.Character", null)
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarWars.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StarWars.Models.Character", b =>
                {
                    b.HasOne("StarWars.Models.EyeColor", "EyeColor")
                        .WithMany("Character")
                        .HasForeignKey("EyeColorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarWars.Models.HairColor", "HairColor")
                        .WithMany("Character")
                        .HasForeignKey("HairColorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarWars.Models.Planet", "Planet")
                        .WithMany("Character")
                        .HasForeignKey("PlanetID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarWars.Models.Race", "Race")
                        .WithMany("Character")
                        .HasForeignKey("RaceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EyeColor");

                    b.Navigation("HairColor");

                    b.Navigation("Planet");

                    b.Navigation("Race");
                });

            modelBuilder.Entity("StarWars.Models.EyeColor", b =>
                {
                    b.Navigation("Character");
                });

            modelBuilder.Entity("StarWars.Models.HairColor", b =>
                {
                    b.Navigation("Character");
                });

            modelBuilder.Entity("StarWars.Models.Planet", b =>
                {
                    b.Navigation("Character");
                });

            modelBuilder.Entity("StarWars.Models.Race", b =>
                {
                    b.Navigation("Character");
                });
#pragma warning restore 612, 618
        }
    }
}

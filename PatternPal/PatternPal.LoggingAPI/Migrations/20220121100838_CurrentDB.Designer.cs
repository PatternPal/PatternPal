﻿// <auto-generated />
using System;
using PatternPal.LoggingAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PatternPal.LoggingAPI.Migrations
{
    [DbContext(typeof(LoggingContext))]
    [Migration("20220121100838_CurrentDB")]
    partial class CurrentDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.13");

            modelBuilder.Entity("PatternPal.LoggingAPI.Action", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActionTypeID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CommitID")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExerciseID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModeID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SessionID")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("PatternPal.LoggingAPI.ActionType", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("ActionTypes");

                    b.HasData(
                        new
                        {
                            ID = "Build"
                        },
                        new
                        {
                            ID = "RebuildAll"
                        },
                        new
                        {
                            ID = "Clean"
                        },
                        new
                        {
                            ID = "Deploy"
                        });
                });

            modelBuilder.Entity("PatternPal.LoggingAPI.ExtensionError", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActionID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("ExtensionErrors");
                });

            modelBuilder.Entity("PatternPal.LoggingAPI.Mode", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Modes");

                    b.HasData(
                        new
                        {
                            ID = "StepByStep"
                        },
                        new
                        {
                            ID = "Default"
                        });
                });

            modelBuilder.Entity("PatternPal.LoggingAPI.Session", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndSession")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExtensionVersion")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartSession")
                        .HasColumnType("TEXT");

                    b.Property<int>("TimeZone")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Sessions");

                    b.HasData(
                        new
                        {
                            ID = new Guid("79f83fbd-a9ed-434e-b585-e9258f804012"),
                            EndSession = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ExtensionVersion = 1,
                            StartSession = new DateTime(2021, 12, 9, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TimeZone = 1
                        });
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PatternPal.LoggingServer.Data;

#nullable disable

namespace PatternPal.LoggingServer.Migrations
{
    [DbContext(typeof(ProgSnap2ContextClass))]
    [Migration("20230327161450_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PatternPal.LoggingServer.Models.ProgSnap2Event", b =>
                {
                    b.Property<string>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("CodeStateID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("EventType")
                        .HasColumnType("integer");

                    b.Property<string>("SubjectID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToolInstances")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EventID");

                    b.ToTable("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
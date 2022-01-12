using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDesign.LoggingAPI
{
    public class LoggingContext : DbContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<Mode> Modes { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<ExtensionError> ExtensionErrors { get; set; }
        public LoggingContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=LoggingDB.db;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>().ToTable("Sessions");
            modelBuilder.Entity<ActionType>().ToTable("ActionTypes");
            modelBuilder.Entity<Mode>().ToTable("Modes");
            modelBuilder.Entity<Action>().ToTable("Actions");
            modelBuilder.Entity<ExtensionError>().ToTable("ExtensionErrors");

            modelBuilder.Entity<ActionType>().HasData(
                new ActionType { ID = "Build" },
                new ActionType { ID = "RebuildAll"},
                new ActionType { ID = "Clean" },
                new ActionType { ID = "Deploy" });

            modelBuilder.Entity<Mode>().HasData(
                new Mode { ID = "Step by Step" });
        }
    }
}

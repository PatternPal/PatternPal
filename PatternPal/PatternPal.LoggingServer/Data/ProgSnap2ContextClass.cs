using Microsoft.EntityFrameworkCore;
using System;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Data
{
    public class ProgSnap2ContextClass : DbContext
        {
            protected readonly IConfiguration Configuration = null!;

            public ProgSnap2ContextClass(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            /// <inheritdoc />
            public ProgSnap2ContextClass(DbContextOptions<ProgSnap2ContextClass> options) : base(options)
            {
            }
            /// <summary>
            /// When initializing the context, use the connection string from appsettings.json. When in development with docker, this will be the connection string to the postgres container and host should be "postgres". Otherwise it will be the connection string to the postgres database.
            /// </summary>
            /// <param name="optionsBuilder"></param>
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
            }

            public DbSet<ProgSnap2Event> Events { get; set; } = null!;
        }
}

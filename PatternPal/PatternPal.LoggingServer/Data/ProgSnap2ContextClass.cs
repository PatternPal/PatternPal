using Microsoft.EntityFrameworkCore;
using System;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Data
{
    public class ProgSnap2ContextClass : DbContext
        {
            protected readonly IConfiguration Configuration;

            public ProgSnap2ContextClass(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgresConnection"));
            }

            public DbSet<ProgSnap2Event> Events { get; set; }
        }
}

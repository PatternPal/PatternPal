using Microsoft.EntityFrameworkCore;
using System;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Data
{
    public class ProgSnap2ContextClass : DbContext
        {
            public ProgSnap2ContextClass(DbContextOptions<ProgSnap2ContextClass> options) : base(options)
            {
            }
            public DbSet<ProgSnap2Event> Events { get; set; } = null!;
        }
}

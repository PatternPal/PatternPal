#region

using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Models;

#endregion

namespace PatternPal.ProgSnapExport.Data;

/// <summary>
/// The context class necessary for connecting to the
/// PostgreSQL-database. It uses the ProgSnap2-model from
/// PatternPal.LoggingServer.
/// </summary>
public class ProgSnap2ContextClass : DbContext
{
    /// <summary>
    /// Contains the credentials for connecting to the database.
    /// </summary>
    private readonly string _connectionString;
    
    /// <summary>
    /// Represents the contents of the database.
    /// </summary>
    public DbSet<ProgSnap2Event> Events { get; set; }
    
    public ProgSnap2ContextClass(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString: _connectionString); 
        base.OnConfiguring(optionsBuilder);
    }
}

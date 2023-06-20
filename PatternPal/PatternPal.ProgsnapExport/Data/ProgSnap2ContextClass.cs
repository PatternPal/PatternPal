#region

using Microsoft.EntityFrameworkCore;

using PatternPal.LoggingServer.Models;

#endregion

namespace PatternPal.ProgSnapExport.Data;

public class ProgSnap2ContextClass : DbContext
{
    private readonly string _connectionString;
    
    public ProgSnap2ContextClass(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public DbSet<ProgSnap2Event> Events { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString: _connectionString); 
        base.OnConfiguring(optionsBuilder);
    }
}

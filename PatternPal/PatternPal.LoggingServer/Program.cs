
using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Data;
using PatternPal.LoggingServer.Data.Interfaces;
using PatternPal.LoggingServer.Models;
using PatternPal.LoggingServer.Services;

namespace PatternPal;

internal static class Program
{
    internal static void Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        builder.Services.AddGrpc();




        builder.Services.AddDbContext<ProgSnap2ContextClass>(options => 
            options.UseNpgsql( builder.Configuration.GetConnectionString("PostgresConnection") )
        );

        // Add the repository
        builder.Services.AddScoped<EventRepository, EventRepository>();

        WebApplication app = builder.Build();

        app.MapGrpcService<LoggerService>();

        app.Run();
    }
}


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

        // Build the webapp and add the services needed, including the database context and the repository as a scoped service.
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();
        builder.Services.AddDbContext<ProgSnap2ContextClass>(options => 
            options.UseNpgsql( builder.Configuration.GetConnectionString("PostgresConnection") )
        );
        builder.Services.AddScoped<EventRepository, EventRepository>();

        WebApplication app = builder.Build();

        // migrate the database to the latest version
        DatabaseManagementService.MigrationInitialization(app);

        // Adding of routing or GRPC services
        app.MapGrpcService<LoggerService>();

        // Run the webapp
        app.Run();
    }
}

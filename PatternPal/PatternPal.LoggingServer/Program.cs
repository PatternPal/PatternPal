
using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Data;
using PatternPal.LoggingServer.Data.Interfaces;
using PatternPal.LoggingServer.Models;
using PatternPal.LoggingServer.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Grpc.AspNetCore.HealthChecks;
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

        builder.Services.AddGrpcHealthChecks()
            .AddCheck("Sample", () => HealthCheckResult.Healthy());


        builder.Services.AddScoped<EventRepository, EventRepository>();

        WebApplication app = builder.Build();

        DatabaseManagementService.MigrationInitialization(app); 

        app.MapGrpcService<LoggerService>();

        // Run the webapp
        app.Run();
    }
}

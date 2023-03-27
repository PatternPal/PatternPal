
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

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

        builder.Services.AddGrpc();




        builder.Services.AddDbContext<ProgSnap2ContextClass>(options => 
            options.UseNpgsql( builder.Configuration.GetConnectionString("PostgresConnection") )
        );

        // Add the repository
        builder.Services.AddScoped<IRepository<ProgSnap2Event>, EventRepository>();

        WebApplication app = builder.Build();


        //app.UseGrpcWeb(
        //    new GrpcWebOptions
        //    {
        //        DefaultEnabled = true
        //    });
        // configure services

        app.MapGrpcService<LoggerService>();

        app.Run();
    }
}

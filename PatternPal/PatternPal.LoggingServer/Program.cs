#region

using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Data;
using PatternPal.LoggingServer.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;
using PatternPal.LoggingServer.LogJobs;

#endregion

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
            .AddCheck("HealthCheck", () => HealthCheckResult.Healthy());

        builder.Services.AddScoped<EventRepository, EventRepository>();


        // Setup Quartz (Scheduler)
        builder.Services.AddQuartz(q =>
        {
            JobKey jobKey = new JobKey("LoggerJob", "LoggerGroup");
            q.AddJob<ClearCodeStatesJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("LoggerTrigger", "LoggerGroup")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0,0))
                .WithDescription("Removes all redundant CodeStates every day at midnight")
            );

            q.UseMicrosoftDependencyInjectionJobFactory();
        });

        builder.Services.AddQuartzServer(options =>{
                options.WaitForJobsToComplete = true;
        });

        WebApplication app = builder.Build();

        DatabaseManagementService.MigrationInitialization(app); 
        app.MapGrpcService<LoggerService>();

        // Run the webapp
        app.Run();
    }
}

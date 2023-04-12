using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Data;

namespace PatternPal.LoggingServer.Services
{
    public static class DatabaseManagementService
    {
        public static void MigrationInitialization(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ProgSnap2ContextClass>().Database.Migrate();
                // NOTE: This will only work up to 1 instance. If you have multiple instances, you will need to use a distributed lock or something similar. Probably wise to split migration from launch anyway and write a deploy script.
            }
        }
    }
}

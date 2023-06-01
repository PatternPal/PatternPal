#region

using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Data;

#endregion

namespace PatternPal.LoggingServer.Services
{
    public static class DatabaseManagementService
    {
        public static void MigrationInitialization(IApplicationBuilder app)
        {
            using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
            // NOTE: This will only work up to 1 instance. If you have multiple instances, you will need to use a distributed lock or something similar. Probably wise to split migration from launch anyway and write a deploy script.
            serviceScope.ServiceProvider.GetService<ProgSnap2ContextClass>().Database.Migrate();
        }
    }
}

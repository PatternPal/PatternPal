using IDesign.LoggingAPI;
using IDesign.LoggingAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests
{
    public class LoggingControllerTests
    {
        protected LoggingControllerTests(DbContextOptions<TestContext> contextOptions)
        {
            ContextOptions = contextOptions;

            //Seed();
        }

        protected DbContextOptions<TestContext> ContextOptions { get; }

        protected TestContext Seed()
        {
            var context = new TestContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            context.SaveChanges();
            return context;
        }
    }
}

using EmployeeHR.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace EmployeeHR.Dal.Tests
{
    internal static class Helper
    {
        internal static async Task<EmployeeHRDbContext> CreateDbContextAsync(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeHRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var options = optionsBuilder.Options;
            var dbContext = new EmployeeHRDbContext(options);

            await dbContext.Database.MigrateAsync();

            return dbContext;
        }

        internal static TestDbContextFactory CreateDbContextFactory(IConfiguration configuration)
        {
            var factory = new TestDbContextFactory(configuration);
            return factory;
        }


        internal class TestDbContextFactory : IDbContextFactory<EmployeeHRDbContext>
        {
            private readonly IConfiguration _configuration;

            public TestDbContextFactory(IConfiguration configuration)
            {
                this._configuration = configuration;
            }

            public EmployeeHRDbContext CreateDbContext()
            {
                string connectionString = this._configuration.GetConnectionString("DefaultConnection");
                var optionsBuilder = new DbContextOptionsBuilder<EmployeeHRDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                var options = optionsBuilder.Options;
                var dbContext = new EmployeeHRDbContext(options);

                dbContext.Database.Migrate();

                return dbContext;
            }
        }
    }
}

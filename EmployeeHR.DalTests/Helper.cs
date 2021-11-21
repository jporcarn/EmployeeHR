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
    }
}

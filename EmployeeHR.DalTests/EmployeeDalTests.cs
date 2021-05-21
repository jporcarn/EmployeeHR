using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeHR.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeHR.EF;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHR.Dal.Tests
{
    [TestClass()]
    public class EmployeeDalTests
    {
        [TestMethod()]
        public async Task GetAsyncTest()
        {
            EmployeeHRDbContext dbContext = CreateDbContext();

            var dal = new EmployeeDal(dbContext);

            var employees = await dal.GetAsync();

            Assert.IsNotNull(employees);
        }

        private static EmployeeHRDbContext CreateDbContext()
        {
            string connectionString = "Integrated Security=SSPI;Persist Security Info=False;User ID='';Initial Catalog=EmmployeeHRDb;Data Source=.\\sqlexpress;";
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeHRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            var options = optionsBuilder.Options;
            var dbContext = new EmployeeHRDbContext(options);
            return dbContext;
        }
    }
}
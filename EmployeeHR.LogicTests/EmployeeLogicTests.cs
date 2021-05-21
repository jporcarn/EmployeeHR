using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeHR.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeHR.Dal;
using EmployeeHR.EF;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHR.Logic.Tests
{
    [TestClass()]
    public class EmployeeLogicTests
    {
        [TestMethod()]
        public async Task GetAsyncTest()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var dal = new EmployeeLogic(employeeDal);

            var employees = await dal.GetAsync();
            Assert.IsNotNull(employees, "Employees can't be null");
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
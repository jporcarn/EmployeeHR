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
using EmployeeHR.Dto;

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

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var dal = new EmployeeLogic(employeeDal);

            var employee = await dal.GetByIdAsync(1);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == "Palmer");
            Assert.IsTrue(employee.LastName == "Matthew Hogan");
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var dal = new EmployeeLogic(employeeDal);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeAdded = await dal.AddAsync(employeeToAdd);

            Assert.IsNotNull(employeeAdded);
            Assert.IsTrue(employeeAdded.Id > 0);
            Assert.IsTrue(employeeAdded.FirstName == "Test");
            Assert.IsTrue(employeeAdded.LastName == "Case");
        }
    }
}
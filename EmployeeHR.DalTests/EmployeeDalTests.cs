using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeHR.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeHR.EF;
using Microsoft.EntityFrameworkCore;
using EmployeeHR.Dto;

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
            EmployeeHRDbContext dbContext = CreateDbContext();

            var dal = new EmployeeDal(dbContext);

            var employee = await dal.GetByIdAsync(1);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == "Palmer");
            Assert.IsTrue(employee.LastName == "Matthew Hogan");

            Assert.IsTrue(employee.RowVersion.Date != System.DateTime.MinValue, "Row version hasn't been retrieved correctly");
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            EmployeeHRDbContext dbContext = CreateDbContext();

            var dal = new EmployeeDal(dbContext);

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

            Assert.IsTrue(employeeAdded.RowVersion.Date == System.DateTime.Now.Date, "Row version hasn't been either updated or retrieved correctly");
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            EmployeeHRDbContext dbContext = CreateDbContext();

            var dal = new EmployeeDal(dbContext);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeToUpdate = await dal.AddAsync(employeeToAdd);
            var rowVersion = employeeToUpdate.RowVersion;

            System.Threading.Thread.Sleep(1000); // wait for 1 second before attempting to update the record just created
            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            var employeeUpdated = await dal.UpdateAsync(employeeToUpdate);

            Assert.IsTrue(employeeUpdated.Id == employeeToUpdate.Id);
            Assert.IsTrue(employeeUpdated.FirstName == "Test updated");
            Assert.IsTrue(employeeUpdated.LastName == "Case updated");
            Assert.IsTrue(employeeUpdated.RowVersion >= rowVersion);
        }
    }
}
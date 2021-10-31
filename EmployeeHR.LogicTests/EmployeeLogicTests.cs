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
            var logic = new EmployeeLogic(employeeDal);

            var employees = await logic.GetAsync();
            Assert.IsNotNull(employees, "Employees can't be null");
        }

        private static EmployeeHRDbContext CreateDbContext()
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=EmployeeHRDb;Trusted_Connection=True;MultipleActiveResultSets=true";
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
            var logic = new EmployeeLogic(employeeDal);

            var employee = await logic.GetByIdAsync(1);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == "Palmer");
            Assert.IsTrue(employee.LastName == "Matthew Hogan");
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeAdded = await logic.AddAsync(employeeToAdd);

            Assert.IsNotNull(employeeAdded);
            Assert.IsTrue(employeeAdded.Id > 0);
            Assert.IsTrue(employeeAdded.FirstName == "Test");
            Assert.IsTrue(employeeAdded.LastName == "Case");
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeToUpdate = await logic.AddAsync(employeeToAdd);
            var rowVersion = employeeToUpdate.RowVersion;

            System.Threading.Thread.Sleep(1000); // Sleep for 1 second

            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            var employeeUpdated = await logic.UpdateAsync(employeeToUpdate.Id, employeeToUpdate);

            Assert.IsTrue(employeeUpdated.Id == employeeToUpdate.Id);
            Assert.IsTrue(employeeUpdated.FirstName == "Test updated");
            Assert.IsTrue(employeeUpdated.LastName == "Case updated");
            Assert.IsTrue(employeeUpdated.RowVersion >= rowVersion);
        }

        [TestMethod()]
        public async Task UpdateAsyncNegativeTest1()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToUpdate = await logic.GetByIdAsync(1);

            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            try
            {
                var employeeUpdated = await logic.UpdateAsync(2, employeeToUpdate);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        [TestMethod()]
        public async Task UpdateAsyncNegativeTest2()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToUpdate = await logic.GetByIdAsync(1);

            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            try
            {
                var employeeUpdated = await logic.UpdateAsync(99999, employeeToUpdate);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
        }

        [TestMethod()]
        public async Task UpdateAsyncNegativeTest3()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToUpdate = await logic.GetByIdAsync(1);

            // simulate some other user updates same employee before me
            var employeeInOtherThread = await logic.GetByIdAsync(1);
            var employeeInOtherThreadUpdated = await logic.UpdateAsync(employeeInOtherThread.Id, employeeInOtherThread);

            Assert.IsNotNull(employeeInOtherThreadUpdated, "Employee updated in other thread shoudn't be null");

            try
            {
                var employeeUpdated = await logic.UpdateAsync(1, employeeToUpdate);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.Conflict);
            }
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeAdded = await logic.AddAsync(employeeToAdd);
            Assert.IsNotNull(employeeAdded);

            try
            {
                int affectedRecords = await logic.DeleteAsync(employeeAdded.Id, employeeAdded);

                Assert.IsTrue(affectedRecords > 0, "Affected records shoud be higher than 0");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public async Task DeleteAsyncNegativeTest1()
        {
            var dbContext = CreateDbContext();
            var employeeDal = new EmployeeDal(dbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeAdded = await logic.AddAsync(employeeToAdd);
            Assert.IsNotNull(employeeAdded);

            // simulate some other user updates same employee before me
            var rowVersion = employeeAdded.RowVersion;

            System.Threading.Thread.Sleep(1000); // Sleep for 1 second

            var employeeInOtherThread = await logic.GetByIdAsync(employeeAdded.Id);
            var employeeInOtherThreadUpdated = await logic.UpdateAsync(employeeInOtherThread.Id, employeeInOtherThread);

            Assert.IsFalse(rowVersion == employeeInOtherThreadUpdated.RowVersion, "Row version has not been updated accordingly");

            Assert.IsNotNull(employeeInOtherThreadUpdated, "Employee updated in other thread shoudn't be null");

            try
            {
                int affectedRecords = await logic.DeleteAsync(employeeAdded.Id, employeeAdded);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
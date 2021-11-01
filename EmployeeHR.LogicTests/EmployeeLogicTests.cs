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
using Microsoft.Extensions.Configuration;

namespace EmployeeHR.Logic.Tests
{

    [TestClass()]
    public class EmployeeLogicTests
    {
        public static IConfiguration Configuration { get; private set; }
        public static EmployeeHRDbContext DbContext { get; private set; }


        [ClassCleanup] // free resources obtained by all the tests in the test class.
        public static async Task Cleanup()
        {
            System.Diagnostics.Debug.WriteLine("ClassCleanup");
            if (EmployeeLogicTests.DbContext != null)
            {
                await EmployeeLogicTests.DbContext.Database.EnsureDeletedAsync();
                await EmployeeLogicTests.DbContext.DisposeAsync();
            }
        }

        [ClassInitialize] // before any of the tests in the test class have run
        public static async Task Initialize(TestContext context)
        {
            System.Diagnostics.Debug.WriteLine("ClassInitialize");

            EmployeeLogicTests.Configuration = context.Properties["configuration"] as IConfiguration;
            EmployeeLogicTests.DbContext = await CreateDbContextAsync(EmployeeLogicTests.Configuration);
        }

        [TestMethod()]
        public async Task GetAsyncTest()
        {
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employees = await logic.GetAsync();
            Assert.IsNotNull(employees, "Employees can't be null");
        }

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
            var logic = new EmployeeLogic(employeeDal);

            var employee = await logic.GetByIdAsync(1);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == "Palmer");
            Assert.IsTrue(employee.LastName == "Matthew Hogan");
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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
            var employeeDal = new EmployeeDal(EmployeeLogicTests.DbContext);
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

        public static async Task<EmployeeHRDbContext> CreateDbContextAsync(IConfiguration configuration)
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
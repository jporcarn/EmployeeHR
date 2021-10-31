﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        private static EmployeeHRDbContext dbContext;

        [ClassCleanup] // free resources obtained by all the tests in the test class.
        public static async Task Cleanup()
        {
            Console.WriteLine("ClassCleanup");
            if (EmployeeDalTests.dbContext != null)
            {
                await EmployeeDalTests.dbContext.Database.EnsureDeletedAsync();
                await EmployeeDalTests.dbContext.DisposeAsync();
            }
        }

        [ClassInitialize] // before any of the tests in the test class have run
        public static async Task Initialize(TestContext context)
        {
            Console.WriteLine("ClassInitialize");
            EmployeeDalTests.dbContext = await CreateDbContextAsync();
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            var dal = new EmployeeDal(EmployeeDalTests.dbContext);

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
        public async Task GetAsyncTest()
        {
            var dal = new EmployeeDal(EmployeeDalTests.dbContext);

            var employees = await dal.GetAsync();

            Assert.IsNotNull(employees, "Employees can't be null");
        }

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            var dal = new EmployeeDal(EmployeeDalTests.dbContext);

            var employee = await dal.GetByIdAsync(1);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == "Palmer");
            Assert.IsTrue(employee.LastName == "Matthew Hogan");

            Assert.IsTrue(employee.RowVersion.Date != System.DateTime.MinValue, "Row version hasn't been retrieved correctly");
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            var dal = new EmployeeDal(EmployeeDalTests.dbContext);

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

        private static async Task<EmployeeHRDbContext> CreateDbContextAsync()
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=EmployeeHRTestDb;Trusted_Connection=True;MultipleActiveResultSets=true";
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeHRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var options = optionsBuilder.Options;
            var dbContext = new EmployeeHRDbContext(options);

            await dbContext.Database.MigrateAsync();

            return dbContext;
        }
    }
}
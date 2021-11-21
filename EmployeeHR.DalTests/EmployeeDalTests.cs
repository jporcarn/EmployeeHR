using EmployeeHR.Dto;
using EmployeeHR.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace EmployeeHR.Dal.Tests
{

    [TestClass()]
    public class EmployeeDalTests
    {
        public static IConfiguration Configuration { get; private set; }
        public static EmployeeHRDbContext DbContext { get; private set; }


        [ClassCleanup] // free resources obtained by all the tests in the test class.
        public static async Task Cleanup()
        {
            System.Diagnostics.Debug.WriteLine("ClassCleanup");
            if (DbContext != null)
            {
                await DbContext.Database.EnsureDeletedAsync();
                await DbContext.DisposeAsync();
            }
        }

        [ClassInitialize] // before any of the tests in the test class have run
        public static async Task Initialize(TestContext context)
        {
            System.Diagnostics.Debug.WriteLine("ClassInitialize");

            Configuration = context.Properties["configuration"] as IConfiguration;
            DbContext = await Helper.CreateDbContextAsync(EmployeeDalTests.Configuration);
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            var dal = new EmployeeDal(DbContext);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeAddedId = await dal.AddAsync(employeeToAdd);

            var employeeAdded = await dal.GetByIdAsync(employeeAddedId);

            Assert.IsNotNull(employeeAdded);
            Assert.IsTrue(employeeAdded.Id > 0);
            Assert.IsTrue(employeeAdded.FirstName == "Test");
            Assert.IsTrue(employeeAdded.LastName == "Case");

            Assert.IsTrue(employeeAdded.RowVersion.Date == System.DateTime.Now.Date, "Row version hasn't been either updated or retrieved correctly");
        }

        [TestMethod()]
        public async Task GetAsyncTest()
        {
            var dal = new EmployeeDal(DbContext);

            var employees = await dal.GetAsync();

            Assert.IsNotNull(employees, "Employees can't be null");
        }

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            var dal = new EmployeeDal(DbContext);

            var employee = await dal.GetByIdAsync(1);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == "Palmer");
            Assert.IsTrue(employee.LastName == "Matthew Hogan");

            Assert.IsTrue(employee.RowVersion.Date != System.DateTime.MinValue, "Row version hasn't been retrieved correctly");
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            var dal = new EmployeeDal(DbContext);

            var employeeToAdd = new Employee
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Case",
                SocialSecurityNumber = "12345678",
                PhoneNumber = "000000000"
            };

            var employeeToUpdateId = await dal.AddAsync(employeeToAdd);

            var employeeToUpdate = await dal.GetByIdAsync(employeeToUpdateId);

            var rowVersion = employeeToUpdate.RowVersion;

            System.Threading.Thread.Sleep(1000); // wait for 1 second before attempting to update the record just created
            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            var n = await dal.UpdateAsync(employeeToUpdate);
            Assert.AreEqual(1, n);

            var employeeUpdated = await dal.GetByIdAsync(employeeToUpdate.Id);

            Assert.IsTrue(employeeUpdated.Id == employeeToUpdate.Id);
            Assert.IsTrue(employeeUpdated.FirstName == "Test updated");
            Assert.IsTrue(employeeUpdated.LastName == "Case updated");
            Assert.IsTrue(employeeUpdated.RowVersion >= rowVersion);
        }


    }
}
using EmployeeHR.Dto;
using EmployeeHR.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace EmployeeHR.Dal.Tests
{
    [TestClass()]
    public class EmployeeUnitOfWorkTests
    {

        public static IConfiguration Configuration { get; private set; }
        public static EmployeeHRDbContext DbContext { get; private set; }
        public static IDbContextFactory<EmployeeHRDbContext> DbContextFactory { get; private set; }

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
            DbContext = await Helper.CreateDbContextAsync(Configuration);

            DbContextFactory = Helper.CreateDbContextFactory(Configuration);
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            // Mock
            Employee employeeToAddMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();

            // Test

            var unitOfWork = new EmployeeUnitOfWork(DbContextFactory);

            var employeeAdded = await unitOfWork.AddAsync(employeeToAddMock);

            Assert.IsNotNull(employeeAdded);
            Assert.IsTrue(employeeAdded.Id > 0);
            Assert.IsTrue(employeeAdded.FirstName == employeeToAddMock.FirstName);
            Assert.IsTrue(employeeAdded.LastName == employeeToAddMock.LastName);


        }

    }
}
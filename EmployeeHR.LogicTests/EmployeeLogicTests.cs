using EmployeeHR.Dto;
using EmployeeHR.Interfaces;
using EmployeeHR.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeHR.Logic.Tests
{

    [TestClass()]
    public class DomainTest
    {
        public static IConfiguration Configuration { get; private set; }


        [ClassCleanup] // free resources obtained by all the tests in the test class.
        public static void Cleanup()
        {
            System.Diagnostics.Debug.WriteLine("ClassCleanup");

        }

        [ClassInitialize] // before any of the tests in the test class have run
        public static void Initialize(TestContext context)
        {
            System.Diagnostics.Debug.WriteLine("ClassInitialize");

            Configuration = context.Properties["configuration"] as IConfiguration;
        }
    }

    [TestClass()]
    public class EmployeeLogicTests : DomainTest
    {

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            // Mock
            Employee employeeToAdd = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();

            IEmployeeUnitOfwork employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                 .WithGetAll()
                 .WithAdd(employeeToAdd)
                 .WithGetById(employeeToAdd.Id + 1)
                 .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            var employeeAdded = await logic.AddAsync(employeeToAdd);

            Assert.IsNotNull(employeeAdded);
            Assert.IsTrue(employeeAdded.Id > 0);
            Assert.IsTrue(employeeAdded.FirstName == employeeToAdd.FirstName);
            Assert.IsTrue(employeeAdded.LastName == employeeToAdd.LastName);

        }

        [TestMethod()]
        [DataRow(1)]
        public async Task DeleteAsyncNegativeTest1(int employeeId)
        {
            // Mock
            Employee employeeToDeleteMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeToDeleteMock.Id = employeeId;

            IEmployeeUnitOfwork employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                 .WithGetAll()
                 .WithGetById(employeeId)
                 .WithDelete(employeeToDeleteMock)
                 .AsObject();

            var employeeToDelete = (await employeeUnitOfwork.GetAsync()).FirstOrDefault(e => e.Id == employeeId);
            Assert.IsTrue(employeeToDelete != null, "Mock data has not been set up correctly");

            // simulate some other user updates same employee before me
            var employeeInOtherThreadUpdated = employeeToDelete.Clone() as Employee;
            Assert.IsNotNull(employeeInOtherThreadUpdated, "Employee updated in other thread shoudn't be null");
            employeeInOtherThreadUpdated.RowVersion = employeeInOtherThreadUpdated.RowVersion.AddMilliseconds(300);


            employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                .WithGetAll()
                .WithGetById(employeeId, employeeInOtherThreadUpdated)
                .WithDelete(employeeToDelete)
                .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            try
            {
                int affectedRecords = await logic.DeleteAsync(employeeId, employeeToDelete);

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

        [TestMethod()]
        [DataRow(1)]
        public async Task DeleteAsyncTest(int employeeId)
        {
            // Mock

            Employee employeeToDeleteMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeToDeleteMock.Id = employeeId;

            IEmployeeUnitOfwork employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                 .WithGetAll()
                 .WithGetById(employeeId)
                 .WithDelete(employeeToDeleteMock)
                 .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            try
            {
                int affectedRecords = await logic.DeleteAsync(employeeId, employeeToDeleteMock);

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
        public async Task GetAsyncTest()
        {
            // Mock
            IEmployeeUnitOfwork employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                .WithGetAll()
                .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            var employees = await logic.GetAsync();
            Assert.IsNotNull(employees, "Employees can't be null");
        }

        [TestMethod()]
        [DataRow(1)]
        public async Task GetByIdAsyncTest(int employeeId)
        {
            // Mock
            IEmployeeUnitOfwork employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                 .WithGetAll()
                 .WithGetById(employeeId)
                 .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            var employee = await logic.GetByIdAsync(employeeId);

            var employees = await logic.GetAsync();

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == employees.FirstOrDefault(e => e.Id == employeeId).FirstName);
            Assert.IsTrue(employee.LastName == employees.FirstOrDefault(e => e.Id == employeeId).LastName);
        }
        [TestMethod()]
        [DataRow(1)]
        public async Task UpdateAsyncNegativeTest1(int employeeId)
        {
            // Mock
            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";

            const int WRONG_ID_THAT_EXISTS = 2;
            var employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                            .WithGetAll()
                            .WithGetById(employeeId)
                            .WithGetById(WRONG_ID_THAT_EXISTS)
                            .WithUpdate(employeeMock, employeeUpdatedMock)
                            .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            var employeeToUpdate = employeeMock;
            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            try
            {

                var employeeUpdated = await logic.UpdateAsync(WRONG_ID_THAT_EXISTS, employeeToUpdate);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        [TestMethod()]
        [DataRow(1)]
        public async Task UpdateAsyncNegativeTest2(int employeeId)
        {
            // Mock
            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";

            var employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                .WithGetAll()
                .WithGetById(employeeId)
                .WithUpdate(employeeMock, employeeUpdatedMock)
                .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            var employeeToUpdate = employeeMock;
            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            try
            {
                const int WRONG_ID_THAT_DOES_NOT_EXIST = -1;

                var employeeUpdated = await logic.UpdateAsync(WRONG_ID_THAT_DOES_NOT_EXIST, employeeToUpdate);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
        }

        [TestMethod()]
        [DataRow(1)]
        public async Task UpdateAsyncNegativeTest3(int employeeId)
        {
            // Mock
            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";

            // simulate some other user updates same employee before me
            var employeeInOtherThreadUpdated = employeeMock.Clone() as Employee;
            Assert.IsNotNull(employeeInOtherThreadUpdated, "Employee updated in other thread shoudn't be null");
            employeeInOtherThreadUpdated.RowVersion = employeeInOtherThreadUpdated.RowVersion.AddMilliseconds(300);


            var employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                .WithGetAll()
                .WithGetById(employeeId, employeeInOtherThreadUpdated)
                .WithUpdate(employeeMock, employeeUpdatedMock)
                .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            var employeeToUpdate = employeeMock;
            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            try
            {
                var employeeUpdated = await logic.UpdateAsync(employeeId, employeeToUpdate);

                Assert.Fail("Employee shoudn't have been updated");
            }
            catch (CustomException ex)
            {
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.Conflict);
            }
        }

        [TestMethod()]
        [DataRow(1)]
        public async Task UpdateAsyncTest(int employeeId)
        {
            // Mock
            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";
            employeeUpdatedMock.RowVersion = employeeUpdatedMock.RowVersion.AddMilliseconds(300);

            var employeeToUpdate = employeeMock.Clone() as Employee;

            IEmployeeUnitOfwork employeeUnitOfwork = new FluentEmployeeUnitOfworkMock()
                 .WithGetAll()
                 .WithGetById(employeeId, employeeMock)
                 .WithUpdate(employeeToUpdate, employeeUpdatedMock)
                 .AsObject();

            // Test
            var logic = new EmployeeLogic(employeeUnitOfwork);

            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            var employeeUpdated = await logic.UpdateAsync(employeeToUpdate.Id, employeeToUpdate);

            Assert.IsNotNull(employeeUpdated);
            Assert.IsTrue(employeeUpdated.Id == employeeMock.Id);
            Assert.IsTrue(employeeUpdated.FirstName == "Test updated");
            Assert.IsTrue(employeeUpdated.LastName == "Case updated");
            Assert.IsTrue(employeeUpdated.RowVersion >= employeeMock.RowVersion);
        }
    }
}
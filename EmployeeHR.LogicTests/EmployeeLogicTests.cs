using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeHR.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeHR.Dto;
using Microsoft.Extensions.Configuration;
using Moq;
using EmployeeHR.Interfaces;

namespace EmployeeHR.Logic.Tests
{

    [TestClass()]
    public class EmployeeLogicTests
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

            EmployeeLogicTests.Configuration = context.Properties["configuration"] as IConfiguration;
        }

        [TestMethod()]
        public async Task GetAsyncTest()
        {
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mock = new Mock<IEmployeeDal>();

            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(100)
                .Build()
                .AsEnumerable();

            mock
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            IEmployeeDal employeeDal = mock.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

            var employees = await logic.GetAsync();
            Assert.IsNotNull(employees, "Employees can't be null");
        }

        [TestMethod()]
        [DataRow(1)]
        public async Task GetByIdAsyncTest(int employeeId)
        {
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mock = new Mock<IEmployeeDal>();

            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(100)
                .Build()
                .AsEnumerable();

            mock
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            mock
                .Setup(m => m.GetByIdAsync(employeeId))
                .Returns(Task.FromResult(employeesMoke.FirstOrDefault(e => e.Id == 1)));

            IEmployeeDal employeeDal = mock.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

            var employee = await logic.GetByIdAsync(employeeId);

            Assert.IsNotNull(employee);
            Assert.IsTrue(employee.FirstName == employeesMoke.FirstOrDefault(e => e.Id == 1).FirstName);
            Assert.IsTrue(employee.LastName == employeesMoke.FirstOrDefault(e => e.Id == 1).LastName);
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mockEmployeeDal = new Mock<IEmployeeDal>();

            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(100)
                .Build()
                .AsEnumerable();

            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();

            var expected = employeeMock.Clone() as Employee;
            expected.Id = employeesMoke.Count() + 1;

            mockEmployeeDal
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            mockEmployeeDal
                .Setup(m => m.GetByIdAsync(expected.Id))
                .Returns(Task.FromResult(expected));

            mockEmployeeDal
                .Setup(m => m.AddAsync(employeeMock))
                .Returns(Task.FromResult(expected));

            IEmployeeDal employeeDal = mockEmployeeDal.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

            var employeeAdded = await logic.AddAsync(employeeMock);

            Assert.IsNotNull(employeeAdded);
            Assert.IsTrue(employeeAdded.Id > 0);
            Assert.IsTrue(employeeAdded.FirstName == employeeMock.FirstName);
            Assert.IsTrue(employeeAdded.LastName == employeeMock.LastName);

        }

        [TestMethod()]
        [DataRow(1)]
        public async Task UpdateAsyncTest(int employeeId)
        {
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mockEmployeeDal = new Mock<IEmployeeDal>();

            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            mockEmployeeDal
               .Setup(m => m.GetByIdAsync(employeeId))
               .Returns(Task.FromResult(employeeMock));

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";
            employeeUpdatedMock.RowVersion = employeeUpdatedMock.RowVersion.AddMilliseconds(300);

            mockEmployeeDal
                .Setup(m => m.UpdateAsync(employeeMock))
                .Returns(Task.FromResult(employeeUpdatedMock));

            IEmployeeDal employeeDal = mockEmployeeDal.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

            var employeeToUpdate = employeeMock;
            employeeToUpdate.FirstName = "Test updated";
            employeeToUpdate.LastName = "Case updated";

            var employeeUpdated = await logic.UpdateAsync(employeeMock.Id, employeeToUpdate);

            Assert.IsTrue(employeeUpdated.Id == employeeMock.Id);
            Assert.IsTrue(employeeUpdated.FirstName == "Test updated");
            Assert.IsTrue(employeeUpdated.LastName == "Case updated");
            Assert.IsTrue(employeeUpdated.RowVersion >= employeeMock.RowVersion);
        }

        [TestMethod()]
        [DataRow(1)]
        public async Task UpdateAsyncNegativeTest1(int employeeId)
        {
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mockEmployeeDal = new Mock<IEmployeeDal>();

            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            mockEmployeeDal
               .Setup(m => m.GetByIdAsync(employeeId))
               .Returns(Task.FromResult(employeeMock));

            const int WRONG_ID_THAT_EXISTS = 2;
            Employee employeeMock2 = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = WRONG_ID_THAT_EXISTS;
            mockEmployeeDal
                          .Setup(m => m.GetByIdAsync(WRONG_ID_THAT_EXISTS))
                          .Returns(Task.FromResult(employeeMock2));

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";

            mockEmployeeDal
                .Setup(m => m.UpdateAsync(employeeMock))
                .Returns(Task.FromResult(employeeUpdatedMock));

            IEmployeeDal employeeDal = mockEmployeeDal.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

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
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mockEmployeeDal = new Mock<IEmployeeDal>();

            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            mockEmployeeDal
               .Setup(m => m.GetByIdAsync(employeeId))
               .Returns(Task.FromResult(employeeMock));

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";

            mockEmployeeDal
                .Setup(m => m.UpdateAsync(employeeMock))
                .Returns(Task.FromResult(employeeUpdatedMock));

            IEmployeeDal employeeDal = mockEmployeeDal.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

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
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mockEmployeeDal = new Mock<IEmployeeDal>();

            Employee employeeMock = FizzWare.NBuilder.Builder<Employee>.CreateNew().Build();
            employeeMock.Id = employeeId;

            var employeeUpdatedMock = employeeMock.Clone() as Employee;
            employeeUpdatedMock.FirstName = "Test updated";
            employeeUpdatedMock.LastName = "Case updated";

            mockEmployeeDal
                .Setup(m => m.UpdateAsync(employeeMock))
                .Returns(Task.FromResult(employeeUpdatedMock));

            // simulate some other user updates same employee before me
            var employeeInOtherThreadUpdated = employeeMock.Clone() as Employee;
            Assert.IsNotNull(employeeInOtherThreadUpdated, "Employee updated in other thread shoudn't be null");
            employeeInOtherThreadUpdated.RowVersion = employeeInOtherThreadUpdated.RowVersion.AddMilliseconds(300);

            mockEmployeeDal
               .Setup(m => m.GetByIdAsync(employeeId))
               .Returns(Task.FromResult(employeeInOtherThreadUpdated));

            IEmployeeDal employeeDal = mockEmployeeDal.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

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
        public async Task DeleteAsyncTest(int employeeId)
        {

            // Mock IEmployeeDal
            Mock<IEmployeeDal> mock = new Mock<IEmployeeDal>();

            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(100)
                .Build()
                .AsEnumerable();

            mock
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            var employeeToDelete = employeesMoke.FirstOrDefault(e => e.Id == employeeId);
            Assert.IsTrue(employeeToDelete != null, "Mock data has not been set up correctly");

            mock
                .Setup(m => m.GetByIdAsync(employeeId))
                .Returns(Task.FromResult(employeeToDelete));

            mock
                .Setup(m => m.DeleteAsync(employeeToDelete))
                .Returns(Task.FromResult(1));


            IEmployeeDal employeeDal = mock.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

            try
            {
                int affectedRecords = await logic.DeleteAsync(employeeId, employeeToDelete);

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
        [DataRow(1)]
        public async Task DeleteAsyncNegativeTest1(int employeeId)
        {
            // Mock IEmployeeDal
            Mock<IEmployeeDal> mock = new Mock<IEmployeeDal>();

            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(100)
                .Build()
                .AsEnumerable();

            mock
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            var employeeToDelete = employeesMoke.FirstOrDefault(e => e.Id == employeeId);
            Assert.IsTrue(employeeToDelete != null, "Mock data has not been set up correctly");

            // simulate some other user updates same employee before me
            var employeeInOtherThreadUpdated = employeeToDelete.Clone() as Employee;
            Assert.IsNotNull(employeeInOtherThreadUpdated, "Employee updated in other thread shoudn't be null");
            employeeInOtherThreadUpdated.RowVersion = employeeInOtherThreadUpdated.RowVersion.AddMilliseconds(300);

            mock
                .Setup(m => m.GetByIdAsync(employeeId))
                .Returns(Task.FromResult(employeeInOtherThreadUpdated));

            mock
                .Setup(m => m.DeleteAsync(employeeToDelete))
                .Returns(Task.FromResult(1));


            IEmployeeDal employeeDal = mock.Object;

            // Test
            var logic = new EmployeeLogic(employeeDal);

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

    }
}
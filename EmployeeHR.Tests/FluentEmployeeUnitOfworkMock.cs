using EmployeeHR.Dto;
using EmployeeHR.Interfaces;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeHR.Tests
{

    public class FluentEmployeeUnitOfworkMock : IFluentMock<IEmployeeUnitOfwork>
    {
        private Mock<IEmployeeUnitOfwork> unitOfWorkMock = new Mock<IEmployeeUnitOfwork>();

        public IEmployeeUnitOfwork AsObject()
        {
            IEmployeeUnitOfwork unitOfWork = unitOfWorkMock.Object;

            return unitOfWork;
        }

        public FluentEmployeeUnitOfworkMock WithAdd(Employee employeeToAdd)
        {
            var employees = this.AsObject().GetAsync().Result;

            var expected = employeeToAdd.Clone() as Employee;
            expected.Id = employees.Count() + 1;

            this.unitOfWorkMock
                .Setup(m => m.AddAsync(employeeToAdd))
                .Returns(Task.FromResult(expected));

            return this;
        }

        public FluentEmployeeUnitOfworkMock WithDelete(Employee employeeToDelete, int expected = 1)
        {

            this.unitOfWorkMock
                .Setup(m => m.DeleteAsync(employeeToDelete))
                .Returns(Task.FromResult(expected));

            return this;
        }

        public FluentEmployeeUnitOfworkMock WithDelete(int expected)
        {

            this.unitOfWorkMock
                .Setup(m => m.DeleteAsync(It.IsAny<Employee>()))
                .Returns(Task.FromResult(expected));

            return this;
        }

        public FluentEmployeeUnitOfworkMock WithGetAll(int size = 1000)
        {
            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(size)
                .Build()
                .AsEnumerable();

            this.unitOfWorkMock
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            return this;
        }
        public FluentEmployeeUnitOfworkMock WithGetById(int employeeId)
        {
            var expected = this.AsObject().GetAsync().Result.FirstOrDefault(e => e.Id == employeeId);

            this.unitOfWorkMock
                .Setup(m => m.GetByIdAsync(employeeId))
                .Returns(Task.FromResult(expected));

            return this;
        }

        public FluentEmployeeUnitOfworkMock WithGetById(int employeeId, Employee expected)
        {
            this.unitOfWorkMock
                .Setup(m => m.GetByIdAsync(employeeId))
                .Returns(Task.FromResult(expected));

            return this;
        }

        public FluentEmployeeUnitOfworkMock WithUpdate(Employee original, Employee expected)
        {

            this.unitOfWorkMock
                .Setup(m => m.UpdateAsync(original))
                .Returns(Task.FromResult(expected));

            return this;
        }
    }

}

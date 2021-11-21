using EmployeeHR.Dto;
using EmployeeHR.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeHR.Tests
{
    public class FluentEmployeeDalMock : IFluentMock<IEmployeeDal>
    {
        private Mock<IEmployeeDal> employeeDalMock = new Mock<IEmployeeDal>();

        public IEmployeeDal AsObject()
        {
            IEmployeeDal employeeDal = employeeDalMock.Object;

            return employeeDal;
        }

        public FluentEmployeeDalMock WithAdd(Employee employeeToAdd, IEnumerable<Employee> employees)
        {
            var expected = employeeToAdd.Clone() as Employee;
            expected.Id = employees.Count() + 1;

            this.employeeDalMock
                .Setup(m => m.AddAsync(employeeToAdd))
                .Returns(Task.FromResult(expected.Id));

            return this;
        }

        public FluentEmployeeDalMock WithGetAll()
        {
            var employeesMoke = FizzWare.NBuilder.Builder<Employee>
                .CreateListOfSize(100)
                .Build()
                .AsEnumerable();

            this.employeeDalMock
                .Setup(m => m.GetAsync())
                .Returns(Task.FromResult(employeesMoke));

            return this;
        }
        public FluentEmployeeDalMock WithGetById(Employee employee)
        {
            this.employeeDalMock
                .Setup(m => m.GetByIdAsync(employee.Id))
                .Returns(Task.FromResult(employee));

            return this;
        }

        public FluentEmployeeDalMock WithUpdate(Employee employeeToUpdate)
        {
            var expected = employeeToUpdate.Clone() as Employee;
            expected.RowVersion = System.DateTime.Now;

            this.employeeDalMock
                .Setup(m => m.GetByIdAsync(employeeToUpdate.Id))
                .Returns(Task.FromResult(expected));

            return this;
        }

        public FluentEmployeeDalMock WithDelete(Employee employeeToDeleteId)
        {
            var expected = employeeToDeleteId.Clone() as Employee;
            expected.RowVersion = System.DateTime.Now.AddMilliseconds(300);

            this.employeeDalMock
                .Setup(m => m.DeleteAsync(employeeToDeleteId))
                .Returns(Task.FromResult(1));

            return this;
        }
    }

}

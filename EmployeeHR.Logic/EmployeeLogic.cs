using EmployeeHR.Dto;
using EmployeeHR.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHR.Logic
{
    public class EmployeeLogic : IEmployeeLogic
    {
        private readonly IEmployeeUnitOfwork _employeeUnitOfwork;

        public EmployeeLogic(IEmployeeUnitOfwork employeeUnitOfwork)
        {

            this._employeeUnitOfwork = employeeUnitOfwork;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            // TODO: Validations

            var employeeAdded = await this._employeeUnitOfwork.AddAsync(employee);

            return employeeAdded;
        }

        public async Task<int> DeleteAsync(int id, Employee employee)
        {
            var employeeOriginal = await this._employeeUnitOfwork.GetByIdAsync(id);

            // Validations
            if (employeeOriginal == null)
            {
                throw new CustomException($"Employee {id} not found") { StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            if (employeeOriginal.Id != employee.Id)
            {
                throw new CustomException("Ids don't match") { StatusCode = System.Net.HttpStatusCode.BadRequest };
            }

            // Concurrency validation. Avoid data base update
            if (employeeOriginal.RowVersion != employee.RowVersion)
            {
                throw new CustomException("Data has changed recently. Please refresh your data to get latest changes and try again") { StatusCode = System.Net.HttpStatusCode.Conflict };
            }

            int affectedRecords = await this._employeeUnitOfwork.DeleteAsync(employee);

            return affectedRecords;
        }

        public async Task<IEnumerable<Employee>> GetAsync()
        {
            var employees = await this._employeeUnitOfwork.GetAsync();

            return employees;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await this._employeeUnitOfwork.GetByIdAsync(id);

            return employee;
        }

        public async Task<Employee> UpdateAsync(int id, Employee employee)
        {
            var employeeOriginal = await this._employeeUnitOfwork.GetByIdAsync(id);

            // Validations
            if (employeeOriginal == null)
            {
                throw new CustomException($"Employee {id} not found") { StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            if (employeeOriginal.Id != employee.Id)
            {
                throw new CustomException("Ids don't match") { StatusCode = System.Net.HttpStatusCode.BadRequest };
            }

            // Concurrency validation. Avoid data base update

            if (employeeOriginal.RowVersion != employee.RowVersion)
            {
                throw new CustomException("Data has changed recently. Please refresh your data to get latest changes and try again") { StatusCode = System.Net.HttpStatusCode.Conflict };
            }

            var employeeUpdated = await this._employeeUnitOfwork.UpdateAsync(employee);

            return employeeUpdated;
        }
    }
}
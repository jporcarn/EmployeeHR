using EmployeeHR.Dto;
using EmployeeHR.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHR.Logic
{
    public class EmployeeLogic : IEmployeeLogic
    {
        private readonly IEmployeeDal _employeeDal;

        public EmployeeLogic(IEmployeeDal employeeDal)
        {
            this._employeeDal = employeeDal;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            // TODO: Validations

            var employeeAdded = await this._employeeDal.AddAsync(employee);

            return employeeAdded;
        }

        public async Task<int> DeleteAsync(int id, Employee employee)
        {
            var employeeOriginal = await this._employeeDal.GetByIdAsync(id);

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

            int affectedRecords = await this._employeeDal.DeleteAsync(employee);

            return affectedRecords;
        }

        public async Task<IEnumerable<Employee>> GetAsync()
        {
            var employees = await this._employeeDal.GetAsync();

            return employees;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await this._employeeDal.GetByIdAsync(id);

            return employee;
        }

        public async Task<Employee> UpdateAsync(int id, Employee employee)
        {
            var employeeOriginal = await this._employeeDal.GetByIdAsync(id);

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

            var employeeUpdated = await this._employeeDal.UpdateAsync(employee);

            return employeeUpdated;
        }
    }
}
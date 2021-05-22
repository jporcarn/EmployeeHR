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

        public async Task<List<Employee>> GetAsync()
        {
            var employees = await this._employeeDal.GetAsync();

            return employees;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await this._employeeDal.GetByIdAsync(id);

            return employee;
        }
    }
}
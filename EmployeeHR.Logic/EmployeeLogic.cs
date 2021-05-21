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

        public async Task<List<Employee>> GetAsync()
        {
            var employees = await this._employeeDal.GetAsync();

            return employees;
        }
    }
}
using EmployeeHR.Dto;
using EmployeeHR.EF;
using EmployeeHR.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHR.Dal
{
    public class EmployeeDal : IEmployeeDal
    {
        private readonly EmployeeHRDbContext _dbContext;

        public EmployeeDal(EmployeeHRDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Employee>> GetAsync()
        {
            var employees = await this._dbContext.Employee.ToListAsync();
            return employees;
        }
    }
}
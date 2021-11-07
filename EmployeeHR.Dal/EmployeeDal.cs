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

        public async Task<Employee> AddAsync(Employee employee)
        {
            employee.Id = 0;
            this._dbContext.Employee.Add(employee);
            await this._dbContext.SaveChangesAsync();

            this._dbContext.Entry(employee).State = EntityState.Detached; // Untrack the instance of entity

            var employeeAdded = await this.GetByIdAsync(employee.Id);
            return employeeAdded;
        }

        public async Task<int> DeleteAsync(Employee employee)
        {
            this._dbContext.Employee.Remove(employee);

            int affectedRecords = await this._dbContext.SaveChangesAsync();

            return affectedRecords;
        }

        public async Task<IEnumerable<Employee>> GetAsync()
        {
            var employees = await this._dbContext
                .Employee
                .AsNoTracking()
                .ToListAsync();

            return employees;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await this._dbContext
                .Employee
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == id);

            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var entry = this._dbContext.Employee.Update(employee);

            entry.Property(p => p.Id).IsModified = false;

            var n = await this._dbContext.SaveChangesAsync();

            this._dbContext.Entry(employee).State = EntityState.Detached; // Untrack the instance of entity

            var employeeUpdated = await this.GetByIdAsync(employee.Id);

            return employeeUpdated;
        }
    }
}
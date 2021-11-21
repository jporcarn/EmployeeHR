using EmployeeHR.Dto;
using EmployeeHR.EF;
using EmployeeHR.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<int> AddAsync(Employee employee)
        {
            employee.Id = 0;
            this._dbContext.Employee.Add(employee);
            await this._dbContext.SaveChangesAsync();

            this._dbContext.Entry(employee).State = EntityState.Detached; // Untrack the instance of entity

            return employee.Id;
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

        public async Task<int> UpdateAsync(Employee employee)
        {
            var entry = this._dbContext.Employee.Update(employee);

            entry.Property(p => p.Id).IsModified = false;

            var n = await this._dbContext.SaveChangesAsync();

            this._dbContext.Entry(employee).State = EntityState.Detached; // Untrack the instance of entity

            return n;
        }

        #region IDisposable
        private bool disposedValue;
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
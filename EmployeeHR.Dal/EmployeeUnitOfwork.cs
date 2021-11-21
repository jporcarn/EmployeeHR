using EmployeeHR.Dto;
using EmployeeHR.EF;
using EmployeeHR.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHR.Dal
{
    public class EmployeeUnitOfwork : IEmployeeUnitOfwork
    {
        private readonly EmployeeHRDbContext _context;
        private readonly IEmployeeDal _employeeDal;


        public EmployeeUnitOfwork(EmployeeHRDbContext context)
        {
            this._context = context;
            this._employeeDal = new EmployeeDal(this._context);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            employee.Id = 0;
            await this._employeeDal.AddAsync(employee);

            var employeeAdded = await this._employeeDal.GetByIdAsync(employee.Id);
            return employeeAdded;
        }

        public Task<int> DeleteAsync(Employee employee)
        {
            return this._employeeDal.DeleteAsync(employee);
        }

        public Task<IEnumerable<Employee>> GetAsync()
        {
            return this._employeeDal.GetAsync();
        }

        public Task<Employee> GetByIdAsync(int id)
        {
            return this._employeeDal.GetByIdAsync(id);
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            await this._employeeDal.UpdateAsync(employee);

            var employeeAdded = await this._employeeDal.GetByIdAsync(employee.Id);
            return employeeAdded;

        }


        #region IDisposable
        private bool disposedValue;
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    this._employeeDal.Dispose();

                }


                disposedValue = true;
            }
        }

        #endregion

    }
}

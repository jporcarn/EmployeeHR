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

        private IEmployeeDal _employeeDal;
        public IEmployeeDal EmployeeDal
        {
            get
            {

                if (this._employeeDal == null)
                {
                    this._employeeDal = new EmployeeDal(this._context);
                }

                return this._employeeDal;
            }
        }


        public EmployeeUnitOfwork(EmployeeHRDbContext context)
        {
            this._context = context;

        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            employee.Id = 0;
            await this.EmployeeDal.AddAsync(employee);

            var employeeAdded = await this.EmployeeDal.GetByIdAsync(employee.Id);
            return employeeAdded;
        }

        public Task<int> DeleteAsync(Employee employee)
        {
            return this.EmployeeDal.DeleteAsync(employee);
        }

        public Task<IEnumerable<Employee>> GetAsync()
        {
            return this.EmployeeDal.GetAsync();
        }

        public Task<Employee> GetByIdAsync(int id)
        {
            return this.EmployeeDal.GetByIdAsync(id);
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            await this.EmployeeDal.UpdateAsync(employee);

            var employeeAdded = await this.EmployeeDal.GetByIdAsync(employee.Id);
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
                    this.EmployeeDal.Dispose();

                }


                disposedValue = true;
            }
        }

        #endregion

    }
}

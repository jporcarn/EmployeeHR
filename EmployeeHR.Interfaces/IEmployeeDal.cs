using EmployeeHR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHR.Interfaces
{
    public interface IEmployeeDal : IDisposable
    {
        Task<int> AddAsync(Employee employee);

        Task<int> DeleteAsync(Employee employee);

        Task<IEnumerable<Employee>> GetAsync();

        Task<Employee> GetByIdAsync(int id);

        Task<int> UpdateAsync(Employee employee);
    }
}
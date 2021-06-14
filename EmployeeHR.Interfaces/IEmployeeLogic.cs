using EmployeeHR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHR.Interfaces
{
    public interface IEmployeeLogic
    {
        Task<Employee> AddAsync(Employee employee);

        Task<List<Employee>> GetAsync();

        Task<Employee> GetByIdAsync(int id);

        Task<Employee> UpdateAsync(int id, Employee employee);

        Task<int> DeleteAsync(int id, Employee employee);
    }
}
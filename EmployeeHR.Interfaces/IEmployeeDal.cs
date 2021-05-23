using EmployeeHR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHR.Interfaces
{
    public interface IEmployeeDal
    {
        Task<Employee> AddAsync(Employee employee);

        Task<List<Employee>> GetAsync();

        Task<Employee> GetByIdAsync(int id);

        Task<Employee> UpdateAsync(Employee employee);
    }
}
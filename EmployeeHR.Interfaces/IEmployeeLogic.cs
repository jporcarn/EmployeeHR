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
        Task<List<Employee>> GetAsync();
    }
}
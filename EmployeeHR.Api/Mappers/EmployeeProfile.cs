using AutoMapper;
using EmployeeHR.Api.Models;
using EmployeeHR.Dto;

namespace EmployeeHR.Api.Mappers
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<CreateEmployeeRequest, Employee>(MemberList.Source);
        }
    }
}

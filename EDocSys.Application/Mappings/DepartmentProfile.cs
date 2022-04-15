using EDocSys.Application.Features.Departments.Commands.Create;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Departments.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;
using EDocSys.Domain.Entities.DocumentationMaster;

namespace EDocSys.Application.Mappings
{
    internal class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CreateDepartmentCommand, Department>().ReverseMap();
            CreateMap<GetDepartmentByIdResponse, Department>().ReverseMap();
            CreateMap<GetAllDepartmentsCachedResponse, Department>().ReverseMap();
        }
    }
}
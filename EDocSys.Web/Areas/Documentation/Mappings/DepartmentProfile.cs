using EDocSys.Application.Features.Departments.Commands.Create;
using EDocSys.Application.Features.Departments.Commands.Update;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Departments.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;

namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<GetAllDepartmentsCachedResponse, DepartmentViewModel>().ReverseMap();
            CreateMap<GetDepartmentByIdResponse, DepartmentViewModel>().ReverseMap();
            CreateMap<CreateDepartmentCommand, DepartmentViewModel>().ReverseMap();
            CreateMap<UpdateDepartmentCommand, DepartmentViewModel>().ReverseMap();
        }
        
    }
}

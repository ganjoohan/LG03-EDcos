using EDocSys.Application.Features.UserApprovers.Commands.Create;
using EDocSys.Application.Features.UserApprovers.Commands.Update;
using EDocSys.Application.Features.UserApprovers.Queries.GetAllCached;
using EDocSys.Application.Features.UserApprovers.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.UserMaster;

namespace EDocSys.Web.Areas.Documentation.Mappings
{
    public class UserApproverProfile : Profile
    {
        public UserApproverProfile()
        {
            CreateMap<UserApprover, GetAllUserApproversCachedResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name));

            CreateMap<GetAllUserApproversCachedResponse, UserApproverViewModel>().ReverseMap();


            CreateMap<UserApprover, GetUserApproverByIdResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name));


            CreateMap<GetUserApproverByIdResponse, UserApproverViewModel>().ReverseMap();
            CreateMap<CreateUserApproverCommand, UserApproverViewModel>().ReverseMap();
            CreateMap<UpdateUserApproverCommand, UserApproverViewModel>().ReverseMap();
        }
    }
}

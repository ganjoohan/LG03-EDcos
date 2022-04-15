using EDocSys.Application.Features.UserApprovers.Commands.Create;
using EDocSys.Application.Features.UserApprovers.Queries.GetAllCached;
using EDocSys.Application.Features.UserApprovers.Queries.GetById;
using EDocSys.Domain.Entities.UserMaster;
using AutoMapper;
using EDocSys.Application.Features.UserApprovers.Queries.GetAllPaged;

namespace EDocSys.Application.Mappings
{
    internal class UserApproverProfile : Profile
    {
        public UserApproverProfile()
        {
            CreateMap<CreateUserApproverCommand, UserApprover>().ReverseMap();
            CreateMap<GetUserApproverByIdResponse, UserApprover>().ReverseMap();

            CreateMap<GetAllUserApproversCachedResponse, UserApprover>().ReverseMap();
            CreateMap<GetAllUserApproversResponse, UserApprover>().ReverseMap();
        }
    }
}

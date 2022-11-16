using EDocSys.Web.Areas.Admin.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace EDocSys.Web.Areas.Admin.Mappings
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {
            CreateMap<IdentityRole, UserRolesViewModel>().ReverseMap();
        }
    }
}
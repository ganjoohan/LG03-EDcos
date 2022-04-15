using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Web.Areas.Admin.Models;
using AutoMapper;

namespace EDocSys.Web.Areas.Admin.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
        }
    }
}
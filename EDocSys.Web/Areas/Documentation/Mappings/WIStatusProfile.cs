using EDocSys.Application.Features.WIStatuses.Commands.Create;
using EDocSys.Application.Features.WIStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class WIStatusProfile : Profile
    {
        public WIStatusProfile()
        {
            CreateMap<CreateWIStatusCommand, WIStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllWIStatusCachedResponse>();

            CreateMap<GetAllWIStatusCachedResponse, WIStatusViewModel>().ReverseMap();
        }
    }
}

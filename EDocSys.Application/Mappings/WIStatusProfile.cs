using EDocSys.Application.Features.WIStatuses.Commands.Create;
using EDocSys.Application.Features.WIStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.WIStatuses.Queries.GetAllPaged;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class WIStatusProfile : Profile
    {
        public WIStatusProfile()
        {
            CreateMap<CreateWIStatusCommand, WIStatus>().ReverseMap();
            // CreateMap<GetWIStatusByIdResponse, WIStatus>().ReverseMap();
            CreateMap<GetAllWIStatusCachedResponse, WIStatus>().ReverseMap();
            CreateMap<GetAllWIStatusResponse, WIStatus>().ReverseMap();
        }
    }
}

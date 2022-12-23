using EDocSys.Application.Features.IssuanceStatuses.Commands.Create;
using EDocSys.Application.Features.IssuanceStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.IssuanceStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.IssuanceStatuses.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class IssuanceStatusProfile : Profile
    {
        public IssuanceStatusProfile()
        {
            CreateMap<CreateIssuanceStatusCommand, IssuanceStatus>().ReverseMap();
            // CreateMap<GetIssuanceStatusByIdResponse, IssuanceStatus>().ReverseMap();
            CreateMap<GetAllIssuanceStatusCachedResponse, IssuanceStatus>().ReverseMap();
            CreateMap<GetAllIssuanceStatusResponse, IssuanceStatus>().ReverseMap();
        }
    }
}

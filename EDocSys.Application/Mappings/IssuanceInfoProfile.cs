using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetAllPaged;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class IssuanceInfoProfile : Profile
    {
        public IssuanceInfoProfile()
        {
            CreateMap<CreateIssuanceInfoCommand, IssuanceInfo>().ReverseMap();
            CreateMap<GetIssuanceInfoByHIdResponse, IssuanceInfo>().ReverseMap();

            CreateMap<GetAllIssuancesInfoCachedResponse, IssuanceInfo>().ReverseMap();
            CreateMap<GetAllIssuancesInfoResponse, IssuanceInfo>().ReverseMap();
        }
    }
}
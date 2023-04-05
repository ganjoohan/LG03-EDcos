using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetAllPaged;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class IssuanceInfoPrintProfile : Profile
    {
        public IssuanceInfoPrintProfile()
        {
            CreateMap<CreateIssuanceInfoPrintCommand, IssuanceInfoPrint>().ReverseMap();
            CreateMap<GetIssuanceInfoPrintByHIdResponse, IssuanceInfoPrint>().ReverseMap();

            CreateMap<GetAllIssuancesInfoPrintCachedResponse, IssuanceInfoPrint>().ReverseMap();
            CreateMap<GetAllIssuancesInfoPrintResponse, IssuanceInfoPrint>().ReverseMap();
        }
    }
}
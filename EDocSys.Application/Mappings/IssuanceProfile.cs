using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetAllPaged;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class IssuanceProfile : Profile
    {
        public IssuanceProfile()
        {
            CreateMap<CreateIssuanceCommand, Issuance>().ReverseMap();
            CreateMap<GetIssuanceByIdResponse, Issuance>().ReverseMap();

            CreateMap<GetAllIssuancesCachedResponse, Issuance>().ReverseMap();
            CreateMap<GetAllIssuancesResponse, Issuance>().ReverseMap();
        }
    }
}
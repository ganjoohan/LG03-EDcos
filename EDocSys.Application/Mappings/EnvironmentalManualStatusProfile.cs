using EDocSys.Application.Features.EnvironmentalManualStatuses.Commands.Create;
using EDocSys.Application.Features.EnvironmentalManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.EnvironmentalManualStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.EnvironmentalManualStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class EnvironmentalManualStatusProfile : Profile
    {
        public EnvironmentalManualStatusProfile()
        {
            CreateMap<CreateEnvironmentalManualStatusCommand, EnvironmentalManualStatus>().ReverseMap();
            // CreateMap<GetEnvironmentalManualStatusByIdResponse, EnvironmentalManualStatus>().ReverseMap();
            CreateMap<GetAllEnvironmentalManualStatusCachedResponse, EnvironmentalManualStatus>().ReverseMap();
            CreateMap<GetAllEnvironmentalManualStatusResponse, EnvironmentalManualStatus>().ReverseMap();
        }
    }
}

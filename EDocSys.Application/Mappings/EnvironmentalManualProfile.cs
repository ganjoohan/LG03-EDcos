using EDocSys.Application.Features.EnvironmentalManuals.Commands.Create;
using EDocSys.Application.Features.EnvironmentalManuals.Queries.GetAllCached;
using EDocSys.Application.Features.EnvironmentalManuals.Queries.GetAllPaged;
using EDocSys.Application.Features.EnvironmentalManuals.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class EnvironmentalManualProfile : Profile
    {
        public EnvironmentalManualProfile()
        {
            CreateMap<CreateEnvironmentalManualCommand, EnvironmentalManual>().ReverseMap();
            CreateMap<GetEnvironmentalManualByIdResponse, EnvironmentalManual>().ReverseMap();

            CreateMap<GetAllEnvironmentalManualsCachedResponse, EnvironmentalManual>().ReverseMap();
            CreateMap<GetAllEnvironmentalManualsResponse, EnvironmentalManual>().ReverseMap();
        }
    }
}
using EDocSys.Application.Features.SafetyHealthManuals.Commands.Create;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllPaged;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class SafetyHealthManualProfile : Profile
    {
        public SafetyHealthManualProfile()
        {
            CreateMap<CreateSafetyHealthManualCommand, SafetyHealthManual>().ReverseMap();
            CreateMap<GetSafetyHealthManualByIdResponse, SafetyHealthManual>().ReverseMap();

            CreateMap<GetAllSafetyHealthManualsCachedResponse, SafetyHealthManual>().ReverseMap();
            CreateMap<GetAllSafetyHealthManualsResponse, SafetyHealthManual>().ReverseMap();
        }
    }
}
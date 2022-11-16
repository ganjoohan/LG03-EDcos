using EDocSys.Application.Features.SafetyHealthManualStatuses.Commands.Create;
using EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.SafetyHealthManualStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class SafetyHealthManualStatusProfile : Profile
    {
        public SafetyHealthManualStatusProfile()
        {
            CreateMap<CreateSafetyHealthManualStatusCommand, SafetyHealthManualStatus>().ReverseMap();
            // CreateMap<GetSafetyHealthManualStatusByIdResponse, SafetyHealthManualStatus>().ReverseMap();
            CreateMap<GetAllSafetyHealthManualStatusCachedResponse, SafetyHealthManualStatus>().ReverseMap();
            CreateMap<GetAllSafetyHealthManualStatusResponse, SafetyHealthManualStatus>().ReverseMap();
        }
    }
}

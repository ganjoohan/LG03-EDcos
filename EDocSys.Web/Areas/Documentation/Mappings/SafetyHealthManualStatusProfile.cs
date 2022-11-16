using EDocSys.Application.Features.SafetyHealthManualStatuses.Commands.Create;
using EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class SafetyHealthManualStatusProfile : Profile
    {
        public SafetyHealthManualStatusProfile()
        {
            CreateMap<CreateSafetyHealthManualStatusCommand, SafetyHealthManualStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllSafetyHealthManualStatusCachedResponse>();

            CreateMap<GetAllSafetyHealthManualStatusCachedResponse, SafetyHealthManualStatusViewModel>().ReverseMap();
        }
    }
}

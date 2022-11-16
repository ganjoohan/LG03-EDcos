using EDocSys.Application.Features.EnvironmentalManualStatuses.Commands.Create;
using EDocSys.Application.Features.EnvironmentalManualStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class EnvironmentalManualStatusProfile : Profile
    {
        public EnvironmentalManualStatusProfile()
        {
            CreateMap<CreateEnvironmentalManualStatusCommand, EnvironmentalManualStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllEnvironmentalManualStatusCachedResponse>();

            CreateMap<GetAllEnvironmentalManualStatusCachedResponse, EnvironmentalManualStatusViewModel>().ReverseMap();
        }
    }
}

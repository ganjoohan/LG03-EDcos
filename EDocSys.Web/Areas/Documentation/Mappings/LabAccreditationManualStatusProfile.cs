using EDocSys.Application.Features.LabAccreditationManualStatuses.Commands.Create;
using EDocSys.Application.Features.LabAccreditationManualStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class LabAccreditationManualStatusProfile : Profile
    {
        public LabAccreditationManualStatusProfile()
        {
            CreateMap<CreateLabAccreditationManualStatusCommand, LabAccreditationManualStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllLabAccreditationManualStatusCachedResponse>();

            CreateMap<GetAllLabAccreditationManualStatusCachedResponse, LabAccreditationManualStatusViewModel>().ReverseMap();
        }
    }
}

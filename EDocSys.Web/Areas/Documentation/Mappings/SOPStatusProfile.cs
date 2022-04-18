using EDocSys.Application.Features.SOPStatuses.Commands.Create;
using EDocSys.Application.Features.SOPStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class SOPStatusProfile : Profile
    {
        public SOPStatusProfile()
        {
            CreateMap<CreateSOPStatusCommand, SOPStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllSOPStatusCachedResponse>();

            CreateMap<GetAllSOPStatusCachedResponse, SOPStatusViewModel>().ReverseMap();
        }
    }
}

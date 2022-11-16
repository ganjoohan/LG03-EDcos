using EDocSys.Application.Features.QualityManualStatuses.Commands.Create;
using EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class QualityManualStatusProfile : Profile
    {
        public QualityManualStatusProfile()
        {
            CreateMap<CreateQualityManualStatusCommand, QualityManualStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllQualityManualStatusCachedResponse>();

            CreateMap<GetAllQualityManualStatusCachedResponse, QualityManualStatusViewModel>().ReverseMap();
        }
    }
}

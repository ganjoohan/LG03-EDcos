using EDocSys.Application.Features.QualityManualStatuses.Commands.Create;
using EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.QualityManualStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class QualityManualStatusProfile : Profile
    {
        public QualityManualStatusProfile()
        {
            CreateMap<CreateQualityManualStatusCommand, QualityManualStatus>().ReverseMap();
            // CreateMap<GetQualityManualStatusByIdResponse, QualityManualStatus>().ReverseMap();
            CreateMap<GetAllQualityManualStatusCachedResponse, QualityManualStatus>().ReverseMap();
            CreateMap<GetAllQualityManualStatusResponse, QualityManualStatus>().ReverseMap();
        }
    }
}

using EDocSys.Application.Features.QualityManuals.Commands.Create;
using EDocSys.Application.Features.QualityManuals.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManuals.Queries.GetAllPaged;
using EDocSys.Application.Features.QualityManuals.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class QualityManualProfile : Profile
    {
        public QualityManualProfile()
        {
            CreateMap<CreateQualityManualCommand, QualityManual>().ReverseMap();
            CreateMap<GetQualityManualByIdResponse, QualityManual>().ReverseMap();

            CreateMap<GetAllQualityManualsCachedResponse, QualityManual>().ReverseMap();
            CreateMap<GetAllQualityManualsResponse, QualityManual>().ReverseMap();
        }
    }
}
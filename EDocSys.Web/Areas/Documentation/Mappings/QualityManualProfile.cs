using EDocSys.Application.Features.QualityManuals.Commands.Create;
using EDocSys.Application.Features.QualityManuals.Commands.Update;
using EDocSys.Application.Features.QualityManuals.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManuals.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class QualityManualProfile : Profile
    {
        public QualityManualProfile()
        {
            CreateMap<QualityManual, GetAllQualityManualsCachedResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllQualityManualsCachedResponse, QualityManualViewModel>().ReverseMap();


            CreateMap<QualityManual, GetQualityManualByIdResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetQualityManualByIdResponse, QualityManualViewModel>().ReverseMap();
            CreateMap<CreateQualityManualCommand, QualityManualViewModel>().ReverseMap();
            CreateMap<UpdateQualityManualCommand, QualityManualViewModel>().ReverseMap();

        }
    }
}
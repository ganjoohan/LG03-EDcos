using EDocSys.Application.Features.DocumentManuals.Commands.Create;
using EDocSys.Application.Features.DocumentManuals.Commands.Update;
using EDocSys.Application.Features.DocumentManuals.Queries.GetAllCached;
using EDocSys.Application.Features.DocumentManuals.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class DocumentManualProfile : Profile
    {
        public DocumentManualProfile()
        {
            CreateMap<DocumentManual, GetAllDocumentManualsCachedResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllDocumentManualsCachedResponse, DocumentManualViewModel>().ReverseMap();


            CreateMap<DocumentManual, GetDocumentManualByIdResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetDocumentManualByIdResponse, DocumentManualViewModel>().ReverseMap();
            CreateMap<CreateDocumentManualCommand, DocumentManualViewModel>().ReverseMap();
            CreateMap<UpdateDocumentManualCommand, DocumentManualViewModel>().ReverseMap();

        }
    }
}
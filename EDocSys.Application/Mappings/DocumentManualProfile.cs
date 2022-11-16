using EDocSys.Application.Features.DocumentManuals.Commands.Create;
using EDocSys.Application.Features.DocumentManuals.Queries.GetAllCached;
using EDocSys.Application.Features.DocumentManuals.Queries.GetAllPaged;
using EDocSys.Application.Features.DocumentManuals.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class DocumentManualProfile : Profile
    {
        public DocumentManualProfile()
        {
            CreateMap<CreateDocumentManualCommand, DocumentManual>().ReverseMap();
            CreateMap<GetDocumentManualByIdResponse, DocumentManual>().ReverseMap();

            CreateMap<GetAllDocumentManualsCachedResponse, DocumentManual>().ReverseMap();
            CreateMap<GetAllDocumentManualsResponse, DocumentManual>().ReverseMap();
        }
    }
}
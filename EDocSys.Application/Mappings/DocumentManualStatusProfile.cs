using EDocSys.Application.Features.DocumentManualStatuses.Commands.Create;
using EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.DocumentManualStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class DocumentManualStatusProfile : Profile
    {
        public DocumentManualStatusProfile()
        {
            CreateMap<CreateDocumentManualStatusCommand, DocumentManualStatus>().ReverseMap();
            // CreateMap<GetDocumentManualStatusByIdResponse, DocumentManualStatus>().ReverseMap();
            CreateMap<GetAllDocumentManualStatusCachedResponse, DocumentManualStatus>().ReverseMap();
            CreateMap<GetAllDocumentManualStatusResponse, DocumentManualStatus>().ReverseMap();
        }
    }
}

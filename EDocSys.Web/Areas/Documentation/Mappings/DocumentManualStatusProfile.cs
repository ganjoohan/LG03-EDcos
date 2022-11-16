using EDocSys.Application.Features.DocumentManualStatuses.Commands.Create;
using EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class DocumentManualStatusProfile : Profile
    {
        public DocumentManualStatusProfile()
        {
            CreateMap<CreateDocumentManualStatusCommand, DocumentManualStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllDocumentManualStatusCachedResponse>();

            CreateMap<GetAllDocumentManualStatusCachedResponse, DocumentManualStatusViewModel>().ReverseMap();
        }
    }
}

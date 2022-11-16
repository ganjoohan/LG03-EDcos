using EDocSys.Application.Features.ExternalFeatures.Attachments.Commands.Create;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetAllCached;
//using EDocSys.Application.Features.Attachments.Queries.GetAllPaged;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetById;
using EDocSys.Domain.Entities.ExternalRecord;
using AutoMapper;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetByDocId;

namespace EDocSys.Application.Mappings.ExternalProfile
{
    internal class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<CreateAttachmentCommand, Attachment>().ReverseMap();
            CreateMap<GetAttachmentByIdResponse, Attachment>().ReverseMap();
            CreateMap<GetAttachmentByDocIdResponse, Attachment>().ReverseMap();

            CreateMap<GetAllAttachmentsCachedResponse, Attachment>().ReverseMap();
            //CreateMap<GetAllAttachmentsResponse, Attachment>().ReverseMap();
        }
    }
}
using EDocSys.Application.Features.QualityFeatures.Attachments.Commands.Create;
using EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetAllCached;
//using EDocSys.Application.Features.Attachments.Queries.GetAllPaged;
using EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetById;
using EDocSys.Domain.Entities.QualityRecord;
using AutoMapper;
using EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetByDocId;

namespace EDocSys.Application.Mappings.QualityProfile
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
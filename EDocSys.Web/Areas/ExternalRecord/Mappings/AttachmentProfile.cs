using EDocSys.Application.Features.ExternalFeatures.Attachments.Commands.Create;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Commands.Update;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetAllCached;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetById;
using EDocSys.Web.Areas.ExternalRecord.Models;
using AutoMapper;
using EDocSys.Domain.Entities.ExternalRecord;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetByDocId;
using System.Collections.Generic;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<Attachment, GetAllAttachmentsCachedResponse>();
                //.ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));
            CreateMap<GetAllAttachmentsCachedResponse, AttachmentViewModel>().ReverseMap();

            CreateMap<Attachment, GetAttachmentByIdResponse>();
            CreateMap<GetAttachmentByIdResponse, AttachmentViewModel>().ReverseMap();
            CreateMap<Attachment, GetAttachmentByDocIdResponse>();
            CreateMap<GetAttachmentByDocIdResponse, AttachmentViewModel>().ReverseMap();
            CreateMap<CreateAttachmentCommand, AttachmentViewModel>().ReverseMap();
            CreateMap<UpdateAttachmentCommand, AttachmentViewModel>().ReverseMap();

        }
    }
}
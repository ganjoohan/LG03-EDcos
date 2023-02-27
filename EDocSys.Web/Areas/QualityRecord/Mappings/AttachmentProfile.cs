using EDocSys.Application.Features.QualityFeatures.Attachments.Commands.Create;
using EDocSys.Application.Features.QualityFeatures.Attachments.Commands.Update;
using EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetAllCached;
using EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetById;
using EDocSys.Web.Areas.QualityRecord.Models;
using AutoMapper;
using EDocSys.Domain.Entities.QualityRecord;
using EDocSys.Application.Features.QualityFeatures.Attachments.Queries.GetByDocId;
using System.Collections.Generic;

namespace EDocSys.Web.Areas.QualityRecord.Mappings
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
using EDocSys.Application.Features.LabAccreditationManuals.Commands.Create;
using EDocSys.Application.Features.LabAccreditationManuals.Commands.Update;
using EDocSys.Application.Features.LabAccreditationManuals.Queries.GetAllCached;
using EDocSys.Application.Features.LabAccreditationManuals.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class LabAccreditationManualProfile : Profile
    {
        public LabAccreditationManualProfile()
        {
            CreateMap<LabAccreditationManual, GetAllLabAccreditationManualsCachedResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllLabAccreditationManualsCachedResponse, LabAccreditationManualViewModel>().ReverseMap();


            CreateMap<LabAccreditationManual, GetLabAccreditationManualByIdResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetLabAccreditationManualByIdResponse, LabAccreditationManualViewModel>().ReverseMap();
            CreateMap<CreateLabAccreditationManualCommand, LabAccreditationManualViewModel>().ReverseMap();
            CreateMap<UpdateLabAccreditationManualCommand, LabAccreditationManualViewModel>().ReverseMap();

        }
    }
}
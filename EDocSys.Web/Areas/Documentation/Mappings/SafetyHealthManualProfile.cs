using EDocSys.Application.Features.SafetyHealthManuals.Commands.Create;
using EDocSys.Application.Features.SafetyHealthManuals.Commands.Update;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class SafetyHealthManualProfile : Profile
    {
        public SafetyHealthManualProfile()
        {
            CreateMap<SafetyHealthManual, GetAllSafetyHealthManualsCachedResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllSafetyHealthManualsCachedResponse, SafetyHealthManualViewModel>().ReverseMap();


            CreateMap<SafetyHealthManual, GetSafetyHealthManualByIdResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetSafetyHealthManualByIdResponse, SafetyHealthManualViewModel>().ReverseMap();
            CreateMap<CreateSafetyHealthManualCommand, SafetyHealthManualViewModel>().ReverseMap();
            CreateMap<UpdateSafetyHealthManualCommand, SafetyHealthManualViewModel>().ReverseMap();

        }
    }
}
using EDocSys.Application.Features.EnvironmentalManuals.Commands.Create;
using EDocSys.Application.Features.EnvironmentalManuals.Commands.Update;
using EDocSys.Application.Features.EnvironmentalManuals.Queries.GetAllCached;
using EDocSys.Application.Features.EnvironmentalManuals.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class EnvironmentalManualProfile : Profile
    {
        public EnvironmentalManualProfile()
        {
            CreateMap<EnvironmentalManual, GetAllEnvironmentalManualsCachedResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllEnvironmentalManualsCachedResponse, EnvironmentalManualViewModel>().ReverseMap();


            CreateMap<EnvironmentalManual, GetEnvironmentalManualByIdResponse>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetEnvironmentalManualByIdResponse, EnvironmentalManualViewModel>().ReverseMap();
            CreateMap<CreateEnvironmentalManualCommand, EnvironmentalManualViewModel>().ReverseMap();
            CreateMap<UpdateEnvironmentalManualCommand, EnvironmentalManualViewModel>().ReverseMap();

        }
    }
}
using EDocSys.Application.Features.WIs.Commands.Create;
using EDocSys.Application.Features.WIs.Commands.Update;
using EDocSys.Application.Features.WIs.Queries.GetAllCached;
using EDocSys.Application.Features.WIs.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Application.Features.WIs.Queries.GetByParameter;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class WIProfile : Profile
    {
        public WIProfile()
        {
            CreateMap<WI, GetAllWIsCachedResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            
                

            CreateMap<WI, GetWIByIdResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));



            CreateMap<GetAllWIsCachedResponse, WIViewModel>().ReverseMap();

            CreateMap<WI, GetWIsByParameterResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetWIsByParameterResponse, WIViewModel>().ReverseMap();



            CreateMap<GetWIByIdResponse, WIViewModel>().ReverseMap();
            CreateMap<CreateWICommand, WIViewModel>().ReverseMap();
            CreateMap<UpdateWICommand, WIViewModel>().ReverseMap();

        }
    }
}
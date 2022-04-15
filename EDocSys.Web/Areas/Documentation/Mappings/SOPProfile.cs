using EDocSys.Application.Features.SOPs.Commands.Create;
using EDocSys.Application.Features.SOPs.Commands.Update;
using EDocSys.Application.Features.SOPs.Queries.GetAllCached;
using EDocSys.Application.Features.SOPs.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class SOPProfile : Profile
    {
        public SOPProfile()
        {
            CreateMap<SOP, GetAllSOPsCachedResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            
                

            CreateMap<SOP, GetSOPByIdResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));



            CreateMap<GetAllSOPsCachedResponse, SOPViewModel>().ReverseMap();


            


            CreateMap<GetSOPByIdResponse, SOPViewModel>().ReverseMap();
            CreateMap<CreateSOPCommand, SOPViewModel>().ReverseMap();
            CreateMap<UpdateSOPCommand, SOPViewModel>().ReverseMap();

        }
    }
}
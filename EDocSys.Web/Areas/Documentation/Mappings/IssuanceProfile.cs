using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Commands.Update;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class IssuanceProfile : Profile
    {
        public IssuanceProfile()
        {
            CreateMap<Issuance, GetAllIssuancesCachedResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllIssuancesCachedResponse, IssuanceViewModel>().ReverseMap();


            CreateMap<Issuance, GetIssuanceByIdResponse>()
               .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetIssuanceByIdResponse, IssuanceViewModel>().ReverseMap();
            CreateMap<CreateIssuanceCommand, IssuanceViewModel>().ReverseMap();
            CreateMap<UpdateIssuanceCommand, IssuanceViewModel>().ReverseMap();

        }
    }
}
using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Commands.Update;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class IssuanceInfoProfile : Profile
    {
        public IssuanceInfoProfile()
        {
            CreateMap<Issuance, GetAllIssuancesInfoCachedResponse>();
                //.ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllIssuancesInfoCachedResponse, IssuanceInfoViewModel>().ReverseMap();


            //CreateMap<IssuanceInfo, GetIssuanceInfoByIdResponse>()
            //    .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetIssuanceInfoByIHdResponse, IssuanceInfoViewModel>().ReverseMap();
            CreateMap<CreateIssuanceInfoCommand, IssuanceInfoViewModel>().ReverseMap();
            CreateMap<UpdateIssuanceInfoCommand, IssuanceInfoViewModel>().ReverseMap();

        }
    }
}
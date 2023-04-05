using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Commands.Update;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class IssuanceInfoPrintProfile : Profile
    {
        public IssuanceInfoPrintProfile()
        {
            CreateMap<IssuanceInfoPrint, GetAllIssuancesInfoPrintCachedResponse>();
                //.ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllIssuancesInfoPrintCachedResponse, IssuanceInfoPrintViewModel>().ReverseMap();


            //CreateMap<IssuanceInfo, GetIssuanceInfoByIdResponse>()
            //    .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetIssuanceInfoPrintByHIdResponse, IssuanceInfoPrintViewModel>().ReverseMap();
            CreateMap<CreateIssuanceInfoPrintCommand, IssuanceInfoPrintViewModel>().ReverseMap();
            CreateMap<UpdateIssuanceInfoPrintCommand, IssuanceInfoPrintViewModel>().ReverseMap();

        }
    }
}
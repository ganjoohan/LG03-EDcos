using EDocSys.Application.Features.Companies.Commands.Create;
using EDocSys.Application.Features.Companies.Commands.Update;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;

namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<GetAllCompaniesCachedResponse, CompanyViewModel>().ReverseMap();
            CreateMap<GetCompanyByIdResponse, CompanyViewModel>().ReverseMap();
            CreateMap<CreateCompanyCommand, CompanyViewModel>().ReverseMap();
            CreateMap<UpdateCompanyCommand, CompanyViewModel>().ReverseMap();
        }
        
    }
}

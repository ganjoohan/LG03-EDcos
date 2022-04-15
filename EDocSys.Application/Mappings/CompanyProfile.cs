using EDocSys.Application.Features.Companies.Commands.Create;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;
using EDocSys.Domain.Entities.DocumentationMaster;

namespace EDocSys.Application.Mappings
{
    internal class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CreateCompanyCommand, Company>().ReverseMap();
            CreateMap<GetCompanyByIdResponse, Company>().ReverseMap();
            CreateMap<GetAllCompaniesCachedResponse, Company>().ReverseMap();
        }
    }
}
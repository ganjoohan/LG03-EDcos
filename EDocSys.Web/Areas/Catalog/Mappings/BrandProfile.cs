using EDocSys.Application.Features.Brands.Commands.Create;
using EDocSys.Application.Features.Brands.Commands.Update;
using EDocSys.Application.Features.Brands.Queries.GetAllCached;
using EDocSys.Application.Features.Brands.Queries.GetById;
using EDocSys.Web.Areas.Catalog.Models;
using AutoMapper;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<GetAllBrandsCachedResponse, BrandViewModel>().ReverseMap();
            CreateMap<GetBrandByIdResponse, BrandViewModel>().ReverseMap();
            CreateMap<CreateBrandCommand, BrandViewModel>().ReverseMap();
            CreateMap<UpdateBrandCommand, BrandViewModel>().ReverseMap();
        }
    }
}
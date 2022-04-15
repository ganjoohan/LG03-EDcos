using EDocSys.Application.Features.Brands.Commands.Create;
using EDocSys.Application.Features.Brands.Queries.GetAllCached;
using EDocSys.Application.Features.Brands.Queries.GetById;
using EDocSys.Domain.Entities.Catalog;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<CreateBrandCommand, Brand>().ReverseMap();
            CreateMap<GetBrandByIdResponse, Brand>().ReverseMap();
            CreateMap<GetAllBrandsCachedResponse, Brand>().ReverseMap();
        }
    }
}
using EDocSys.Application.Features.Products.Commands.Create;
using EDocSys.Application.Features.Products.Queries.GetAllCached;
using EDocSys.Application.Features.Products.Queries.GetAllPaged;
using EDocSys.Application.Features.Products.Queries.GetById;
using EDocSys.Domain.Entities.Catalog;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductCommand, Product>().ReverseMap();
            CreateMap<GetProductByIdResponse, Product>().ReverseMap();
            CreateMap<GetAllProductsCachedResponse, Product>().ReverseMap();
            CreateMap<GetAllProductsResponse, Product>().ReverseMap();
        }
    }
}
using EDocSys.Application.Features.Products.Commands.Create;
using EDocSys.Application.Features.Products.Commands.Update;
using EDocSys.Application.Features.Products.Queries.GetAllCached;
using EDocSys.Application.Features.Products.Queries.GetById;
using EDocSys.Web.Areas.Catalog.Models;
using AutoMapper;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<GetAllProductsCachedResponse, ProductViewModel>().ReverseMap();
            CreateMap<GetProductByIdResponse, ProductViewModel>().ReverseMap();
            CreateMap<CreateProductCommand, ProductViewModel>().ReverseMap();
            CreateMap<UpdateProductCommand, ProductViewModel>().ReverseMap();
        }
    }
}
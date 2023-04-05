using EDocSys.Application.Features.WIs.Commands.Create;
using EDocSys.Application.Features.WIs.Queries.GetAllCached;
using EDocSys.Application.Features.WIs.Queries.GetAllPaged;
using EDocSys.Application.Features.WIs.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;
using EDocSys.Application.Features.WIs.Queries.GetByParameter;

namespace EDocSys.Application.Mappings
{
    internal class WIProfile : Profile
    {
        public WIProfile()
        {
            CreateMap<CreateWICommand, WI>().ReverseMap();
            CreateMap<GetWIByIdResponse, WI>().ReverseMap();
            CreateMap<GetAllWIsCachedResponse, WI>().ReverseMap();
            CreateMap<GetAllWIsResponse, WI>().ReverseMap();

            CreateMap<GetWIsByParameterResponse, WI>().ReverseMap();
        }
    }
}
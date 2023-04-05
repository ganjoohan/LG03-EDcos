using EDocSys.Application.Features.SOPs.Commands.Create;
using EDocSys.Application.Features.SOPs.Queries.GetAllCached;
using EDocSys.Application.Features.SOPs.Queries.GetAllPaged;
using EDocSys.Application.Features.SOPs.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;
using EDocSys.Application.Features.SOPs.Queries.GetByParameter;

namespace EDocSys.Application.Mappings
{
    internal class SOPProfile : Profile
    {
        public SOPProfile()
        {
            CreateMap<CreateSOPCommand, SOP>().ReverseMap();
            CreateMap<GetSOPByIdResponse, SOP>().ReverseMap();
            CreateMap<GetAllSOPsCachedResponse, SOP>().ReverseMap();
            CreateMap<GetAllSOPsResponse, SOP>().ReverseMap();

            CreateMap<GetSOPsByParameterResponse, SOP>().ReverseMap();
        }
    }
}
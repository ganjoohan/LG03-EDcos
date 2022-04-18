using EDocSys.Application.Features.SOPStatuses.Commands.Create;
using EDocSys.Application.Features.SOPStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.SOPStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.ProcedureStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class SOPStatusProfile : Profile
    {
        public SOPStatusProfile()
        {
            CreateMap<CreateSOPStatusCommand, SOPStatus>().ReverseMap();
            // CreateMap<GetSOPStatusByIdResponse, SOPStatus>().ReverseMap();
            CreateMap<GetAllSOPStatusCachedResponse, SOPStatus>().ReverseMap();
            CreateMap<GetAllSOPStatusResponse, SOPStatus>().ReverseMap();
        }
    }
}

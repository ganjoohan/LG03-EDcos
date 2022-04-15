using EDocSys.Application.Features.ProcedureStatuses.Commands.Create;
using EDocSys.Application.Features.ProcedureStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.ProcedureStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.ProcedureStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class ProcedureStatusProfile : Profile
    {
        public ProcedureStatusProfile()
        {
            CreateMap<CreateProcedureStatusCommand, ProcedureStatus>().ReverseMap();
            // CreateMap<GetProcedureStatusByIdResponse, ProcedureStatus>().ReverseMap();
            CreateMap<GetAllProcedureStatusCachedResponse, ProcedureStatus>().ReverseMap();
            CreateMap<GetAllProcedureStatusResponse, ProcedureStatus>().ReverseMap();
        }
    }
}

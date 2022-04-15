using EDocSys.Application.Features.ProcedureStatuses.Commands.Create;
using EDocSys.Application.Features.ProcedureStatuses.Queries.GetAllCached;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;


namespace EDocSys.Web.Areas.Documentation.Mappings
{
    internal class ProcedureStatusProfile : Profile
    {
        public ProcedureStatusProfile()
        {
            CreateMap<CreateProcedureStatusCommand, ProcedureStatusViewModel>().ReverseMap();

            CreateMap<SOP, GetAllProcedureStatusCachedResponse>();

            CreateMap<GetAllProcedureStatusCachedResponse, ProcedureStatusViewModel>().ReverseMap();
        }
    }
}

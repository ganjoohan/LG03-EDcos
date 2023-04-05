using EDocSys.Application.Features.Procedures.Commands.Create;
using EDocSys.Application.Features.Procedures.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Queries.GetAllPaged;
using EDocSys.Application.Features.Procedures.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;
using EDocSys.Application.Features.Procedures.Queries.GetByParameter;

namespace EDocSys.Application.Mappings
{
    internal class ProcedureProfile : Profile
    {
        public ProcedureProfile()
        {
            CreateMap<CreateProcedureCommand, Procedure>().ReverseMap();
            CreateMap<GetProcedureByIdResponse, Procedure>().ReverseMap();

            CreateMap<GetAllProceduresCachedResponse, Procedure>().ReverseMap();
            CreateMap<GetAllProceduresResponse, Procedure>().ReverseMap();

            CreateMap<GetProceduresByParameterResponse, Procedure>().ReverseMap();
        }
    }
}
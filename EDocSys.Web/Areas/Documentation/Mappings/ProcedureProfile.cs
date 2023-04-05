using EDocSys.Application.Features.Procedures.Commands.Create;
using EDocSys.Application.Features.Procedures.Commands.Update;
using EDocSys.Application.Features.Procedures.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Queries.GetById;
using EDocSys.Web.Areas.Documentation.Models;
using AutoMapper;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Application.Features.Procedures.Queries.GetByParameter;

namespace EDocSys.Web.Areas.Catalog.Mappings
{
    internal class ProcedureProfile : Profile
    {
        public ProcedureProfile()
        {
            CreateMap<Procedure, GetAllProceduresCachedResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllProceduresCachedResponse, ProcedureViewModel>().ReverseMap();

            CreateMap<Procedure, GetProcedureByIdResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetProcedureByIdResponse, ProcedureViewModel>().ReverseMap();
            CreateMap<Procedure, GetProceduresByParameterResponse>()
                .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.Department.Name))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetProceduresByParameterResponse, ProcedureViewModel>().ReverseMap();

            CreateMap<CreateProcedureCommand, ProcedureViewModel>().ReverseMap();
            CreateMap<UpdateProcedureCommand, ProcedureViewModel>().ReverseMap();

        }
    }
}
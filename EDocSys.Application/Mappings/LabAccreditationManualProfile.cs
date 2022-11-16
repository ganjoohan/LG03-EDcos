using EDocSys.Application.Features.LabAccreditationManuals.Commands.Create;
using EDocSys.Application.Features.LabAccreditationManuals.Queries.GetAllCached;
using EDocSys.Application.Features.LabAccreditationManuals.Queries.GetAllPaged;
using EDocSys.Application.Features.LabAccreditationManuals.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;

namespace EDocSys.Application.Mappings
{
    internal class LabAccreditationManualProfile : Profile
    {
        public LabAccreditationManualProfile()
        {
            CreateMap<CreateLabAccreditationManualCommand, LabAccreditationManual>().ReverseMap();
            CreateMap<GetLabAccreditationManualByIdResponse, LabAccreditationManual>().ReverseMap();

            CreateMap<GetAllLabAccreditationManualsCachedResponse, LabAccreditationManual>().ReverseMap();
            CreateMap<GetAllLabAccreditationManualsResponse, LabAccreditationManual>().ReverseMap();
        }
    }
}
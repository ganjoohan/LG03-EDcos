using EDocSys.Application.Features.LabAccreditationManualStatuses.Commands.Create;
using EDocSys.Application.Features.LabAccreditationManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.LabAccreditationManualStatuses.Queries.GetAllPaged;
//using EDocSys.Application.Features.LabAccreditationManualStatus.Queries.GetById;
using EDocSys.Domain.Entities.Documentation;
using AutoMapper;


namespace EDocSys.Application.Mappings
{
    internal class LabAccreditationManualStatusProfile : Profile
    {
        public LabAccreditationManualStatusProfile()
        {
            CreateMap<CreateLabAccreditationManualStatusCommand, LabAccreditationManualStatus>().ReverseMap();
            // CreateMap<GetLabAccreditationManualStatusByIdResponse, LabAccreditationManualStatus>().ReverseMap();
            CreateMap<GetAllLabAccreditationManualStatusCachedResponse, LabAccreditationManualStatus>().ReverseMap();
            CreateMap<GetAllLabAccreditationManualStatusResponse, LabAccreditationManualStatus>().ReverseMap();
        }
    }
}

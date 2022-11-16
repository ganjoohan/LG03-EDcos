using EDocSys.Application.Features.ExternalFeatures.LionSteels.Commands.Create;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetAllCached;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetAllPaged;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetById;
using AutoMapper;
using EDocSys.Domain.Entities.ExternalRecord;

namespace EDocSys.Application.Mappings.ExternalProfile
{
    internal class LionSteelProfile : Profile
    {
        public LionSteelProfile()
        {
            CreateMap<CreateLionSteelCommand, LionSteel>().ReverseMap();
            CreateMap<GetLionSteelByIdResponse, LionSteel>().ReverseMap();

            CreateMap<GetAllLionSteelsCachedResponse, LionSteel>().ReverseMap();
            CreateMap<GetAllLionSteelsResponse, LionSteel>().ReverseMap();
        }
    }
}
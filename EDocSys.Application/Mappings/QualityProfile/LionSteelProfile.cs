using EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Create;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetAllCached;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetAllPaged;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetById;
using AutoMapper;
using EDocSys.Domain.Entities.QualityRecord;

namespace EDocSys.Application.Mappings.QualityProfile
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
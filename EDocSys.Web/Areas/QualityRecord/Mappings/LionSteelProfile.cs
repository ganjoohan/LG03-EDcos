using EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Create;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Update;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetAllCached;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetById;
using EDocSys.Web.Areas.QualityRecord.Models;
using AutoMapper;
using EDocSys.Domain.Entities.QualityRecord;
using EDocSys.Application.Features.Procedures.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Queries.GetById;
using EDocSys.Application.Features.Procedures.Queries.GetByParameter;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Web.Areas.Documentation.Models;

namespace EDocSys.Web.Areas.QualityRecord.Mappings
{
    internal class LionSteelProfile : Profile
    {
        public LionSteelProfile()
        {
            CreateMap<LionSteel, GetAllLionSteelsCachedResponse>();

            CreateMap<GetAllLionSteelsCachedResponse, LionSteelViewModel>().ReverseMap();


            CreateMap<LionSteel, GetLionSteelByIdResponse>();


            CreateMap<GetLionSteelByIdResponse, LionSteelViewModel>().ReverseMap();
            CreateMap<CreateLionSteelCommand, LionSteelViewModel>().ReverseMap();
            CreateMap<UpdateLionSteelCommand, LionSteelViewModel>().ReverseMap();

        }
    }
}
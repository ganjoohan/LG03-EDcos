using EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Create;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Update;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetAllCached;
using EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetById;
using EDocSys.Web.Areas.QualityRecord.Models;
using AutoMapper;
using EDocSys.Domain.Entities.QualityRecord;

namespace EDocSys.Web.Areas.QualityRecord.Mappings
{
    internal class LionSteelProfile : Profile
    {
        public LionSteelProfile()
        {
            CreateMap<LionSteel, GetAllLionSteelsCachedResponse>();
                //.ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<GetAllLionSteelsCachedResponse, LionSteelViewModel>().ReverseMap();


            CreateMap<LionSteel, GetLionSteelByIdResponse>();
                //.ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name));


            CreateMap<GetLionSteelByIdResponse, LionSteelViewModel>().ReverseMap();
            CreateMap<CreateLionSteelCommand, LionSteelViewModel>().ReverseMap();
            CreateMap<UpdateLionSteelCommand, LionSteelViewModel>().ReverseMap();

        }
    }
}
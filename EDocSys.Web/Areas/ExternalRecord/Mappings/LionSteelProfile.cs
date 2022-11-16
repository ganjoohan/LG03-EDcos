using EDocSys.Application.Features.ExternalFeatures.LionSteels.Commands.Create;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Commands.Update;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetAllCached;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetById;
using EDocSys.Web.Areas.ExternalRecord.Models;
using AutoMapper;
using EDocSys.Domain.Entities.ExternalRecord;

namespace EDocSys.Web.Areas.ExternalRecord.Mappings
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
using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;

namespace EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetByDOCPNo
{
    public class GetLionSteelByDOCNoQuery : IRequest<Result<GetLionSteelByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetLionSteelByDOCNoQueryHandler : IRequestHandler<GetLionSteelByDOCNoQuery, Result<GetLionSteelByDOCNoResponse>>
        {
            private readonly ILionSteelCacheRepository _lionSteelCache;
            private readonly IMapper _mapper;

            public GetLionSteelByDOCNoQueryHandler(ILionSteelCacheRepository lionSteelCache, IMapper mapper)
            {
                _lionSteelCache = lionSteelCache;
                _mapper = mapper;
            }

            public async Task<Result<GetLionSteelByDOCNoResponse>> Handle(GetLionSteelByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var lionSteel = await _lionSteelCache.GetByDOCNoAsync(query.docno);
                var mappedLionSteel = _mapper.Map<GetLionSteelByDOCNoResponse>(lionSteel);
                return Result<GetLionSteelByDOCNoResponse>.Success(mappedLionSteel);
            }
        }
    }
}
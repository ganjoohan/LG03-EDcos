using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;

namespace EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetAllCached
{
    public class GetAllLionSteelsCachedQuery : IRequest<Result<List<GetAllLionSteelsCachedResponse>>>
    {
        public GetAllLionSteelsCachedQuery()
        {
        }

        public class GetAllLionSteelsCachedQueryHandler : IRequestHandler<GetAllLionSteelsCachedQuery, Result<List<GetAllLionSteelsCachedResponse>>>
        {
            private readonly ILionSteelCacheRepository _lionSteelCache;
            private readonly IMapper _mapper;

            public GetAllLionSteelsCachedQueryHandler(ILionSteelCacheRepository lionSteelCache, IMapper mapper)
            {
                _lionSteelCache = lionSteelCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllLionSteelsCachedResponse>>> Handle(GetAllLionSteelsCachedQuery request, CancellationToken cancellationToken)
            {
                var lionSteelList = await _lionSteelCache.GetCachedListAsync();
                var mappedLionSteels = _mapper.Map<List<GetAllLionSteelsCachedResponse>>(lionSteelList);
                return Result<List<GetAllLionSteelsCachedResponse>>.Success(mappedLionSteels);
            }
        }
    }
}

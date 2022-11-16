using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllCached
{
    public class GetAllSafetyHealthManualsCachedQuery : IRequest<Result<List<GetAllSafetyHealthManualsCachedResponse>>>
    {
        public GetAllSafetyHealthManualsCachedQuery()
        {
        }

        public class GetAllSafetyHealthManualsCachedQueryHandler : IRequestHandler<GetAllSafetyHealthManualsCachedQuery, Result<List<GetAllSafetyHealthManualsCachedResponse>>>
        {
            private readonly ISafetyHealthManualCacheRepository _safetyHealthManualCache;
            private readonly IMapper _mapper;

            public GetAllSafetyHealthManualsCachedQueryHandler(ISafetyHealthManualCacheRepository safetyHealthManualCache, IMapper mapper)
            {
                _safetyHealthManualCache = safetyHealthManualCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllSafetyHealthManualsCachedResponse>>> Handle(GetAllSafetyHealthManualsCachedQuery request, CancellationToken cancellationToken)
            {
                var safetyHealthManualList = await _safetyHealthManualCache.GetCachedListAsync();
                var mappedSafetyHealthManuals = _mapper.Map<List<GetAllSafetyHealthManualsCachedResponse>>(safetyHealthManualList);
                return Result<List<GetAllSafetyHealthManualsCachedResponse>>.Success(mappedSafetyHealthManuals);
            }
        }
    }
}

using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllCached
{
    public class GetAllSafetyHealthManualStatusCachedQuery : IRequest<Result<List<GetAllSafetyHealthManualStatusCachedResponse>>>
    {
        public GetAllSafetyHealthManualStatusCachedQuery()
        {
        }

        public class GetAllSafetyHealthManualStatusCachedQueryHandler : IRequestHandler<GetAllSafetyHealthManualStatusCachedQuery, Result<List<GetAllSafetyHealthManualStatusCachedResponse>>>
        {
            private readonly ISafetyHealthManualStatusCacheRepository _safetyHealthManualStatusCache;
            private readonly IMapper _mapper;

            public GetAllSafetyHealthManualStatusCachedQueryHandler(ISafetyHealthManualStatusCacheRepository safetyHealthManualStatusCache, IMapper mapper)
            {
                _safetyHealthManualStatusCache = safetyHealthManualStatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllSafetyHealthManualStatusCachedResponse>>> Handle(GetAllSafetyHealthManualStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var safetyHealthManualStatusList = await _safetyHealthManualStatusCache.GetCachedListAsync();
                var mappedSafetyHealthManualStatus = _mapper.Map<List<GetAllSafetyHealthManualStatusCachedResponse>>(safetyHealthManualStatusList);
                return Result<List<GetAllSafetyHealthManualStatusCachedResponse>>.Success(mappedSafetyHealthManualStatus);
            }
        }
    }
}

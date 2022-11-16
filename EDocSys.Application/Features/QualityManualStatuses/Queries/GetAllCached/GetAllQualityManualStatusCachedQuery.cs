using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllCached
{
    public class GetAllQualityManualStatusCachedQuery : IRequest<Result<List<GetAllQualityManualStatusCachedResponse>>>
    {
        public GetAllQualityManualStatusCachedQuery()
        {
        }

        public class GetAllQualityManualStatusCachedQueryHandler : IRequestHandler<GetAllQualityManualStatusCachedQuery, Result<List<GetAllQualityManualStatusCachedResponse>>>
        {
            private readonly IQualityManualStatusCacheRepository _qualityManualStatusCache;
            private readonly IMapper _mapper;

            public GetAllQualityManualStatusCachedQueryHandler(IQualityManualStatusCacheRepository qualityManualStatusCache, IMapper mapper)
            {
                _qualityManualStatusCache = qualityManualStatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllQualityManualStatusCachedResponse>>> Handle(GetAllQualityManualStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var qualityManualStatusList = await _qualityManualStatusCache.GetCachedListAsync();
                var mappedQualityManualStatus = _mapper.Map<List<GetAllQualityManualStatusCachedResponse>>(qualityManualStatusList);
                return Result<List<GetAllQualityManualStatusCachedResponse>>.Success(mappedQualityManualStatus);
            }
        }
    }
}

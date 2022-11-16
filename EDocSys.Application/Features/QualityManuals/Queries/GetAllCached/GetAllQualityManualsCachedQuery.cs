using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManuals.Queries.GetAllCached
{
    public class GetAllQualityManualsCachedQuery : IRequest<Result<List<GetAllQualityManualsCachedResponse>>>
    {
        public GetAllQualityManualsCachedQuery()
        {
        }

        public class GetAllQualityManualsCachedQueryHandler : IRequestHandler<GetAllQualityManualsCachedQuery, Result<List<GetAllQualityManualsCachedResponse>>>
        {
            private readonly IQualityManualCacheRepository _qualityManualCache;
            private readonly IMapper _mapper;

            public GetAllQualityManualsCachedQueryHandler(IQualityManualCacheRepository qualityManualCache, IMapper mapper)
            {
                _qualityManualCache = qualityManualCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllQualityManualsCachedResponse>>> Handle(GetAllQualityManualsCachedQuery request, CancellationToken cancellationToken)
            {
                var qualityManualList = await _qualityManualCache.GetCachedListAsync();
                var mappedQualityManuals = _mapper.Map<List<GetAllQualityManualsCachedResponse>>(qualityManualList);
                return Result<List<GetAllQualityManualsCachedResponse>>.Success(mappedQualityManuals);
            }
        }
    }
}

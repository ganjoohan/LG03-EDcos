using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManualStatuses.Queries.GetAllCached
{
    public class GetAllEnvironmentalManualStatusCachedQuery : IRequest<Result<List<GetAllEnvironmentalManualStatusCachedResponse>>>
    {
        public GetAllEnvironmentalManualStatusCachedQuery()
        {
        }

        public class GetAllEnvironmentalManualStatusCachedQueryHandler : IRequestHandler<GetAllEnvironmentalManualStatusCachedQuery, Result<List<GetAllEnvironmentalManualStatusCachedResponse>>>
        {
            private readonly IEnvironmentalManualStatusCacheRepository _environmentalManualStatusCache;
            private readonly IMapper _mapper;

            public GetAllEnvironmentalManualStatusCachedQueryHandler(IEnvironmentalManualStatusCacheRepository environmentalManualStatusCache, IMapper mapper)
            {
                _environmentalManualStatusCache = environmentalManualStatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllEnvironmentalManualStatusCachedResponse>>> Handle(GetAllEnvironmentalManualStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var environmentalManualStatusList = await _environmentalManualStatusCache.GetCachedListAsync();
                var mappedEnvironmentalManualStatus = _mapper.Map<List<GetAllEnvironmentalManualStatusCachedResponse>>(environmentalManualStatusList);
                return Result<List<GetAllEnvironmentalManualStatusCachedResponse>>.Success(mappedEnvironmentalManualStatus);
            }
        }
    }
}

using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManuals.Queries.GetAllCached
{
    public class GetAllEnvironmentalManualsCachedQuery : IRequest<Result<List<GetAllEnvironmentalManualsCachedResponse>>>
    {
        public GetAllEnvironmentalManualsCachedQuery()
        {
        }

        public class GetAllEnvironmentalManualsCachedQueryHandler : IRequestHandler<GetAllEnvironmentalManualsCachedQuery, Result<List<GetAllEnvironmentalManualsCachedResponse>>>
        {
            private readonly IEnvironmentalManualCacheRepository _environmentalManualCache;
            private readonly IMapper _mapper;

            public GetAllEnvironmentalManualsCachedQueryHandler(IEnvironmentalManualCacheRepository environmentalManualCache, IMapper mapper)
            {
                _environmentalManualCache = environmentalManualCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllEnvironmentalManualsCachedResponse>>> Handle(GetAllEnvironmentalManualsCachedQuery request, CancellationToken cancellationToken)
            {
                var environmentalManualList = await _environmentalManualCache.GetCachedListAsync();
                var mappedEnvironmentalManuals = _mapper.Map<List<GetAllEnvironmentalManualsCachedResponse>>(environmentalManualList);
                return Result<List<GetAllEnvironmentalManualsCachedResponse>>.Success(mappedEnvironmentalManuals);
            }
        }
    }
}

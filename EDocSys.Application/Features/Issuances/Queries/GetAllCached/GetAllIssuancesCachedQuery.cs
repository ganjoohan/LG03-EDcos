using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetAllCached
{
    public class GetAllIssuancesCachedQuery : IRequest<Result<List<GetAllIssuancesCachedResponse>>>
    {
        public GetAllIssuancesCachedQuery()
        {
        }

        public class GetAllIssuancesCachedQueryHandler : IRequestHandler<GetAllIssuancesCachedQuery, Result<List<GetAllIssuancesCachedResponse>>>
        {
            private readonly IIssuanceCacheRepository _issuanceCache;
            private readonly IMapper _mapper;

            public GetAllIssuancesCachedQueryHandler(IIssuanceCacheRepository issuanceCache, IMapper mapper)
            {
                _issuanceCache = issuanceCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllIssuancesCachedResponse>>> Handle(GetAllIssuancesCachedQuery request, CancellationToken cancellationToken)
            {
                var issuanceList = await _issuanceCache.GetCachedListAsync();
                var mappedIssuances = _mapper.Map<List<GetAllIssuancesCachedResponse>>(issuanceList);
                return Result<List<GetAllIssuancesCachedResponse>>.Success(mappedIssuances);
            }
        }
    }
}

using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.IssuanceStatuses.Queries.GetAllCached
{
    public class GetAllIssuanceStatusCachedQuery : IRequest<Result<List<GetAllIssuanceStatusCachedResponse>>>
    {
        public GetAllIssuanceStatusCachedQuery()
        {
        }

        public class GetAllIssuanceStatusCachedQueryHandler : IRequestHandler<GetAllIssuanceStatusCachedQuery, Result<List<GetAllIssuanceStatusCachedResponse>>>
        {
            private readonly IIssuanceStatusCacheRepository _issuanceStatusCache;
            private readonly IMapper _mapper;

            public GetAllIssuanceStatusCachedQueryHandler(IIssuanceStatusCacheRepository issuanceStatusCache, IMapper mapper)
            {
                _issuanceStatusCache = issuanceStatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllIssuanceStatusCachedResponse>>> Handle(GetAllIssuanceStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var issuanceStatusList = await _issuanceStatusCache.GetCachedListAsync();
                var mappedIssuanceStatus = _mapper.Map<List<GetAllIssuanceStatusCachedResponse>>(issuanceStatusList);
                return Result<List<GetAllIssuanceStatusCachedResponse>>.Success(mappedIssuanceStatus);
            }
        }
    }
}

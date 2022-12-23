using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetAllCached
{
    public class GetAllIssuancesInfoCachedQuery : IRequest<Result<List<GetAllIssuancesInfoCachedResponse>>>
    {
        public GetAllIssuancesInfoCachedQuery()
        {
        }

        public class GetAllIssuancesInfoCachedQueryHandler : IRequestHandler<GetAllIssuancesInfoCachedQuery, Result<List<GetAllIssuancesInfoCachedResponse>>>
        {
            private readonly IIssuanceInfoCacheRepository _issuanceInfoCache;
            private readonly IMapper _mapper;

            public GetAllIssuancesInfoCachedQueryHandler(IIssuanceInfoCacheRepository issuanceInfoCache, IMapper mapper)
            {
                _issuanceInfoCache = issuanceInfoCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllIssuancesInfoCachedResponse>>> Handle(GetAllIssuancesInfoCachedQuery request, CancellationToken cancellationToken)
            {
                var issuanceInfoList = await _issuanceInfoCache.GetCachedListAsync();
                var mappedIssuancesInfo = _mapper.Map<List<GetAllIssuancesInfoCachedResponse>>(issuanceInfoList);
                return Result<List<GetAllIssuancesInfoCachedResponse>>.Success(mappedIssuancesInfo);
            }
        }
    }
}

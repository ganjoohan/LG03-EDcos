using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.WIStatuses.Queries.GetAllCached
{
    public class GetAllWIStatusCachedQuery : IRequest<Result<List<GetAllWIStatusCachedResponse>>>
    {
        public GetAllWIStatusCachedQuery()
        {
        }

        public class GetAllWIStatusCachedQueryHandler : IRequestHandler<GetAllWIStatusCachedQuery, Result<List<GetAllWIStatusCachedResponse>>>
        {
            private readonly IWIStatusCacheRepository _wistatusCache;
            private readonly IMapper _mapper;

            public GetAllWIStatusCachedQueryHandler(IWIStatusCacheRepository wistatusCache, IMapper mapper)
            {
                _wistatusCache = wistatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllWIStatusCachedResponse>>> Handle(GetAllWIStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var wistatusList = await _wistatusCache.GetCachedListAsync();
                var mappedWIStatus = _mapper.Map<List<GetAllWIStatusCachedResponse>>(wistatusList);
                return Result<List<GetAllWIStatusCachedResponse>>.Success(mappedWIStatus);
            }
        }
    }
}

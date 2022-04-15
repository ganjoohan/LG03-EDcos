using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SOPs.Queries.GetAllCached
{
    public class GetAllSOPsCachedQuery : IRequest<Result<List<GetAllSOPsCachedResponse>>>
    {
        public GetAllSOPsCachedQuery()
        {
        }

        public class GetAllSOPsCachedQueryHandler : IRequestHandler<GetAllSOPsCachedQuery, Result<List<GetAllSOPsCachedResponse>>>
        {
            private readonly ISOPCacheRepository _sopCache;
            private readonly IMapper _mapper;

            public GetAllSOPsCachedQueryHandler(ISOPCacheRepository sopCache, IMapper mapper)
            {
                _sopCache = sopCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllSOPsCachedResponse>>> Handle(GetAllSOPsCachedQuery request, CancellationToken cancellationToken)
            {
                var sopList = await _sopCache.GetCachedListAsync();
                var mappedSOPs = _mapper.Map<List<GetAllSOPsCachedResponse>>(sopList);
                return Result<List<GetAllSOPsCachedResponse>>.Success(mappedSOPs);
            }
        }
    }
}

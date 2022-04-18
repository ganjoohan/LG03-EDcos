using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.SOPStatuses.Queries.GetAllCached
{
    public class GetAllSOPStatusCachedQuery : IRequest<Result<List<GetAllSOPStatusCachedResponse>>>
    {
        public GetAllSOPStatusCachedQuery()
        {
        }

        public class GetAllSOPStatusCachedQueryHandler : IRequestHandler<GetAllSOPStatusCachedQuery, Result<List<GetAllSOPStatusCachedResponse>>>
        {
            private readonly ISOPStatusCacheRepository _sopstatusCache;
            private readonly IMapper _mapper;

            public GetAllSOPStatusCachedQueryHandler(ISOPStatusCacheRepository sopstatusCache, IMapper mapper)
            {
                _sopstatusCache = sopstatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllSOPStatusCachedResponse>>> Handle(GetAllSOPStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var sopstatusList = await _sopstatusCache.GetCachedListAsync();
                var mappedSOPStatus = _mapper.Map<List<GetAllSOPStatusCachedResponse>>(sopstatusList);
                return Result<List<GetAllSOPStatusCachedResponse>>.Success(mappedSOPStatus);
            }
        }
    }
}

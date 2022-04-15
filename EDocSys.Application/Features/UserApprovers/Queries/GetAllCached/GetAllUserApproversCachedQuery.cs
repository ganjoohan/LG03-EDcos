using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.UserApprovers.Queries.GetAllCached
{
    public class GetAllUserApproversCachedQuery : IRequest<Result<List<GetAllUserApproversCachedResponse>>>
    {
        public GetAllUserApproversCachedQuery()
        {
        }

        public class GetAllUserApproversCachedQueryHandler : IRequestHandler<GetAllUserApproversCachedQuery, Result<List<GetAllUserApproversCachedResponse>>>
        {
            private readonly IUserApproverCacheRepository _userapproverCache;
            private readonly IMapper _mapper;

            public GetAllUserApproversCachedQueryHandler(IUserApproverCacheRepository userapproverCache, IMapper mapper)
            {
                _userapproverCache = userapproverCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllUserApproversCachedResponse>>> Handle(GetAllUserApproversCachedQuery request, CancellationToken cancellationToken)
            {
                var userapproverList = await _userapproverCache.GetCachedListAsync();
                var mappedUserApprovers = _mapper.Map<List<GetAllUserApproversCachedResponse>>(userapproverList);
                return Result<List<GetAllUserApproversCachedResponse>>.Success(mappedUserApprovers);
            }
        }
    }
}

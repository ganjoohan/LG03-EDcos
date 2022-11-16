using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManualStatuses.Queries.GetAllCached
{
    public class GetAllLabAccreditationManualStatusCachedQuery : IRequest<Result<List<GetAllLabAccreditationManualStatusCachedResponse>>>
    {
        public GetAllLabAccreditationManualStatusCachedQuery()
        {
        }

        public class GetAllLabAccreditationManualStatusCachedQueryHandler : IRequestHandler<GetAllLabAccreditationManualStatusCachedQuery, Result<List<GetAllLabAccreditationManualStatusCachedResponse>>>
        {
            private readonly ILabAccreditationManualStatusCacheRepository _labAccreditationManualStatusCache;
            private readonly IMapper _mapper;

            public GetAllLabAccreditationManualStatusCachedQueryHandler(ILabAccreditationManualStatusCacheRepository labAccreditationManualStatusCache, IMapper mapper)
            {
                _labAccreditationManualStatusCache = labAccreditationManualStatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllLabAccreditationManualStatusCachedResponse>>> Handle(GetAllLabAccreditationManualStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var labAccreditationManualStatusList = await _labAccreditationManualStatusCache.GetCachedListAsync();
                var mappedLabAccreditationManualStatus = _mapper.Map<List<GetAllLabAccreditationManualStatusCachedResponse>>(labAccreditationManualStatusList);
                return Result<List<GetAllLabAccreditationManualStatusCachedResponse>>.Success(mappedLabAccreditationManualStatus);
            }
        }
    }
}

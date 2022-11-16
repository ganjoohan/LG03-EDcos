using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManuals.Queries.GetAllCached
{
    public class GetAllLabAccreditationManualsCachedQuery : IRequest<Result<List<GetAllLabAccreditationManualsCachedResponse>>>
    {
        public GetAllLabAccreditationManualsCachedQuery()
        {
        }

        public class GetAllLabAccreditationManualsCachedQueryHandler : IRequestHandler<GetAllLabAccreditationManualsCachedQuery, Result<List<GetAllLabAccreditationManualsCachedResponse>>>
        {
            private readonly ILabAccreditationManualCacheRepository _labAccreditationManualCache;
            private readonly IMapper _mapper;

            public GetAllLabAccreditationManualsCachedQueryHandler(ILabAccreditationManualCacheRepository labAccreditationManualCache, IMapper mapper)
            {
                _labAccreditationManualCache = labAccreditationManualCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllLabAccreditationManualsCachedResponse>>> Handle(GetAllLabAccreditationManualsCachedQuery request, CancellationToken cancellationToken)
            {
                var labAccreditationManualList = await _labAccreditationManualCache.GetCachedListAsync();
                var mappedLabAccreditationManuals = _mapper.Map<List<GetAllLabAccreditationManualsCachedResponse>>(labAccreditationManualList);
                return Result<List<GetAllLabAccreditationManualsCachedResponse>>.Success(mappedLabAccreditationManuals);
            }
        }
    }
}

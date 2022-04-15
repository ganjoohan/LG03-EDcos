using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.WIs.Queries.GetAllCached
{
    public class GetAllWIsCachedQuery : IRequest<Result<List<GetAllWIsCachedResponse>>>
    {
        public GetAllWIsCachedQuery()
        {
        }

        public class GetAllWIsCachedQueryHandler : IRequestHandler<GetAllWIsCachedQuery, Result<List<GetAllWIsCachedResponse>>>
        {
            private readonly IWICacheRepository _wiCache;
            private readonly IMapper _mapper;

            public GetAllWIsCachedQueryHandler(IWICacheRepository wiCache, IMapper mapper)
            {
                _wiCache = wiCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllWIsCachedResponse>>> Handle(GetAllWIsCachedQuery request, CancellationToken cancellationToken)
            {
                var wiList = await _wiCache.GetCachedListAsync();
                var mappedWIs = _mapper.Map<List<GetAllWIsCachedResponse>>(wiList);
                return Result<List<GetAllWIsCachedResponse>>.Success(mappedWIs);
            }
        }
    }
}

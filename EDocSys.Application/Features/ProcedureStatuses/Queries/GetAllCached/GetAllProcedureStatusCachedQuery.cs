using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.ProcedureStatuses.Queries.GetAllCached
{
    public class GetAllProcedureStatusCachedQuery : IRequest<Result<List<GetAllProcedureStatusCachedResponse>>>
    {
        public GetAllProcedureStatusCachedQuery()
        {
        }

        public class GetAllProcedureStatusCachedQueryHandler : IRequestHandler<GetAllProcedureStatusCachedQuery, Result<List<GetAllProcedureStatusCachedResponse>>>
        {
            private readonly IProcedureStatusCacheRepository _procedurestatusCache;
            private readonly IMapper _mapper;

            public GetAllProcedureStatusCachedQueryHandler(IProcedureStatusCacheRepository procedurestatusCache, IMapper mapper)
            {
                _procedurestatusCache = procedurestatusCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllProcedureStatusCachedResponse>>> Handle(GetAllProcedureStatusCachedQuery request, CancellationToken cancellationToken)
            {
                // throw new System.NotImplementedException();
                var procedurestatusList = await _procedurestatusCache.GetCachedListAsync();
                var mappedProcedureStatus = _mapper.Map<List<GetAllProcedureStatusCachedResponse>>(procedurestatusList);
                return Result<List<GetAllProcedureStatusCachedResponse>>.Success(mappedProcedureStatus);
            }
        }
    }
}

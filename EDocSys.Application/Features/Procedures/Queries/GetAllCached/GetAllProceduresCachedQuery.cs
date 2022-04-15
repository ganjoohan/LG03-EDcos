using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Procedures.Queries.GetAllCached
{
    public class GetAllProceduresCachedQuery : IRequest<Result<List<GetAllProceduresCachedResponse>>>
    {
        public GetAllProceduresCachedQuery()
        {
        }

        public class GetAllProceduresCachedQueryHandler : IRequestHandler<GetAllProceduresCachedQuery, Result<List<GetAllProceduresCachedResponse>>>
        {
            private readonly IProcedureCacheRepository _procedureCache;
            private readonly IMapper _mapper;

            public GetAllProceduresCachedQueryHandler(IProcedureCacheRepository procedureCache, IMapper mapper)
            {
                _procedureCache = procedureCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllProceduresCachedResponse>>> Handle(GetAllProceduresCachedQuery request, CancellationToken cancellationToken)
            {
                var procedureList = await _procedureCache.GetCachedListAsync();
                var mappedProcedures = _mapper.Map<List<GetAllProceduresCachedResponse>>(procedureList);
                return Result<List<GetAllProceduresCachedResponse>>.Success(mappedProcedures);
            }
        }
    }
}

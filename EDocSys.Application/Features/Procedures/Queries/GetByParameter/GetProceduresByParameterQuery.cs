using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.Procedures.Queries.GetByParameter
{
    public class GetProceduresByParameterQuery : IRequest<Result<List<GetProceduresByParameterResponse>>>
    {
        public int CompId { get; set; }
        public int DeptId { get; set; }

        public class GetProceduresByParameterQueryHandler : IRequestHandler<GetProceduresByParameterQuery, Result<List<GetProceduresByParameterResponse>>>
        {
            private readonly IProcedureCacheRepository _procedureCache;
            private readonly IMapper _mapper;

            public GetProceduresByParameterQueryHandler(IProcedureCacheRepository procedureCache, IMapper mapper)
            {
                _procedureCache = procedureCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetProceduresByParameterResponse>>> Handle(GetProceduresByParameterQuery query, CancellationToken cancellationToken)
            {
                var procedureList = await _procedureCache.GetByParameterAsync(query.CompId, query.DeptId);
                var mappedProcedures = _mapper.Map<List<GetProceduresByParameterResponse>>(procedureList);
                return Result< List<GetProceduresByParameterResponse>>.Success(mappedProcedures);
            }
        }
    }
}
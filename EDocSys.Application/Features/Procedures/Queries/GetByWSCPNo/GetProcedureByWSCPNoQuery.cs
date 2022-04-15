using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.Procedures.Queries.GetByWSCPNo
{
    public class GetProcedureByWSCPNoQuery : IRequest<Result<GetProcedureByWSCPNoResponse>>
    {
        public int Id { get; set; }
        public string wscpno { get; set; }

        public class GetProcedureByWSCPNoQueryHandler : IRequestHandler<GetProcedureByWSCPNoQuery, Result<GetProcedureByWSCPNoResponse>>
        {
            private readonly IProcedureCacheRepository _procedureCache;
            private readonly IMapper _mapper;

            public GetProcedureByWSCPNoQueryHandler(IProcedureCacheRepository procedureCache, IMapper mapper)
            {
                _procedureCache = procedureCache;
                _mapper = mapper;
            }

            public async Task<Result<GetProcedureByWSCPNoResponse>> Handle(GetProcedureByWSCPNoQuery query, CancellationToken cancellationToken)
            {
                var procedure = await _procedureCache.GetByWSCPNoAsync(query.wscpno);
                var mappedProcedure = _mapper.Map<GetProcedureByWSCPNoResponse>(procedure);
                return Result<GetProcedureByWSCPNoResponse>.Success(mappedProcedure);
            }
        }
    }
}
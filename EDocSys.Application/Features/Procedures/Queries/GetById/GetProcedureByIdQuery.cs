using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Procedures.Queries.GetById
{
    public class GetProcedureByIdQuery : IRequest<Result<GetProcedureByIdResponse>>
    {
        public int Id { get; set; }

        public class GetProcedureByIdQueryHandler : IRequestHandler<GetProcedureByIdQuery, Result<GetProcedureByIdResponse>>
        {
            private readonly IProcedureCacheRepository _procedureCache;
            private readonly IMapper _mapper;

            public GetProcedureByIdQueryHandler(IProcedureCacheRepository procedureCache, IMapper mapper)
            {
                _procedureCache = procedureCache;
                _mapper = mapper;
            }

            public async Task<Result<GetProcedureByIdResponse>> Handle(GetProcedureByIdQuery query, CancellationToken cancellationToken)
            {
                var procedure = await _procedureCache.GetByIdAsync(query.Id);
                var mappedProcedure = _mapper.Map<GetProcedureByIdResponse>(procedure);
                return Result<GetProcedureByIdResponse>.Success(mappedProcedure);
            }
        }
    }
}
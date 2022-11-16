using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManuals.Queries.GetById
{
    public class GetSafetyHealthManualByIdQuery : IRequest<Result<GetSafetyHealthManualByIdResponse>>
    {
        public int Id { get; set; }

        public class GetSafetyHealthManualByIdQueryHandler : IRequestHandler<GetSafetyHealthManualByIdQuery, Result<GetSafetyHealthManualByIdResponse>>
        {
            private readonly ISafetyHealthManualCacheRepository _safetyHealthManualCache;
            private readonly IMapper _mapper;

            public GetSafetyHealthManualByIdQueryHandler(ISafetyHealthManualCacheRepository safetyHealthManualCache, IMapper mapper)
            {
                _safetyHealthManualCache = safetyHealthManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetSafetyHealthManualByIdResponse>> Handle(GetSafetyHealthManualByIdQuery query, CancellationToken cancellationToken)
            {
                var safetyHealthManual = await _safetyHealthManualCache.GetByIdAsync(query.Id);
                var mappedSafetyHealthManual = _mapper.Map<GetSafetyHealthManualByIdResponse>(safetyHealthManual);
                return Result<GetSafetyHealthManualByIdResponse>.Success(mappedSafetyHealthManual);
            }
        }
    }
}
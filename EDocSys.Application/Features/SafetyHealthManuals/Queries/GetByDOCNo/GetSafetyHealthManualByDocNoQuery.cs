using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.SafetyHealthManuals.Queries.GetByDOCPNo
{
    public class GetSafetyHealthManualByDOCNoQuery : IRequest<Result<GetSafetyHealthManualByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetSafetyHealthManualByDOCNoQueryHandler : IRequestHandler<GetSafetyHealthManualByDOCNoQuery, Result<GetSafetyHealthManualByDOCNoResponse>>
        {
            private readonly ISafetyHealthManualCacheRepository _safetyHealthManualCache;
            private readonly IMapper _mapper;

            public GetSafetyHealthManualByDOCNoQueryHandler(ISafetyHealthManualCacheRepository safetyHealthManualCache, IMapper mapper)
            {
                _safetyHealthManualCache = safetyHealthManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetSafetyHealthManualByDOCNoResponse>> Handle(GetSafetyHealthManualByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var safetyHealthManual = await _safetyHealthManualCache.GetByDOCNoAsync(query.docno);
                var mappedSafetyHealthManual = _mapper.Map<GetSafetyHealthManualByDOCNoResponse>(safetyHealthManual);
                return Result<GetSafetyHealthManualByDOCNoResponse>.Success(mappedSafetyHealthManual);
            }
        }
    }
}
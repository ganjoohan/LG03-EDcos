using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.EnvironmentalManuals.Queries.GetByDOCPNo
{
    public class GetEnvironmentalManualByDOCNoQuery : IRequest<Result<GetEnvironmentalManualByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetEnvironmentalManualByDOCNoQueryHandler : IRequestHandler<GetEnvironmentalManualByDOCNoQuery, Result<GetEnvironmentalManualByDOCNoResponse>>
        {
            private readonly IEnvironmentalManualCacheRepository _environmentalManualCache;
            private readonly IMapper _mapper;

            public GetEnvironmentalManualByDOCNoQueryHandler(IEnvironmentalManualCacheRepository environmentalManualCache, IMapper mapper)
            {
                _environmentalManualCache = environmentalManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetEnvironmentalManualByDOCNoResponse>> Handle(GetEnvironmentalManualByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var environmentalManual = await _environmentalManualCache.GetByDOCNoAsync(query.docno);
                var mappedEnvironmentalManual = _mapper.Map<GetEnvironmentalManualByDOCNoResponse>(environmentalManual);
                return Result<GetEnvironmentalManualByDOCNoResponse>.Success(mappedEnvironmentalManual);
            }
        }
    }
}
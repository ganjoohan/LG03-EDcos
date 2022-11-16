using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.QualityManuals.Queries.GetByDOCPNo
{
    public class GetQualityManualByDOCNoQuery : IRequest<Result<GetQualityManualByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetQualityManualByDOCNoQueryHandler : IRequestHandler<GetQualityManualByDOCNoQuery, Result<GetQualityManualByDOCNoResponse>>
        {
            private readonly IQualityManualCacheRepository _qualityManualCache;
            private readonly IMapper _mapper;

            public GetQualityManualByDOCNoQueryHandler(IQualityManualCacheRepository qualityManualCache, IMapper mapper)
            {
                _qualityManualCache = qualityManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQualityManualByDOCNoResponse>> Handle(GetQualityManualByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var qualityManual = await _qualityManualCache.GetByDOCNoAsync(query.docno);
                var mappedQualityManual = _mapper.Map<GetQualityManualByDOCNoResponse>(qualityManual);
                return Result<GetQualityManualByDOCNoResponse>.Success(mappedQualityManual);
            }
        }
    }
}
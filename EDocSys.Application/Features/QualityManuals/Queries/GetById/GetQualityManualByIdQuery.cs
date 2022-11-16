using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManuals.Queries.GetById
{
    public class GetQualityManualByIdQuery : IRequest<Result<GetQualityManualByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQualityManualByIdQueryHandler : IRequestHandler<GetQualityManualByIdQuery, Result<GetQualityManualByIdResponse>>
        {
            private readonly IQualityManualCacheRepository _qualityManualCache;
            private readonly IMapper _mapper;

            public GetQualityManualByIdQueryHandler(IQualityManualCacheRepository qualityManualCache, IMapper mapper)
            {
                _qualityManualCache = qualityManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQualityManualByIdResponse>> Handle(GetQualityManualByIdQuery query, CancellationToken cancellationToken)
            {
                var qualityManual = await _qualityManualCache.GetByIdAsync(query.Id);
                var mappedQualityManual = _mapper.Map<GetQualityManualByIdResponse>(qualityManual);
                return Result<GetQualityManualByIdResponse>.Success(mappedQualityManual);
            }
        }
    }
}
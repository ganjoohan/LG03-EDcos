using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManuals.Queries.GetById
{
    public class GetEnvironmentalManualByIdQuery : IRequest<Result<GetEnvironmentalManualByIdResponse>>
    {
        public int Id { get; set; }

        public class GetEnvironmentalManualByIdQueryHandler : IRequestHandler<GetEnvironmentalManualByIdQuery, Result<GetEnvironmentalManualByIdResponse>>
        {
            private readonly IEnvironmentalManualCacheRepository _environmentalManualCache;
            private readonly IMapper _mapper;

            public GetEnvironmentalManualByIdQueryHandler(IEnvironmentalManualCacheRepository environmentalManualCache, IMapper mapper)
            {
                _environmentalManualCache = environmentalManualCache;
                _mapper = mapper;
            }

            public async Task<Result<GetEnvironmentalManualByIdResponse>> Handle(GetEnvironmentalManualByIdQuery query, CancellationToken cancellationToken)
            {
                var environmentalManual = await _environmentalManualCache.GetByIdAsync(query.Id);
                var mappedEnvironmentalManual = _mapper.Map<GetEnvironmentalManualByIdResponse>(environmentalManual);
                return Result<GetEnvironmentalManualByIdResponse>.Success(mappedEnvironmentalManual);
            }
        }
    }
}
using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;

namespace EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetById
{
    public class GetLionSteelByIdQuery : IRequest<Result<GetLionSteelByIdResponse>>
    {
        public int Id { get; set; }

        public class GetLionSteelByIdQueryHandler : IRequestHandler<GetLionSteelByIdQuery, Result<GetLionSteelByIdResponse>>
        {
            private readonly ILionSteelCacheRepository _lionSteelCache;
            private readonly IMapper _mapper;

            public GetLionSteelByIdQueryHandler(ILionSteelCacheRepository lionSteelCache, IMapper mapper)
            {
                _lionSteelCache = lionSteelCache;
                _mapper = mapper;
            }

            public async Task<Result<GetLionSteelByIdResponse>> Handle(GetLionSteelByIdQuery query, CancellationToken cancellationToken)
            {
                var lionSteel = await _lionSteelCache.GetByIdAsync(query.Id);
                var mappedLionSteel = _mapper.Map<GetLionSteelByIdResponse>(lionSteel);
                return Result<GetLionSteelByIdResponse>.Success(mappedLionSteel);
            }
        }
    }
}
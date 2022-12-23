using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceInfoByIdQuery : IRequest<Result<GetIssuanceInfoByIdResponse>>
    {
        public int Id { get; set; }

        public class GetIssuanceInfoByIdQueryHandler : IRequestHandler<GetIssuanceInfoByIdQuery, Result<GetIssuanceInfoByIdResponse>>
        {
            private readonly IIssuanceInfoCacheRepository _issuanceInfoCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoByIdQueryHandler(IIssuanceInfoCacheRepository issuanceInfoCache, IMapper mapper)
            {
                _issuanceInfoCache = issuanceInfoCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceInfoByIdResponse>> Handle(GetIssuanceInfoByIdQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoCache.GetByIdAsync(query.Id);
                var mappedIssuanceInfo = _mapper.Map<GetIssuanceInfoByIdResponse>(issuanceInfo);
                return Result<GetIssuanceInfoByIdResponse>.Success(mappedIssuanceInfo);
            }
        }
    }
}
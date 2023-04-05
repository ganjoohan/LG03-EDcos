using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceInfoByIdQuery : IRequest<Result<GetIssuanceInfoByHIdResponse>>
    {
        public int Id { get; set; }


        public class GetIssuanceInfoByIdQueryHandler : IRequestHandler<GetIssuanceInfoByIdQuery, Result<GetIssuanceInfoByHIdResponse>>
        {
            private readonly IIssuanceInfoCacheRepository _issuanceInfoCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoByIdQueryHandler(IIssuanceInfoCacheRepository issuanceInfoCache, IMapper mapper)
            {
                _issuanceInfoCache = issuanceInfoCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceInfoByHIdResponse>> Handle(GetIssuanceInfoByIdQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoCache.GetByIdAsync(query.Id);
                var mappedIssuanceInfo = _mapper.Map<GetIssuanceInfoByHIdResponse>(issuanceInfo);
                return Result<GetIssuanceInfoByHIdResponse>.Success(mappedIssuanceInfo);
            }
        }
    }
}
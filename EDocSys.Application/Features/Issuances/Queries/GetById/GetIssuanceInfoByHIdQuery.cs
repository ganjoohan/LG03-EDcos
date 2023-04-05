using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceInfoByHIdQuery : IRequest<Result<List<GetIssuanceInfoByHIdResponse>>>
    {
        public int HId { get; set; }


        public class GetIssuanceInfoByIdQueryHandler : IRequestHandler<GetIssuanceInfoByHIdQuery, Result<List<GetIssuanceInfoByHIdResponse>>>
        {
            private readonly IIssuanceInfoCacheRepository _issuanceInfoCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoByIdQueryHandler(IIssuanceInfoCacheRepository issuanceInfoCache, IMapper mapper)
            {
                _issuanceInfoCache = issuanceInfoCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetIssuanceInfoByHIdResponse>>> Handle(GetIssuanceInfoByHIdQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoCache.GetByHIdAsync(query.HId);
                var mappedIssuanceInfo = _mapper.Map<List<GetIssuanceInfoByHIdResponse>>(issuanceInfo);
                return Result<List<GetIssuanceInfoByHIdResponse>>.Success(mappedIssuanceInfo);
            }
        }
    }
}
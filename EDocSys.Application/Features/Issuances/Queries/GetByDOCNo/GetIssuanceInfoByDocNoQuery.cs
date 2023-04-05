using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.Issuances.Queries.GetByDOCPNo
{
    public class GetIssuanceInfoByDOCNoQuery : IRequest<Result<List<GetIssuanceInfoByDOCNoResponse>>>
    {
        public int Id { get; set; }
        public string docNo { get; set; }
        public string docType { get; set; }

        public class GetIssuanceInfoByDOCNoQueryHandler : IRequestHandler<GetIssuanceInfoByDOCNoQuery, Result<List<GetIssuanceInfoByDOCNoResponse>>>
        {
            private readonly IIssuanceInfoCacheRepository _issuanceInfoCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoByDOCNoQueryHandler(IIssuanceInfoCacheRepository issuanceInfoCache, IMapper mapper)
            {
                _issuanceInfoCache = issuanceInfoCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetIssuanceInfoByDOCNoResponse>>> Handle(GetIssuanceInfoByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoCache.GetByDOCNoAsync(query.docNo, query.docType);
                var mappedIssuanceInfo = _mapper.Map<List<GetIssuanceInfoByDOCNoResponse>>(issuanceInfo);
                return Result<List<GetIssuanceInfoByDOCNoResponse>>.Success(mappedIssuanceInfo);
            }
        }
    }
}
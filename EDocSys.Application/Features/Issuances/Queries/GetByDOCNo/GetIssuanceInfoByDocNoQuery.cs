using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.Issuances.Queries.GetByDOCPNo
{
    public class GetIssuanceInfoByDOCNoQuery : IRequest<Result<GetIssuanceInfoByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docNo { get; set; }
        public string docType { get; set; }

        public class GetIssuanceInfoByDOCNoQueryHandler : IRequestHandler<GetIssuanceInfoByDOCNoQuery, Result<GetIssuanceInfoByDOCNoResponse>>
        {
            private readonly IIssuanceInfoCacheRepository _issuanceInfoCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoByDOCNoQueryHandler(IIssuanceInfoCacheRepository issuanceInfoCache, IMapper mapper)
            {
                _issuanceInfoCache = issuanceInfoCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceInfoByDOCNoResponse>> Handle(GetIssuanceInfoByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoCache.GetByDOCNoAsync(query.docNo, query.docType);
                var mappedIssuanceInfo = _mapper.Map<GetIssuanceInfoByDOCNoResponse>(issuanceInfo);
                return Result<GetIssuanceInfoByDOCNoResponse>.Success(mappedIssuanceInfo);
            }
        }
    }
}
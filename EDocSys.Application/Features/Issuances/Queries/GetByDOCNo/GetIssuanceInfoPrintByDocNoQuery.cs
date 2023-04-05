using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.Issuances.Queries.GetByDOCPNo
{
    public class GetIssuanceInfoPrintByDOCNoQuery : IRequest<Result<GetIssuanceInfoPrintByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetIssuanceInfoPrintByDOCNoQueryHandler : IRequestHandler<GetIssuanceInfoPrintByDOCNoQuery, Result<GetIssuanceInfoPrintByDOCNoResponse>>
        {
            private readonly IIssuanceInfoPrintCacheRepository _issuanceInfoPrintCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoPrintByDOCNoQueryHandler(IIssuanceInfoPrintCacheRepository issuanceInfoPrintCache, IMapper mapper)
            {
                _issuanceInfoPrintCache = issuanceInfoPrintCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceInfoPrintByDOCNoResponse>> Handle(GetIssuanceInfoPrintByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfoPrint = await _issuanceInfoPrintCache.GetByDOCNoAsync(query.docno);
                var mappedIssuanceInfoPrint = _mapper.Map<GetIssuanceInfoPrintByDOCNoResponse>(issuanceInfoPrint);
                return Result<GetIssuanceInfoPrintByDOCNoResponse>.Success(mappedIssuanceInfoPrint);
            }
        }
    }
}
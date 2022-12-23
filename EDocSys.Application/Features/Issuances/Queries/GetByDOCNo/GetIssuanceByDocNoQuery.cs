using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.Issuances.Queries.GetByDOCPNo
{
    public class GetIssuanceByDOCNoQuery : IRequest<Result<GetIssuanceByDOCNoResponse>>
    {
        public int Id { get; set; }
        public string docno { get; set; }

        public class GetIssuanceByDOCNoQueryHandler : IRequestHandler<GetIssuanceByDOCNoQuery, Result<GetIssuanceByDOCNoResponse>>
        {
            private readonly IIssuanceCacheRepository _issuanceCache;
            private readonly IMapper _mapper;

            public GetIssuanceByDOCNoQueryHandler(IIssuanceCacheRepository issuanceCache, IMapper mapper)
            {
                _issuanceCache = issuanceCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceByDOCNoResponse>> Handle(GetIssuanceByDOCNoQuery query, CancellationToken cancellationToken)
            {
                var issuance = await _issuanceCache.GetByDOCNoAsync(query.docno);
                var mappedIssuance = _mapper.Map<GetIssuanceByDOCNoResponse>(issuance);
                return Result<GetIssuanceByDOCNoResponse>.Success(mappedIssuance);
            }
        }
    }
}
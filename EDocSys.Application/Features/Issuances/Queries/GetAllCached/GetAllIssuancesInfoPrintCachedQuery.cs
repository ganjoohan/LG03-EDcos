using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetAllCached
{
    public class GetAllIssuancesInfoPrintCachedQuery : IRequest<Result<List<GetAllIssuancesInfoPrintCachedResponse>>>
    {
        public GetAllIssuancesInfoPrintCachedQuery()
        {
        }

        public class GetAllIssuancesInfoPrintCachedQueryHandler : IRequestHandler<GetAllIssuancesInfoPrintCachedQuery, Result<List<GetAllIssuancesInfoPrintCachedResponse>>>
        {
            private readonly IIssuanceInfoPrintCacheRepository _issuanceInfoPrintCache;
            private readonly IMapper _mapper;

            public GetAllIssuancesInfoPrintCachedQueryHandler(IIssuanceInfoPrintCacheRepository issuanceInfoPrintCache, IMapper mapper)
            {
                _issuanceInfoPrintCache = issuanceInfoPrintCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllIssuancesInfoPrintCachedResponse>>> Handle(GetAllIssuancesInfoPrintCachedQuery request, CancellationToken cancellationToken)
            {
                var issuanceInfoPrintList = await _issuanceInfoPrintCache.GetCachedListAsync();
                var mappedIssuancesInfoPrint = _mapper.Map<List<GetAllIssuancesInfoPrintCachedResponse>>(issuanceInfoPrintList);
                return Result<List<GetAllIssuancesInfoPrintCachedResponse>>.Success(mappedIssuancesInfoPrint);
            }
        }
    }
}

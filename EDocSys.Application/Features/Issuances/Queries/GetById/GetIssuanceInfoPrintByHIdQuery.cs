using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceInfoPrintByHIdQuery : IRequest<Result<List<GetIssuanceInfoPrintByHIdResponse>>>
    {
        public int IssInfoId { get; set; }


        public class GetIssuanceInfoPrintByIdQueryHandler : IRequestHandler<GetIssuanceInfoPrintByHIdQuery, Result<List<GetIssuanceInfoPrintByHIdResponse>>>
        {
            private readonly IIssuanceInfoPrintCacheRepository _issuanceInfoPrintCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoPrintByIdQueryHandler(IIssuanceInfoPrintCacheRepository issuanceInfoPrintCache, IMapper mapper)
            {
                _issuanceInfoPrintCache = issuanceInfoPrintCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetIssuanceInfoPrintByHIdResponse>>> Handle(GetIssuanceInfoPrintByHIdQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfoPrint = await _issuanceInfoPrintCache.GetByHIdAsync(query.IssInfoId);
                var mappedIssuanceInfoPrint = _mapper.Map<List<GetIssuanceInfoPrintByHIdResponse>>(issuanceInfoPrint);
                return Result<List<GetIssuanceInfoPrintByHIdResponse>>.Success(mappedIssuanceInfoPrint);
            }
        }
    }
}
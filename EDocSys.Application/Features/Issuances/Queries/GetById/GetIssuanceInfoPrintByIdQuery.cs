using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceInfoPrintByIdQuery : IRequest<Result<GetIssuanceInfoPrintByHIdResponse>>
    {
        public int Id { get; set; }


        public class GetIssuanceInfoPrintByIdQueryHandler : IRequestHandler<GetIssuanceInfoPrintByIdQuery, Result<GetIssuanceInfoPrintByHIdResponse>>
        {
            private readonly IIssuanceInfoPrintCacheRepository _issuanceInfoPrintCache;
            private readonly IMapper _mapper;

            public GetIssuanceInfoPrintByIdQueryHandler(IIssuanceInfoPrintCacheRepository issuanceInfoPrintCache, IMapper mapper)
            {
                _issuanceInfoPrintCache = issuanceInfoPrintCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceInfoPrintByHIdResponse>> Handle(GetIssuanceInfoPrintByIdQuery query, CancellationToken cancellationToken)
            {
                var issuanceInfoPrint = await _issuanceInfoPrintCache.GetByIdAsync(query.Id);
                var mappedIssuanceInfoPrint = _mapper.Map<GetIssuanceInfoPrintByHIdResponse>(issuanceInfoPrint);
                return Result<GetIssuanceInfoPrintByHIdResponse>.Success(mappedIssuanceInfoPrint);
            }
        }
    }
}
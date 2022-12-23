using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceByIdQuery : IRequest<Result<GetIssuanceByIdResponse>>
    {
        public int Id { get; set; }

        public class GetIssuanceByIdQueryHandler : IRequestHandler<GetIssuanceByIdQuery, Result<GetIssuanceByIdResponse>>
        {
            private readonly IIssuanceCacheRepository _issuanceCache;
            private readonly IMapper _mapper;

            public GetIssuanceByIdQueryHandler(IIssuanceCacheRepository issuanceCache, IMapper mapper)
            {
                _issuanceCache = issuanceCache;
                _mapper = mapper;
            }

            public async Task<Result<GetIssuanceByIdResponse>> Handle(GetIssuanceByIdQuery query, CancellationToken cancellationToken)
            {
                var issuance = await _issuanceCache.GetByIdAsync(query.Id);
                var mappedIssuance = _mapper.Map<GetIssuanceByIdResponse>(issuance);
                return Result<GetIssuanceByIdResponse>.Success(mappedIssuance);
            }
        }
    }
}
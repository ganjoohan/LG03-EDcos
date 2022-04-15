using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.WIs.Queries.GetById
{
    public class GetWIByIdQuery : IRequest<Result<GetWIByIdResponse>>
    {
        public int Id { get; set; }

        public class GetWIByIdQueryHandler : IRequestHandler<GetWIByIdQuery, Result<GetWIByIdResponse>>
        {
            private readonly IWICacheRepository _wiCache;
            private readonly IMapper _mapper;

            public GetWIByIdQueryHandler(IWICacheRepository wiCache, IMapper mapper)
            {
                _wiCache = wiCache;
                _mapper = mapper;
            }

            public async Task<Result<GetWIByIdResponse>> Handle(GetWIByIdQuery query, CancellationToken cancellationToken)
            {
                var wi = await _wiCache.GetByIdAsync(query.Id);
                var mappedWI = _mapper.Map<GetWIByIdResponse>(wi);
                return Result<GetWIByIdResponse>.Success(mappedWI);
            }
        }
    }
}
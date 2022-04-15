using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SOPs.Queries.GetById
{
    public class GetSOPByIdQuery : IRequest<Result<GetSOPByIdResponse>>
    {
        public int Id { get; set; }

        public class GetSOPByIdQueryHandler : IRequestHandler<GetSOPByIdQuery, Result<GetSOPByIdResponse>>
        {
            private readonly ISOPCacheRepository _sopCache;
            private readonly IMapper _mapper;

            public GetSOPByIdQueryHandler(ISOPCacheRepository sopCache, IMapper mapper)
            {
                _sopCache = sopCache;
                _mapper = mapper;
            }

            public async Task<Result<GetSOPByIdResponse>> Handle(GetSOPByIdQuery query, CancellationToken cancellationToken)
            {
                var sop = await _sopCache.GetByIdAsync(query.Id);
                var mappedSOP = _mapper.Map<GetSOPByIdResponse>(sop);
                return Result<GetSOPByIdResponse>.Success(mappedSOP);
            }
        }
    }
}
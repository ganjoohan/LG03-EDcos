using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.UserApprovers.Queries.GetById
{
    public class GetUserApproverByIdQuery : IRequest<Result<GetUserApproverByIdResponse>>
    {
        public int Id { get; set; }

        public class GetUserApproverByIdQueryHandler : IRequestHandler<GetUserApproverByIdQuery, Result<GetUserApproverByIdResponse>>
        {
            private readonly IUserApproverCacheRepository _userapproverCache;
            private readonly IMapper _mapper;

            public GetUserApproverByIdQueryHandler(IUserApproverCacheRepository userapproverCache, IMapper mapper)
            {
                _userapproverCache = userapproverCache;
                _mapper = mapper;
            }

            public async Task<Result<GetUserApproverByIdResponse>> Handle(GetUserApproverByIdQuery query, CancellationToken cancellationToken)
            {
                var userapprover = await _userapproverCache.GetByIdAsync(query.Id);
                var mappedUserApprover = _mapper.Map<GetUserApproverByIdResponse>(userapprover);
                return Result<GetUserApproverByIdResponse>.Success(mappedUserApprover);
            }
        }
    }
}

using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.SOPs.Queries.GetByParameter
{
    public class GetSOPsByParameterQuery : IRequest<Result<List<GetSOPsByParameterResponse>>>
    {
        public int CompId { get; set; }
        public int DeptId { get; set; }

        public class GetSOPsByParameterQueryHandler : IRequestHandler<GetSOPsByParameterQuery, Result<List<GetSOPsByParameterResponse>>>
        {
            private readonly ISOPCacheRepository _sopCache;
            private readonly IMapper _mapper;

            public GetSOPsByParameterQueryHandler(ISOPCacheRepository sopCache, IMapper mapper)
            {
                _sopCache = sopCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetSOPsByParameterResponse>>> Handle(GetSOPsByParameterQuery query, CancellationToken cancellationToken)
            {
                var sopList = await _sopCache.GetByParameterAsync(query.CompId, query.DeptId);
                var mappedSOPs = _mapper.Map< List<GetSOPsByParameterResponse>>(sopList);
                return Result< List<GetSOPsByParameterResponse>>.Success(mappedSOPs);
            }
        }
    }
}
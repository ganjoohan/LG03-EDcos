using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EDocSys.Application.Features.WIs.Queries.GetByParameter
{
    public class GetWIsByParameterQuery : IRequest<Result<List<GetWIsByParameterResponse>>>
    {
        public int CompId { get; set; }
        public int DeptId { get; set; }

        public class GetWIsByParameterQueryHandler : IRequestHandler<GetWIsByParameterQuery, Result<List<GetWIsByParameterResponse>>>
        {
            private readonly IWICacheRepository _wiCache;
            private readonly IMapper _mapper;

            public GetWIsByParameterQueryHandler(IWICacheRepository wiCache, IMapper mapper)
            {
                _wiCache = wiCache;
                _mapper = mapper;
            }

            public async Task<Result<List<GetWIsByParameterResponse>>> Handle(GetWIsByParameterQuery query, CancellationToken cancellationToken)
            {
                var wiList = await _wiCache.GetByParameterAsync(query.CompId, query.DeptId);
                var mappedWIs = _mapper.Map< List<GetWIsByParameterResponse>>(wiList);
                return Result< List<GetWIsByParameterResponse>>.Success(mappedWIs);
            }
        }
    }
}
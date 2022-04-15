using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Companies.Queries.GetAllCached
{
    public class GetAllCompaniesCachedQuery : IRequest<Result<List<GetAllCompaniesCachedResponse>>>
    {
        public GetAllCompaniesCachedQuery()
        {
        }
    }

    public class GetAllCompaniesCachedQueryHandler : IRequestHandler<GetAllCompaniesCachedQuery, Result<List<GetAllCompaniesCachedResponse>>>
    {
        private readonly ICompanyCacheRepository _procedureCache;
        private readonly IMapper _mapper;

        public GetAllCompaniesCachedQueryHandler(ICompanyCacheRepository procedureCache, IMapper mapper)
        {
            _procedureCache = procedureCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllCompaniesCachedResponse>>> Handle(GetAllCompaniesCachedQuery request, CancellationToken cancellationToken)
        {
            var companyList = await _procedureCache.GetCachedListAsync();
            var mappedCompanies = _mapper.Map<List<GetAllCompaniesCachedResponse>>(companyList);
            return Result<List<GetAllCompaniesCachedResponse>>.Success(mappedCompanies);
        }
    }
}
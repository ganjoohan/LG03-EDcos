using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Departments.Queries.GetAllCached
{
    public class GetAllDepartmentsCachedQuery : IRequest<Result<List<GetAllDepartmentsCachedResponse>>>
    {
        public GetAllDepartmentsCachedQuery()
        {
        }
    }

    public class GetAllDepartmentsCachedQueryHandler : IRequestHandler<GetAllDepartmentsCachedQuery, Result<List<GetAllDepartmentsCachedResponse>>>
    {
        private readonly IDepartmentCacheRepository _procedureCache;
        private readonly IMapper _mapper;

        public GetAllDepartmentsCachedQueryHandler(IDepartmentCacheRepository procedureCache, IMapper mapper)
        {
            _procedureCache = procedureCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllDepartmentsCachedResponse>>> Handle(GetAllDepartmentsCachedQuery request, CancellationToken cancellationToken)
        {
            var departmentList = await _procedureCache.GetCachedListAsync();
            var mappedDepartments = _mapper.Map<List<GetAllDepartmentsCachedResponse>>(departmentList);
            return Result<List<GetAllDepartmentsCachedResponse>>.Success(mappedDepartments);
        }
    }
}
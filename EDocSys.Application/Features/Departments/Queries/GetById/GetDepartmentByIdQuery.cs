using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Departments.Queries.GetById
{
    public class GetDepartmentByIdQuery : IRequest<Result<GetDepartmentByIdResponse>>
    {
        public int Id { get; set; }

        public class GetProcedureByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Result<GetDepartmentByIdResponse>>
        {
            private readonly IDepartmentCacheRepository _departmentCache;
            private readonly IMapper _mapper;

            public GetProcedureByIdQueryHandler(IDepartmentCacheRepository procedureCache, IMapper mapper)
            {
                _departmentCache = procedureCache;
                _mapper = mapper;
            }

            public async Task<Result<GetDepartmentByIdResponse>> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken)
            {
                var procedure = await _departmentCache.GetByIdAsync(query.Id);
                var mappedProcedure = _mapper.Map<GetDepartmentByIdResponse>(procedure);
                return Result<GetDepartmentByIdResponse>.Success(mappedProcedure);
            }
        }
    }
}
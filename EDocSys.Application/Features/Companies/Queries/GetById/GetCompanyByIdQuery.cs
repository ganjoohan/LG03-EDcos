using EDocSys.Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Companies.Queries.GetById
{
    public class GetCompanyByIdQuery : IRequest<Result<GetCompanyByIdResponse>>
    {
        public int Id { get; set; }

        public class GetProcedureByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<GetCompanyByIdResponse>>
        {
            private readonly ICompanyCacheRepository _companyCache;
            private readonly IMapper _mapper;

            public GetProcedureByIdQueryHandler(ICompanyCacheRepository procedureCache, IMapper mapper)
            {
                _companyCache = procedureCache;
                _mapper = mapper;
            }

            public async Task<Result<GetCompanyByIdResponse>> Handle(GetCompanyByIdQuery query, CancellationToken cancellationToken)
            {
                var procedure = await _companyCache.GetByIdAsync(query.Id);
                var mappedProcedure = _mapper.Map<GetCompanyByIdResponse>(procedure);
                return Result<GetCompanyByIdResponse>.Success(mappedProcedure);
            }
        }
    }
}
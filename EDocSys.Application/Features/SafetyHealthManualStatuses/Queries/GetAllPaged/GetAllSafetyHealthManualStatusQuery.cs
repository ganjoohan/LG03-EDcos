using EDocSys.Application.Extensions;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllPaged
{
    public class GetAllSafetyHealthManualStatusQuery : IRequest<PaginatedResult<GetAllSafetyHealthManualStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllSafetyHealthManualStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllSafetyHealthManualStatusQueryHandler : IRequestHandler<GetAllSafetyHealthManualStatusQuery, PaginatedResult<GetAllSafetyHealthManualStatusResponse>>
    {
        private readonly ISafetyHealthManualStatusRepository _repository;

        public GetAllSafetyHealthManualStatusQueryHandler(ISafetyHealthManualStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllSafetyHealthManualStatusResponse>> Handle(GetAllSafetyHealthManualStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<SafetyHealthManualStatus, GetAllSafetyHealthManualStatusResponse>> expression = e => new GetAllSafetyHealthManualStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.SafetyHealthManualStatuses
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

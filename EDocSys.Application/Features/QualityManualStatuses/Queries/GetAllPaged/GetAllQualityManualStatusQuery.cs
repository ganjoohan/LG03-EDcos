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

namespace EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllPaged
{
    public class GetAllQualityManualStatusQuery : IRequest<PaginatedResult<GetAllQualityManualStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllQualityManualStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllQualityManualStatusQueryHandler : IRequestHandler<GetAllQualityManualStatusQuery, PaginatedResult<GetAllQualityManualStatusResponse>>
    {
        private readonly IQualityManualStatusRepository _repository;

        public GetAllQualityManualStatusQueryHandler(IQualityManualStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllQualityManualStatusResponse>> Handle(GetAllQualityManualStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<QualityManualStatus, GetAllQualityManualStatusResponse>> expression = e => new GetAllQualityManualStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.QualityManualStatuses
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

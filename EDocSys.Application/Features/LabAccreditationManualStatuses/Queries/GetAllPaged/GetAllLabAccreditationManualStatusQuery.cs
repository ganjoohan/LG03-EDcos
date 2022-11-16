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

namespace EDocSys.Application.Features.LabAccreditationManualStatuses.Queries.GetAllPaged
{
    public class GetAllLabAccreditationManualStatusQuery : IRequest<PaginatedResult<GetAllLabAccreditationManualStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllLabAccreditationManualStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllLabAccreditationManualStatusQueryHandler : IRequestHandler<GetAllLabAccreditationManualStatusQuery, PaginatedResult<GetAllLabAccreditationManualStatusResponse>>
    {
        private readonly ILabAccreditationManualStatusRepository _repository;

        public GetAllLabAccreditationManualStatusQueryHandler(ILabAccreditationManualStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllLabAccreditationManualStatusResponse>> Handle(GetAllLabAccreditationManualStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<LabAccreditationManualStatus, GetAllLabAccreditationManualStatusResponse>> expression = e => new GetAllLabAccreditationManualStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.LabAccreditationManualStatuses
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

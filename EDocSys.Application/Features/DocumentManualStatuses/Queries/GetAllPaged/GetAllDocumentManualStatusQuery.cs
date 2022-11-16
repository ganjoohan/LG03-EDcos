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

namespace EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllPaged
{
    public class GetAllDocumentManualStatusQuery : IRequest<PaginatedResult<GetAllDocumentManualStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllDocumentManualStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllDocumentManualStatusQueryHandler : IRequestHandler<GetAllDocumentManualStatusQuery, PaginatedResult<GetAllDocumentManualStatusResponse>>
    {
        private readonly IDocumentManualStatusRepository _repository;

        public GetAllDocumentManualStatusQueryHandler(IDocumentManualStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllDocumentManualStatusResponse>> Handle(GetAllDocumentManualStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DocumentManualStatus, GetAllDocumentManualStatusResponse>> expression = e => new GetAllDocumentManualStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.DocumentManualStatuses
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

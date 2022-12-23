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

namespace EDocSys.Application.Features.IssuanceStatuses.Queries.GetAllPaged
{
    public class GetAllIssuanceStatusQuery : IRequest<PaginatedResult<GetAllIssuanceStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllIssuanceStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllIssuanceStatusQueryHandler : IRequestHandler<GetAllIssuanceStatusQuery, PaginatedResult<GetAllIssuanceStatusResponse>>
    {
        private readonly IIssuanceStatusRepository _repository;

        public GetAllIssuanceStatusQueryHandler(IIssuanceStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllIssuanceStatusResponse>> Handle(GetAllIssuanceStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<IssuanceStatus, GetAllIssuanceStatusResponse>> expression = e => new GetAllIssuanceStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.IssuanceStatuses
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

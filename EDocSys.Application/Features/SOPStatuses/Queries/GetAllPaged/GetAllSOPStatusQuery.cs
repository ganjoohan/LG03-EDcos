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

namespace EDocSys.Application.Features.SOPStatuses.Queries.GetAllPaged
{
    public class GetAllSOPStatusQuery : IRequest<PaginatedResult<GetAllSOPStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllSOPStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllSOPStatusQueryHandler : IRequestHandler<GetAllSOPStatusQuery, PaginatedResult<GetAllSOPStatusResponse>>
    {
        private readonly ISOPStatusRepository _repository;

        public GetAllSOPStatusQueryHandler(ISOPStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllSOPStatusResponse>> Handle(GetAllSOPStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<SOPStatus, GetAllSOPStatusResponse>> expression = e => new GetAllSOPStatusResponse
            {
                Id = e.Id,

            };
            var paginatedList = await _repository.SOPStatus
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

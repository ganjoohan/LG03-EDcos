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

namespace EDocSys.Application.Features.WIStatuses.Queries.GetAllPaged
{
    public class GetAllWIStatusQuery : IRequest<PaginatedResult<GetAllWIStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllWIStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllWIStatusQueryHandler : IRequestHandler<GetAllWIStatusQuery, PaginatedResult<GetAllWIStatusResponse>>
    {
        private readonly IWIStatusRepository _repository;

        public GetAllWIStatusQueryHandler(IWIStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllWIStatusResponse>> Handle(GetAllWIStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<WIStatus, GetAllWIStatusResponse>> expression = e => new GetAllWIStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.WIStatus
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

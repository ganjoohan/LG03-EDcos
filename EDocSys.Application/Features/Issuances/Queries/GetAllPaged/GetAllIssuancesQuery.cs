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

namespace EDocSys.Application.Features.Issuances.Queries.GetAllPaged
{
    public class GetAllIssuancesQuery : IRequest<PaginatedResult<GetAllIssuancesResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllIssuancesQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllIssuancesQueryHandler : IRequestHandler<GetAllIssuancesQuery, PaginatedResult<GetAllIssuancesResponse>>
    {
        private readonly IIssuanceRepository _repository;

        public GetAllIssuancesQueryHandler(IIssuanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllIssuancesResponse>> Handle(GetAllIssuancesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Issuance, GetAllIssuancesResponse>> expression = e => new GetAllIssuancesResponse
            {
                Id = e.Id,
                DOCNo = e.DOCNo,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.Issuances
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
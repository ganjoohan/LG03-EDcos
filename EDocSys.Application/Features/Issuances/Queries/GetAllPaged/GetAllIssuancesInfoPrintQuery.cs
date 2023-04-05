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
    public class GetAllIssuancesInfoPrintQuery : IRequest<PaginatedResult<GetAllIssuancesInfoPrintResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllIssuancesInfoPrintQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllIssuancesInfoPrintQueryHandler : IRequestHandler<GetAllIssuancesInfoPrintQuery, PaginatedResult<GetAllIssuancesInfoPrintResponse>>
    {
        private readonly IIssuanceInfoPrintRepository _repository;

        public GetAllIssuancesInfoPrintQueryHandler(IIssuanceInfoPrintRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllIssuancesInfoPrintResponse>> Handle(GetAllIssuancesInfoPrintQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<IssuanceInfoPrint, GetAllIssuancesInfoPrintResponse>> expression = e => new GetAllIssuancesInfoPrintResponse
            {
                Id = e.Id,
                IssInfoId = e.IssInfoId,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.IssuancesInfoPrint
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
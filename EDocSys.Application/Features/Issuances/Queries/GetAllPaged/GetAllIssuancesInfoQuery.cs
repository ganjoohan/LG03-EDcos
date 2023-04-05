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
    public class GetAllIssuancesInfoQuery : IRequest<PaginatedResult<GetAllIssuancesInfoResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllIssuancesInfoQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllIssuancesInfoQueryHandler : IRequestHandler<GetAllIssuancesInfoQuery, PaginatedResult<GetAllIssuancesInfoResponse>>
    {
        private readonly IIssuanceInfoRepository _repository;

        public GetAllIssuancesInfoQueryHandler(IIssuanceInfoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllIssuancesInfoResponse>> Handle(GetAllIssuancesInfoQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<IssuanceInfo, GetAllIssuancesInfoResponse>> expression = e => new GetAllIssuancesInfoResponse
            {
                Id = e.Id,
                HId = e.HId,
                DOCId = e.DOCId,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.IssuancesInfo
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
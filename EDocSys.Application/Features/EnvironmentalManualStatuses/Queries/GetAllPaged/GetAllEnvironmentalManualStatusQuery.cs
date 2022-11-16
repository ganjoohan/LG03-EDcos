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

namespace EDocSys.Application.Features.EnvironmentalManualStatuses.Queries.GetAllPaged
{
    public class GetAllEnvironmentalManualStatusQuery : IRequest<PaginatedResult<GetAllEnvironmentalManualStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllEnvironmentalManualStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllEnvironmentalManualStatusQueryHandler : IRequestHandler<GetAllEnvironmentalManualStatusQuery, PaginatedResult<GetAllEnvironmentalManualStatusResponse>>
    {
        private readonly IEnvironmentalManualStatusRepository _repository;

        public GetAllEnvironmentalManualStatusQueryHandler(IEnvironmentalManualStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllEnvironmentalManualStatusResponse>> Handle(GetAllEnvironmentalManualStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<EnvironmentalManualStatus, GetAllEnvironmentalManualStatusResponse>> expression = e => new GetAllEnvironmentalManualStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.EnvironmentalManualStatuses
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

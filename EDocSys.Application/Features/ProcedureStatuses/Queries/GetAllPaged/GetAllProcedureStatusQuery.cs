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

namespace EDocSys.Application.Features.ProcedureStatuses.Queries.GetAllPaged
{
    public class GetAllProcedureStatusQuery : IRequest<PaginatedResult<GetAllProcedureStatusResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllProcedureStatusQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllProcedureStatusQueryHandler : IRequestHandler<GetAllProcedureStatusQuery, PaginatedResult<GetAllProcedureStatusResponse>>
    {
        private readonly IProcedureStatusRepository _repository;

        public GetAllProcedureStatusQueryHandler(IProcedureStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllProcedureStatusResponse>> Handle(GetAllProcedureStatusQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ProcedureStatus, GetAllProcedureStatusResponse>> expression = e => new GetAllProcedureStatusResponse
            {
                Id = e.Id,
                
            };
            var paginatedList = await _repository.ProcedureStatus
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}

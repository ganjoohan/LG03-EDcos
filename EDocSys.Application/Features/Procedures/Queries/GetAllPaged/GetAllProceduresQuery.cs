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

namespace EDocSys.Application.Features.Procedures.Queries.GetAllPaged
{
    public class GetAllProceduresQuery : IRequest<PaginatedResult<GetAllProceduresResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllProceduresQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllProceduresQueryHandler : IRequestHandler<GetAllProceduresQuery, PaginatedResult<GetAllProceduresResponse>>
    {
        private readonly IProcedureRepository _repository;

        public GetAllProceduresQueryHandler(IProcedureRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllProceduresResponse>> Handle(GetAllProceduresQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Procedure, GetAllProceduresResponse>> expression = e => new GetAllProceduresResponse
            {
                Id = e.Id,
                WSCPNo = e.WSCPNo,
                SOPNo = e.SOPNo,
                WINo = e.WINo,
                Title = e.Title,
                Purpose = e.Purpose,
                Scope = e.Scope,
                Definition = e.Definition,
                Body = e.Body,
                hasSOP = e.hasSOP
            };
            var paginatedList = await _repository.Procedures
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
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

namespace EDocSys.Application.Features.SOPs.Queries.GetAllPaged
{
    public class GetAllSOPsQuery : IRequest<PaginatedResult<GetAllSOPsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllSOPsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllSOPsQueryHandler : IRequestHandler<GetAllSOPsQuery, PaginatedResult<GetAllSOPsResponse>>
    {
        private readonly ISOPRepository _repository;

        public GetAllSOPsQueryHandler(ISOPRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllSOPsResponse>> Handle(GetAllSOPsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<SOP, GetAllSOPsResponse>> expression = e => new GetAllSOPsResponse
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
                hasWI = e.hasWI
            };
            var paginatedList = await _repository.SOPs
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
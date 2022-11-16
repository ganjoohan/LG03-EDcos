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

namespace EDocSys.Application.Features.WIs.Queries.GetAllPaged
{
    public class GetAllWIsQuery : IRequest<PaginatedResult<GetAllWIsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllWIsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllWIsQueryHandler : IRequestHandler<GetAllWIsQuery, PaginatedResult<GetAllWIsResponse>>
    {
        private readonly IWIRepository _repository;

        public GetAllWIsQueryHandler(IWIRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllWIsResponse>> Handle(GetAllWIsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<WI, GetAllWIsResponse>> expression = e => new GetAllWIsResponse
            {
                Id = e.Id,
                WSCPNo = e.WSCPNo,
                SOPNo = e.SOPNo,
                WINo = e.WINo,
                Title = e.Title,
                //Purpose = e.Purpose,
                //Scope = e.Scope,
                //Definition = e.Definition,
                Body = e.Body,
                IsActive = e.IsActive,
                 WSCPId = e.WSCPId,
                 SOPId = e.SOPId
            };
            var paginatedList = await _repository.WIs
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
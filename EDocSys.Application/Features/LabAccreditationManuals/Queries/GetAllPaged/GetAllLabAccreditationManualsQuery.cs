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

namespace EDocSys.Application.Features.LabAccreditationManuals.Queries.GetAllPaged
{
    public class GetAllLabAccreditationManualsQuery : IRequest<PaginatedResult<GetAllLabAccreditationManualsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllLabAccreditationManualsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllLabAccreditationManualsQueryHandler : IRequestHandler<GetAllLabAccreditationManualsQuery, PaginatedResult<GetAllLabAccreditationManualsResponse>>
    {
        private readonly ILabAccreditationManualRepository _repository;

        public GetAllLabAccreditationManualsQueryHandler(ILabAccreditationManualRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllLabAccreditationManualsResponse>> Handle(GetAllLabAccreditationManualsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<LabAccreditationManual, GetAllLabAccreditationManualsResponse>> expression = e => new GetAllLabAccreditationManualsResponse
            {
                Id = e.Id,
                DOCNo = e.DOCNo,
                Title = e.Title,
                Category = e.Category,
                Body = e.Body,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.LabAccreditationManuals
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
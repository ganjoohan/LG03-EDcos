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

namespace EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllPaged
{
    public class GetAllSafetyHealthManualsQuery : IRequest<PaginatedResult<GetAllSafetyHealthManualsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllSafetyHealthManualsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllSafetyHealthManualsQueryHandler : IRequestHandler<GetAllSafetyHealthManualsQuery, PaginatedResult<GetAllSafetyHealthManualsResponse>>
    {
        private readonly ISafetyHealthManualRepository _repository;

        public GetAllSafetyHealthManualsQueryHandler(ISafetyHealthManualRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllSafetyHealthManualsResponse>> Handle(GetAllSafetyHealthManualsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<SafetyHealthManual, GetAllSafetyHealthManualsResponse>> expression = e => new GetAllSafetyHealthManualsResponse
            {
                Id = e.Id,
                DOCNo = e.DOCNo,
                Title = e.Title,
                Category = e.Category,
                Body = e.Body,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.SafetyHealthManuals
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
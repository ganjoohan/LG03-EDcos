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

namespace EDocSys.Application.Features.EnvironmentalManuals.Queries.GetAllPaged
{
    public class GetAllEnvironmentalManualsQuery : IRequest<PaginatedResult<GetAllEnvironmentalManualsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllEnvironmentalManualsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllEnvironmentalManualsQueryHandler : IRequestHandler<GetAllEnvironmentalManualsQuery, PaginatedResult<GetAllEnvironmentalManualsResponse>>
    {
        private readonly IEnvironmentalManualRepository _repository;

        public GetAllEnvironmentalManualsQueryHandler(IEnvironmentalManualRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllEnvironmentalManualsResponse>> Handle(GetAllEnvironmentalManualsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<EnvironmentalManual, GetAllEnvironmentalManualsResponse>> expression = e => new GetAllEnvironmentalManualsResponse
            {
                Id = e.Id,
                DOCNo = e.DOCNo,
                Title = e.Title,
                Category = e.Category,
                Body = e.Body,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.EnvironmentalManuals
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
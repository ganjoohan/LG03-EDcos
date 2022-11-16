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

namespace EDocSys.Application.Features.DocumentManuals.Queries.GetAllPaged
{
    public class GetAllDocumentManualsQuery : IRequest<PaginatedResult<GetAllDocumentManualsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllDocumentManualsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllDocumentManualsQueryHandler : IRequestHandler<GetAllDocumentManualsQuery, PaginatedResult<GetAllDocumentManualsResponse>>
    {
        private readonly IDocumentManualRepository _repository;

        public GetAllDocumentManualsQueryHandler(IDocumentManualRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllDocumentManualsResponse>> Handle(GetAllDocumentManualsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DocumentManual, GetAllDocumentManualsResponse>> expression = e => new GetAllDocumentManualsResponse
            {
                Id = e.Id,
                DOCNo = e.DOCNo,
                Title = e.Title,
                Category = e.Category,
                Body = e.Body,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.DocumentManuals
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
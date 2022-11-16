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

namespace EDocSys.Application.Features.QualityManuals.Queries.GetAllPaged
{
    public class GetAllQualityManualsQuery : IRequest<PaginatedResult<GetAllQualityManualsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllQualityManualsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllQualityManualsQueryHandler : IRequestHandler<GetAllQualityManualsQuery, PaginatedResult<GetAllQualityManualsResponse>>
    {
        private readonly IQualityManualRepository _repository;

        public GetAllQualityManualsQueryHandler(IQualityManualRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllQualityManualsResponse>> Handle(GetAllQualityManualsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<QualityManual, GetAllQualityManualsResponse>> expression = e => new GetAllQualityManualsResponse
            {
                Id = e.Id,
                DOCNo = e.DOCNo,
                SectionNo = e.SectionNo,
                Title = e.Title,
                Category = e.Category,
                Body = e.Body,
                IsActive = e.IsActive

            };
            var paginatedList = await _repository.QualityManuals
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
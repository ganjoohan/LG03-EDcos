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
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using EDocSys.Domain.Entities.QualityRecord;

namespace EDocSys.Application.Features.QualityFeatures.LionSteels.Queries.GetAllPaged
{
    public class GetAllLionSteelsQuery : IRequest<PaginatedResult<GetAllLionSteelsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllLionSteelsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllLionSteelsQueryHandler : IRequestHandler<GetAllLionSteelsQuery, PaginatedResult<GetAllLionSteelsResponse>>
    {
        private readonly ILionSteelRepository _repository;

        public GetAllLionSteelsQueryHandler(ILionSteelRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResult<GetAllLionSteelsResponse>> Handle(GetAllLionSteelsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<LionSteel, GetAllLionSteelsResponse>> expression = e => new GetAllLionSteelsResponse
            {
                Id = e.Id,
                FormNo = e.FormNo,
                //Title = e.Title,
                //Category = e.Category,
                //Body = e.Body,
                //IsActive = e.IsActive

            };
            var paginatedList = await _repository.LionSteels
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
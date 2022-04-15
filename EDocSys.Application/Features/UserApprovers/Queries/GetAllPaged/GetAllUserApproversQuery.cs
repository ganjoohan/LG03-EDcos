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
using EDocSys.Domain.Entities.UserMaster;

namespace EDocSys.Application.Features.UserApprovers.Queries.GetAllPaged
{
    public class GetAllUserApproversQuery : IRequest<PaginatedResult<GetAllUserApproversResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllUserApproversQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public class GetAllUserApproversQueryHandler : IRequestHandler<GetAllUserApproversQuery, PaginatedResult<GetAllUserApproversResponse>>
        {
            private readonly IUserApproverRepository _repository;

            public GetAllUserApproversQueryHandler(IUserApproverRepository repository)
            {
                _repository = repository;
            }

            public async Task<PaginatedResult<GetAllUserApproversResponse>> Handle(GetAllUserApproversQuery request, CancellationToken cancellationToken)
            {
                Expression<Func<UserApprover, GetAllUserApproversResponse>> expression = e => new GetAllUserApproversResponse
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    CompanyId = e.CompanyId,
                    DepartmentId = e.DepartmentId,
                    ApprovalType = e.ApprovalType

                };
                var paginatedList = await _repository.UserApprovers
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return paginatedList;
            }
        }
    }
}

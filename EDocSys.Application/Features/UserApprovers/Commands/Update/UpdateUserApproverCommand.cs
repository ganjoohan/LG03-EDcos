using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.UserApprovers.Commands.Update
{
    public class UpdateUserApproverCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string ApprovalType { get; set; }

        public class UpdateUserApproverCommandHandler : IRequestHandler<UpdateUserApproverCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserApproverRepository _userapproverRepository;

            public UpdateUserApproverCommandHandler(IUserApproverRepository userapproverRepository, IUnitOfWork unitOfWork)
            {
                _userapproverRepository = userapproverRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateUserApproverCommand command, CancellationToken cancellationToken)
            {
                var userapprover = await _userapproverRepository.GetByIdAsync(command.Id);

                if (userapprover == null)
                {
                    return Result<int>.Fail($"UserApprover Not Found.");
                }
                else
                {
                    userapprover.UserId = command.UserId ?? userapprover.UserId;
                    userapprover.DepartmentId = (command.DepartmentId == 0) ? userapprover.DepartmentId : command.DepartmentId;
                    userapprover.CompanyId = (command.CompanyId == 0) ? userapprover.CompanyId : command.CompanyId;

                    userapprover.ApprovalType = command.ApprovalType ?? userapprover.ApprovalType;
                    await _userapproverRepository.UpdateAsync(userapprover);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(userapprover.Id);
                }
            }
        }
    }
}

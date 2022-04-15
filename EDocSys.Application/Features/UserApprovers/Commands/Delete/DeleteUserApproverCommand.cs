using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.UserApprovers.Commands.Delete
{
    public class DeleteUserApproverCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteUserApproverCommandHandler : IRequestHandler<DeleteUserApproverCommand, Result<int>>
        {
            private readonly IUserApproverRepository _userapproverRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteUserApproverCommandHandler(IUserApproverRepository userapproverRepository, IUnitOfWork unitOfWork)
            {
                _userapproverRepository = userapproverRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteUserApproverCommand command, CancellationToken cancellationToken)
            {
                var userapprover = await _userapproverRepository.GetByIdAsync(command.Id);
                await _userapproverRepository.DeleteAsync(userapprover);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(userapprover.Id);
            }
        }
    }
}

using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Commands.Delete
{
    public class DeleteIssuanceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteIssuanceCommandHandler : IRequestHandler<DeleteIssuanceCommand, Result<int>>
        {
            private readonly IIssuanceRepository _issuanceRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteIssuanceCommandHandler(IIssuanceRepository issuanceRepository, IUnitOfWork unitOfWork)
            {
                _issuanceRepository = issuanceRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteIssuanceCommand command, CancellationToken cancellationToken)
            {
                var issuance = await _issuanceRepository.GetByIdAsync(command.Id);
                await _issuanceRepository.DeleteAsync(issuance);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(issuance.Id);
            }
        }
    }
}
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManuals.Commands.Delete
{
    public class DeleteSafetyHealthManualCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteSafetyHealthManualCommandHandler : IRequestHandler<DeleteSafetyHealthManualCommand, Result<int>>
        {
            private readonly ISafetyHealthManualRepository _safetyHealthManualRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteSafetyHealthManualCommandHandler(ISafetyHealthManualRepository safetyHealthManualRepository, IUnitOfWork unitOfWork)
            {
                _safetyHealthManualRepository = safetyHealthManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteSafetyHealthManualCommand command, CancellationToken cancellationToken)
            {
                var safetyHealthmanual = await _safetyHealthManualRepository.GetByIdAsync(command.Id);
                await _safetyHealthManualRepository.DeleteAsync(safetyHealthmanual);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(safetyHealthmanual.Id);
            }
        }
    }
}
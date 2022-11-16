using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManuals.Commands.Delete
{
    public class DeleteLabAccreditationManualCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteLabAccreditationManualCommandHandler : IRequestHandler<DeleteLabAccreditationManualCommand, Result<int>>
        {
            private readonly ILabAccreditationManualRepository _labAccreditationManualRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteLabAccreditationManualCommandHandler(ILabAccreditationManualRepository labAccreditationManualRepository, IUnitOfWork unitOfWork)
            {
                _labAccreditationManualRepository = labAccreditationManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteLabAccreditationManualCommand command, CancellationToken cancellationToken)
            {
                var labAccreditationmanual = await _labAccreditationManualRepository.GetByIdAsync(command.Id);
                await _labAccreditationManualRepository.DeleteAsync(labAccreditationmanual);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(labAccreditationmanual.Id);
            }
        }
    }
}
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;

namespace EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Delete
{
    public class DeleteLionSteelCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteLionSteelCommandHandler : IRequestHandler<DeleteLionSteelCommand, Result<int>>
        {
            private readonly ILionSteelRepository _lionSteelRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteLionSteelCommandHandler(ILionSteelRepository lionSteelRepository, IUnitOfWork unitOfWork)
            {
                _lionSteelRepository = lionSteelRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteLionSteelCommand command, CancellationToken cancellationToken)
            {
                var lionSteel = await _lionSteelRepository.GetByIdAsync(command.Id);
                await _lionSteelRepository.DeleteAsync(lionSteel);
                await _unitOfWork.CommitQuality(cancellationToken);
                return Result<int>.Success(lionSteel.Id);
            }
        }
    }
}
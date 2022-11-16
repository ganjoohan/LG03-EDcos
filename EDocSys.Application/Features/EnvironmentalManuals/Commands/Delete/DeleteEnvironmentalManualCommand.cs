using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManuals.Commands.Delete
{
    public class DeleteEnvironmentalManualCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteEnvironmentalManualCommandHandler : IRequestHandler<DeleteEnvironmentalManualCommand, Result<int>>
        {
            private readonly IEnvironmentalManualRepository _environmentalManualRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteEnvironmentalManualCommandHandler(IEnvironmentalManualRepository environmentalManualRepository, IUnitOfWork unitOfWork)
            {
                _environmentalManualRepository = environmentalManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteEnvironmentalManualCommand command, CancellationToken cancellationToken)
            {
                var environmentalmanual = await _environmentalManualRepository.GetByIdAsync(command.Id);
                await _environmentalManualRepository.DeleteAsync(environmentalmanual);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(environmentalmanual.Id);
            }
        }
    }
}
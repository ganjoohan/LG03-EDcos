using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManuals.Commands.Delete
{
    public class DeleteQualityManualCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteQualityManualCommandHandler : IRequestHandler<DeleteQualityManualCommand, Result<int>>
        {
            private readonly IQualityManualRepository _qualityManualRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteQualityManualCommandHandler(IQualityManualRepository qualityManualRepository, IUnitOfWork unitOfWork)
            {
                _qualityManualRepository = qualityManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteQualityManualCommand command, CancellationToken cancellationToken)
            {
                var qualitymanual = await _qualityManualRepository.GetByIdAsync(command.Id);
                await _qualityManualRepository.DeleteAsync(qualitymanual);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(qualitymanual.Id);
            }
        }
    }
}
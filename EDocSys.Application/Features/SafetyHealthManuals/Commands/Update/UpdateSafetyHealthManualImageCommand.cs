using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManuals.Commands.Update
{
    public class UpdateSafetyHealthManualImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateSafetyHealthManualImageCommandHandler : IRequestHandler<UpdateSafetyHealthManualImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ISafetyHealthManualRepository _safetyHealthManualRepository;

            public UpdateSafetyHealthManualImageCommandHandler(ISafetyHealthManualRepository safetyHealthManualRepository, IUnitOfWork unitOfWork)
            {
                _safetyHealthManualRepository = safetyHealthManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateSafetyHealthManualImageCommand command, CancellationToken cancellationToken)
            {
                var safetyHealthManual = await _safetyHealthManualRepository.GetByIdAsync(command.Id);

                if (safetyHealthManual == null)
                {
                    throw new ApiException($"Safety and Health Manual Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _safetyHealthManualRepository.UpdateAsync(safetyHealthManual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(safetyHealthManual.Id);
                }
            }
        }
    }
}
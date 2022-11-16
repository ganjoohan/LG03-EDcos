using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManuals.Commands.Update
{
    public class UpdateEnvironmentalManualImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateEnvironmentalManualImageCommandHandler : IRequestHandler<UpdateEnvironmentalManualImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEnvironmentalManualRepository _environmentalManualRepository;

            public UpdateEnvironmentalManualImageCommandHandler(IEnvironmentalManualRepository environmentalManualRepository, IUnitOfWork unitOfWork)
            {
                _environmentalManualRepository = environmentalManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateEnvironmentalManualImageCommand command, CancellationToken cancellationToken)
            {
                var environmentalManual = await _environmentalManualRepository.GetByIdAsync(command.Id);

                if (environmentalManual == null)
                {
                    throw new ApiException($"Environmental Manual Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _environmentalManualRepository.UpdateAsync(environmentalManual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(environmentalManual.Id);
                }
            }
        }
    }
}
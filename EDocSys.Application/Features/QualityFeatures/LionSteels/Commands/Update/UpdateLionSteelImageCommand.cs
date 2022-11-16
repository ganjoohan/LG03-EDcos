using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;

namespace EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Update
{
    public class UpdateLionSteelImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateLionSteelImageCommandHandler : IRequestHandler<UpdateLionSteelImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILionSteelRepository _lionSteelRepository;

            public UpdateLionSteelImageCommandHandler(ILionSteelRepository lionSteelRepository, IUnitOfWork unitOfWork)
            {
                _lionSteelRepository = lionSteelRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateLionSteelImageCommand command, CancellationToken cancellationToken)
            {
                var lionSteel = await _lionSteelRepository.GetByIdAsync(command.Id);

                if (lionSteel == null)
                {
                    throw new ApiException($"LionSteel Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _lionSteelRepository.UpdateAsync(lionSteel);
                    await _unitOfWork.CommitQuality(cancellationToken);
                    return Result<int>.Success(lionSteel.Id);
                }
            }
        }
    }
}
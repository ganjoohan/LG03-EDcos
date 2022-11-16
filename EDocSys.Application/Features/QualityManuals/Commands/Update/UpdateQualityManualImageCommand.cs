using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManuals.Commands.Update
{
    public class UpdateQualityManualImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateQualityManualImageCommandHandler : IRequestHandler<UpdateQualityManualImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IQualityManualRepository _qualityManualRepository;

            public UpdateQualityManualImageCommandHandler(IQualityManualRepository qualityManualRepository, IUnitOfWork unitOfWork)
            {
                _qualityManualRepository = qualityManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateQualityManualImageCommand command, CancellationToken cancellationToken)
            {
                var qualityManual = await _qualityManualRepository.GetByIdAsync(command.Id);

                if (qualityManual == null)
                {
                    throw new ApiException($"Quality Manual Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _qualityManualRepository.UpdateAsync(qualityManual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(qualityManual.Id);
                }
            }
        }
    }
}
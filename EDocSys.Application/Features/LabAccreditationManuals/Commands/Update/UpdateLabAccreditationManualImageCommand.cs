using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManuals.Commands.Update
{
    public class UpdateLabAccreditationManualImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateLabAccreditationManualImageCommandHandler : IRequestHandler<UpdateLabAccreditationManualImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILabAccreditationManualRepository _labAccreditationManualRepository;

            public UpdateLabAccreditationManualImageCommandHandler(ILabAccreditationManualRepository labAccreditationManualRepository, IUnitOfWork unitOfWork)
            {
                _labAccreditationManualRepository = labAccreditationManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateLabAccreditationManualImageCommand command, CancellationToken cancellationToken)
            {
                var labAccreditationManual = await _labAccreditationManualRepository.GetByIdAsync(command.Id);

                if (labAccreditationManual == null)
                {
                    throw new ApiException($"Lab Accreditation Manual Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _labAccreditationManualRepository.UpdateAsync(labAccreditationManual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(labAccreditationManual.Id);
                }
            }
        }
    }
}
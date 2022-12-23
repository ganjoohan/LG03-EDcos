using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateIssuanceImageCommandHandler : IRequestHandler<UpdateIssuanceImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceRepository _issuanceRepository;

            public UpdateIssuanceImageCommandHandler(IIssuanceRepository issuanceRepository, IUnitOfWork unitOfWork)
            {
                _issuanceRepository = issuanceRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceImageCommand command, CancellationToken cancellationToken)
            {
                var issuance = await _issuanceRepository.GetByIdAsync(command.Id);

                if (issuance == null)
                {
                    throw new ApiException($"Issuance Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _issuanceRepository.UpdateAsync(issuance);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuance.Id);
                }
            }
        }
    }
}
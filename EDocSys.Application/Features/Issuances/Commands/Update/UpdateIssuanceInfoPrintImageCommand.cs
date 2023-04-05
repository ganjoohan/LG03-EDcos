using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceInfoPrintImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateIssuanceInfoPrintImageCommandHandler : IRequestHandler<UpdateIssuanceInfoPrintImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceInfoPrintRepository _issuanceInfoPrintRepository;

            public UpdateIssuanceInfoPrintImageCommandHandler(IIssuanceInfoPrintRepository issuanceInfoPrintRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoPrintRepository = issuanceInfoPrintRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceInfoPrintImageCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfoPrint = await _issuanceInfoPrintRepository.GetByIdAsync(command.Id);

                if (issuanceInfoPrint == null)
                {
                    throw new ApiException($"Issuance Info Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _issuanceInfoPrintRepository.UpdateAsync(issuanceInfoPrint);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuanceInfoPrint.Id);
                }
            }
        }
    }
}
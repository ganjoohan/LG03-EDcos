using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceInfoImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateIssuanceInfoImageCommandHandler : IRequestHandler<UpdateIssuanceInfoImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceInfoRepository _issuanceInfoRepository;

            public UpdateIssuanceInfoImageCommandHandler(IIssuanceInfoRepository issuanceInfoRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoRepository = issuanceInfoRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceInfoImageCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoRepository.GetByIdAsync(command.Id);

                if (issuanceInfo == null)
                {
                    throw new ApiException($"Issuance Info Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _issuanceInfoRepository.UpdateAsync(issuanceInfo);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuanceInfo.Id);
                }
            }
        }
    }
}
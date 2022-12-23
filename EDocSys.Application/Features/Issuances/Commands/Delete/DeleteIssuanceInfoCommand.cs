using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Commands.Delete
{
    public class DeleteIssuanceInfoCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteIssuanceInfoCommandHandler : IRequestHandler<DeleteIssuanceInfoCommand, Result<int>>
        {
            private readonly IIssuanceInfoRepository _issuanceInfoRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteIssuanceInfoCommandHandler(IIssuanceInfoRepository issuanceInfoRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoRepository = issuanceInfoRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteIssuanceInfoCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoRepository.GetByIdAsync(command.Id);
                await _issuanceInfoRepository.DeleteAsync(issuanceInfo);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(issuanceInfo.Id);
            }
        }
    }
}
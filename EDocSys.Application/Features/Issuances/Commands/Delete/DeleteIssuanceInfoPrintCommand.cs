using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Commands.Delete
{
    public class DeleteIssuanceInfoPrintCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteIssuanceInfoPrintCommandHandler : IRequestHandler<DeleteIssuanceInfoPrintCommand, Result<int>>
        {
            private readonly IIssuanceInfoPrintRepository _issuanceInfoPrintRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteIssuanceInfoPrintCommandHandler(IIssuanceInfoPrintRepository issuanceInfoPrintRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoPrintRepository = issuanceInfoPrintRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteIssuanceInfoPrintCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfoPrint = await _issuanceInfoPrintRepository.GetByIdAsync(command.Id);
                await _issuanceInfoPrintRepository.DeleteAsync(issuanceInfoPrint);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(issuanceInfoPrint.Id);
            }
        }
    }
}
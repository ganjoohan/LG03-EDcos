using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceInfoPrintCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int IssInfoId { get; set; }
        public string RecipientName { get; set; }
        public bool IsPrinted { get; set; }
        public DateTime? PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string ReturnedBy { get; set; }
        public bool IsActive { get; set; }

        public class UpdateIssuanceInfoPrintCommandHandler : IRequestHandler<UpdateIssuanceInfoPrintCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceInfoPrintRepository _issuanceInfoPrintRepository;

            public UpdateIssuanceInfoPrintCommandHandler(IIssuanceInfoPrintRepository issuanceInfoPrintRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoPrintRepository = issuanceInfoPrintRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceInfoPrintCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfoPrint = await _issuanceInfoPrintRepository.GetByIdAsync(command.Id);

                if (issuanceInfoPrint == null)
                {
                    return Result<int>.Fail($"Issuance Info Print Not Found.");
                }
                else
                {
                    issuanceInfoPrint.IssInfoId = command.IssInfoId;
                    issuanceInfoPrint.RecipientName = command.RecipientName ?? issuanceInfoPrint.RecipientName;
                    issuanceInfoPrint.IsPrinted = command.IsPrinted ? true : issuanceInfoPrint.IsPrinted;
                    issuanceInfoPrint.PrintedDate = command.PrintedDate ?? issuanceInfoPrint.PrintedDate;
                    issuanceInfoPrint.PrintedBy = command.PrintedBy ?? issuanceInfoPrint.PrintedBy;
                    issuanceInfoPrint.IsReturned = command.IsReturned ? true : issuanceInfoPrint.IsReturned;
                    issuanceInfoPrint.ReturnedDate = command.ReturnedDate ?? issuanceInfoPrint.ReturnedDate;
                    issuanceInfoPrint.ReturnedBy = command.ReturnedBy ?? issuanceInfoPrint.ReturnedBy;
                    issuanceInfoPrint.IsActive = command.IsActive;
                    await _issuanceInfoPrintRepository.UpdateAsync(issuanceInfoPrint);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuanceInfoPrint.Id);
                }
            }
        }
    }
}
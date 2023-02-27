using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string DOCNo { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string RequestedBy { get; set; }
        public string VerifiedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AcknowledgedBy { get; set; }
        public string RequestedByPosition { get; set; }       
        public int PrintCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public DateTime? RequestedByDate { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public string IssuanceStatusView { get; set; }
        public string DOCStatus { get; set; }
        public class UpdateIssuanceCommandHandler : IRequestHandler<UpdateIssuanceCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceRepository _issuanceRepository;

            public UpdateIssuanceCommandHandler(IIssuanceRepository issuanceRepository, IUnitOfWork unitOfWork)
            {
                _issuanceRepository = issuanceRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceCommand command, CancellationToken cancellationToken)
            {
                var issuance = await _issuanceRepository.GetByIdAsync(command.Id);

                if (issuance == null)
                {
                    return Result<int>.Fail($"Issuance Not Found.");
                }
                else
                {                   

                    issuance.CompanyId = (command.CompanyId == 0) ? issuance.CompanyId : command.CompanyId;
                    issuance.DepartmentId = (command.DepartmentId == 0) ? issuance.DepartmentId : command.DepartmentId;

                    issuance.VerifiedBy = command.VerifiedBy ?? issuance.VerifiedBy;
                    issuance.ApprovedBy = command.ApprovedBy ?? issuance.ApprovedBy;
                    issuance.AcknowledgedBy = command.AcknowledgedBy ?? issuance.AcknowledgedBy;

                    issuance.RequestedBy = command.RequestedBy ?? issuance.RequestedBy;
                    issuance.RequestedByDate = command.RequestedByDate ?? issuance.RequestedByDate;
                    issuance.RequestedByPosition = command.RequestedByPosition ?? issuance.RequestedByPosition;

                    issuance.IsActive = command.IsActive;
                    issuance.IsArchive = command.IsArchive ? true : issuance.IsArchive;
                    issuance.ArchiveId = (command.ArchiveId == 0) ? issuance.ArchiveId : command.ArchiveId;
                    issuance.PrintCount = (command.PrintCount == 0) ? issuance.PrintCount : command.PrintCount;
                    issuance.ArchiveDate = command.ArchiveDate ?? issuance.ArchiveDate;
                    issuance.IssuanceStatusView = command.IssuanceStatusView;
                    await _issuanceRepository.UpdateAsync(issuance);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuance.Id);
                }
            }
        }
    }
}
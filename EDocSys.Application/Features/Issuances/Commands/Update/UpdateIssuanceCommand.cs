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
        public string Title { get; set; }
        public string Category { get; set; }

        public string Body { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionNo { get; set; }
        public DateTime? EstalishedDate { get; set; }
        public int CompanyId { get; set; }
        public string Concurred1 { get; set; }
        public string Concurred2 { get; set; }
        public string ApprovedBy { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }
        public DateTime? ArchiveDate { get; set; }

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
                    issuance.Title = command.Title ?? issuance.Title;
                    issuance.Body = command.Body ?? issuance.Body;

                    issuance.EffectiveDate = command.EffectiveDate ?? issuance.EffectiveDate;
                    issuance.RevisionDate = command.RevisionDate ?? issuance.RevisionDate;
                    issuance.RevisionNo = command.RevisionNo ?? issuance.RevisionNo;
                    issuance.EstalishedDate = command.EstalishedDate ?? issuance.EstalishedDate;

                    issuance.CompanyId = (command.CompanyId == 0) ? issuance.CompanyId : command.CompanyId;

                    issuance.Concurred1 = command.Concurred1 ?? issuance.Concurred1;
                    issuance.Concurred2 = command.Concurred2;
                    issuance.ApprovedBy = command.ApprovedBy ?? issuance.ApprovedBy;

                    issuance.PreparedBy = command.PreparedBy ?? issuance.PreparedBy;
                    issuance.PreparedByDate = command.PreparedByDate ?? issuance.PreparedByDate;
                    issuance.PreparedByPosition = command.PreparedByPosition ?? issuance.PreparedByPosition;

                    issuance.IsActive = command.IsActive;
                    issuance.IsArchive = command.IsArchive ? true : issuance.IsArchive;
                    issuance.ArchiveId = (command.ArchiveId == 0) ? issuance.ArchiveId : command.ArchiveId;
                    issuance.PrintCount = (command.PrintCount == 0) ? issuance.PrintCount : command.PrintCount;
                    issuance.ArchiveDate = command.ArchiveDate ?? issuance.ArchiveDate;
                    await _issuanceRepository.UpdateAsync(issuance);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuance.Id);
                }
            }
        }
    }
}
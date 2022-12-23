using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceInfoCommand : IRequest<Result<int>>
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

        public class UpdateIssuanceInfoCommandHandler : IRequestHandler<UpdateIssuanceInfoCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceInfoRepository _issuanceInfoRepository;

            public UpdateIssuanceInfoCommandHandler(IIssuanceInfoRepository issuanceInfoRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoRepository = issuanceInfoRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceInfoCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoRepository.GetByIdAsync(command.Id);

                if (issuanceInfo == null)
                {
                    return Result<int>.Fail($"Issuance Info Not Found.");
                }
                else
                {
                    issuanceInfo.Title = command.Title ?? issuanceInfo.Title;
                    issuanceInfo.Body = command.Body ?? issuanceInfo.Body;

                    issuanceInfo.EffectiveDate = command.EffectiveDate ?? issuanceInfo.EffectiveDate;
                    issuanceInfo.RevisionDate = command.RevisionDate ?? issuanceInfo.RevisionDate;
                    issuanceInfo.RevisionNo = command.RevisionNo ?? issuanceInfo.RevisionNo;
                    issuanceInfo.EstalishedDate = command.EstalishedDate ?? issuanceInfo.EstalishedDate;

                    issuanceInfo.CompanyId = (command.CompanyId == 0) ? issuanceInfo.CompanyId : command.CompanyId;

                    issuanceInfo.Concurred1 = command.Concurred1 ?? issuanceInfo.Concurred1;
                    issuanceInfo.Concurred2 = command.Concurred2;
                    issuanceInfo.ApprovedBy = command.ApprovedBy ?? issuanceInfo.ApprovedBy;

                    issuanceInfo.PreparedBy = command.PreparedBy ?? issuanceInfo.PreparedBy;
                    issuanceInfo.PreparedByDate = command.PreparedByDate ?? issuanceInfo.PreparedByDate;
                    issuanceInfo.PreparedByPosition = command.PreparedByPosition ?? issuanceInfo.PreparedByPosition;

                    issuanceInfo.IsActive = command.IsActive;
                    issuanceInfo.IsArchive = command.IsArchive ? true : issuanceInfo.IsArchive;
                    issuanceInfo.ArchiveId = (command.ArchiveId == 0) ? issuanceInfo.ArchiveId : command.ArchiveId;
                    issuanceInfo.PrintCount = (command.PrintCount == 0) ? issuanceInfo.PrintCount : command.PrintCount;
                    issuanceInfo.ArchiveDate = command.ArchiveDate ?? issuanceInfo.ArchiveDate;
                    await _issuanceInfoRepository.UpdateAsync(issuanceInfo);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuanceInfo.Id);
                }
            }
        }
    }
}
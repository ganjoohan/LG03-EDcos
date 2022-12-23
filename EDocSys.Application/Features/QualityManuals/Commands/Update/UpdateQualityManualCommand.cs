using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.QualityManuals.Commands.Update
{
    public class UpdateQualityManualCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }      
        public string Title { get; set; }
        public string Category { get; set; }
        public string SectionNo { get; set; }
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

        public class UpdateQualityManualCommandHandler : IRequestHandler<UpdateQualityManualCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IQualityManualRepository _qualityManualRepository;

            public UpdateQualityManualCommandHandler(IQualityManualRepository qualityManualRepository, IUnitOfWork unitOfWork)
            {
                _qualityManualRepository = qualityManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateQualityManualCommand command, CancellationToken cancellationToken)
            {
                var qualitymanual = await _qualityManualRepository.GetByIdAsync(command.Id);

                if (qualitymanual == null)
                {
                    return Result<int>.Fail($"Quality Manual Not Found.");
                }
                else
                {
                    qualitymanual.Title = command.Title ?? qualitymanual.Title;
                    qualitymanual.Body = command.Body ?? qualitymanual.Body;
                    qualitymanual.SectionNo = command.SectionNo ?? qualitymanual.SectionNo;
                    qualitymanual.EffectiveDate = command.EffectiveDate ?? qualitymanual.EffectiveDate;
                    qualitymanual.RevisionDate = command.RevisionDate ?? qualitymanual.RevisionDate;
                    qualitymanual.RevisionNo = command.RevisionNo ?? qualitymanual.RevisionNo;
                    qualitymanual.EstalishedDate = command.EstalishedDate ?? qualitymanual.EstalishedDate;

                    qualitymanual.CompanyId = (command.CompanyId == 0) ? qualitymanual.CompanyId : command.CompanyId;

                    qualitymanual.Concurred1 = command.Concurred1 ?? qualitymanual.Concurred1;
                    qualitymanual.Concurred2 = command.Concurred2;
                    qualitymanual.ApprovedBy = command.ApprovedBy ?? qualitymanual.ApprovedBy;

                    qualitymanual.PreparedBy = command.PreparedBy ?? qualitymanual.PreparedBy;
                    qualitymanual.PreparedByDate = command.PreparedByDate ?? qualitymanual.PreparedByDate;
                    qualitymanual.PreparedByPosition = command.PreparedByPosition ?? qualitymanual.PreparedByPosition;

                    qualitymanual.IsActive = command.IsActive;
                    qualitymanual.IsArchive = command.IsArchive ? true : qualitymanual.IsArchive;
                    qualitymanual.ArchiveId = (command.ArchiveId == 0) ? qualitymanual.ArchiveId : command.ArchiveId;
                    qualitymanual.PrintCount = (command.PrintCount == 0) ? qualitymanual.PrintCount : command.PrintCount;
                    qualitymanual.ArchiveDate = command.ArchiveDate ?? qualitymanual.ArchiveDate;
                    await _qualityManualRepository.UpdateAsync(qualitymanual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(qualitymanual.Id);
                }
            }
        }
    }
}
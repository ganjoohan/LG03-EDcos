using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.LabAccreditationManuals.Commands.Update
{
    public class UpdateLabAccreditationManualCommand : IRequest<Result<int>>
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

        public class UpdateLabAccreditationManualCommandHandler : IRequestHandler<UpdateLabAccreditationManualCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILabAccreditationManualRepository _labAccreditationManualRepository;

            public UpdateLabAccreditationManualCommandHandler(ILabAccreditationManualRepository labAccreditationManualRepository, IUnitOfWork unitOfWork)
            {
                _labAccreditationManualRepository = labAccreditationManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateLabAccreditationManualCommand command, CancellationToken cancellationToken)
            {
                var labAccreditationmanual = await _labAccreditationManualRepository.GetByIdAsync(command.Id);

                if (labAccreditationmanual == null)
                {
                    return Result<int>.Fail($"Lab Accreditation Manual Not Found.");
                }
                else
                {
                    labAccreditationmanual.Title = command.Title ?? labAccreditationmanual.Title;
                    labAccreditationmanual.Body = command.Body ?? labAccreditationmanual.Body;

                    labAccreditationmanual.EffectiveDate = command.EffectiveDate ?? labAccreditationmanual.EffectiveDate;
                    labAccreditationmanual.RevisionDate = command.RevisionDate ?? labAccreditationmanual.RevisionDate;
                    labAccreditationmanual.RevisionNo = command.RevisionNo ?? labAccreditationmanual.RevisionNo;
                    labAccreditationmanual.EstalishedDate = command.EstalishedDate ?? labAccreditationmanual.EstalishedDate;

                    labAccreditationmanual.CompanyId = (command.CompanyId == 0) ? labAccreditationmanual.CompanyId : command.CompanyId;

                    labAccreditationmanual.Concurred1 = command.Concurred1 ?? labAccreditationmanual.Concurred1;
                    labAccreditationmanual.Concurred2 = command.Concurred2;
                    labAccreditationmanual.ApprovedBy = command.ApprovedBy ?? labAccreditationmanual.ApprovedBy;

                    labAccreditationmanual.PreparedBy = command.PreparedBy ?? labAccreditationmanual.PreparedBy;
                    labAccreditationmanual.PreparedByDate = command.PreparedByDate ?? labAccreditationmanual.PreparedByDate;
                    labAccreditationmanual.PreparedByPosition = command.PreparedByPosition ?? labAccreditationmanual.PreparedByPosition;

                    labAccreditationmanual.IsActive = command.IsActive;
                    labAccreditationmanual.IsArchive = command.IsArchive ? true : labAccreditationmanual.IsArchive;
                    labAccreditationmanual.ArchiveId = (command.ArchiveId == 0) ? labAccreditationmanual.ArchiveId : command.ArchiveId;
                    labAccreditationmanual.PrintCount = (command.PrintCount == 0) ? labAccreditationmanual.PrintCount : command.PrintCount;
                    labAccreditationmanual.ArchiveDate = command.ArchiveDate ?? labAccreditationmanual.ArchiveDate;
                    await _labAccreditationManualRepository.UpdateAsync(labAccreditationmanual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(labAccreditationmanual.Id);
                }
            }
        }
    }
}
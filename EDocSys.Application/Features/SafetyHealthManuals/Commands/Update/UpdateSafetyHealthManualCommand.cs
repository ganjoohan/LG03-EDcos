using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.SafetyHealthManuals.Commands.Update
{
    public class UpdateSafetyHealthManualCommand : IRequest<Result<int>>
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

        public class UpdateSafetyHealthManualCommandHandler : IRequestHandler<UpdateSafetyHealthManualCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ISafetyHealthManualRepository _safetyHealthManualRepository;

            public UpdateSafetyHealthManualCommandHandler(ISafetyHealthManualRepository safetyHealthManualRepository, IUnitOfWork unitOfWork)
            {
                _safetyHealthManualRepository = safetyHealthManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateSafetyHealthManualCommand command, CancellationToken cancellationToken)
            {
                var safetyHealthmanual = await _safetyHealthManualRepository.GetByIdAsync(command.Id);

                if (safetyHealthmanual == null)
                {
                    return Result<int>.Fail($"Safety and Health Manual Not Found.");
                }
                else
                {
                    safetyHealthmanual.Title = command.Title ?? safetyHealthmanual.Title;
                    safetyHealthmanual.Body = command.Body ?? safetyHealthmanual.Body;

                    safetyHealthmanual.EffectiveDate = command.EffectiveDate ?? safetyHealthmanual.EffectiveDate;
                    safetyHealthmanual.RevisionDate = command.RevisionDate ?? safetyHealthmanual.RevisionDate;
                    safetyHealthmanual.RevisionNo = command.RevisionNo ?? safetyHealthmanual.RevisionNo;
                    safetyHealthmanual.EstalishedDate = command.EstalishedDate ?? safetyHealthmanual.EstalishedDate;

                    safetyHealthmanual.CompanyId = (command.CompanyId == 0) ? safetyHealthmanual.CompanyId : command.CompanyId;

                    safetyHealthmanual.Concurred1 = command.Concurred1 ?? safetyHealthmanual.Concurred1;
                    safetyHealthmanual.Concurred2 = command.Concurred2;
                    safetyHealthmanual.ApprovedBy = command.ApprovedBy ?? safetyHealthmanual.ApprovedBy;

                    safetyHealthmanual.PreparedBy = command.PreparedBy ?? safetyHealthmanual.PreparedBy;
                    safetyHealthmanual.PreparedByDate = command.PreparedByDate ?? safetyHealthmanual.PreparedByDate;
                    safetyHealthmanual.PreparedByPosition = command.PreparedByPosition ?? safetyHealthmanual.PreparedByPosition;

                    safetyHealthmanual.IsActive = command.IsActive;
                    safetyHealthmanual.IsArchive = command.IsArchive ? true : safetyHealthmanual.IsArchive;
                    safetyHealthmanual.ArchiveId = (command.ArchiveId == 0) ? safetyHealthmanual.ArchiveId : command.ArchiveId;
                    safetyHealthmanual.PrintCount = (command.PrintCount == 0) ? safetyHealthmanual.PrintCount : command.PrintCount;
                    safetyHealthmanual.ArchiveDate = command.ArchiveDate ?? safetyHealthmanual.ArchiveDate;
                    await _safetyHealthManualRepository.UpdateAsync(safetyHealthmanual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(safetyHealthmanual.Id);
                }
            }
        }
    }
}
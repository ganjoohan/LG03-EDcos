using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;

namespace EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Update
{
    public class UpdateLionSteelCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string FormNo { get; set; }
        public string Title { get; set; }
        public string Section { get; set; }
        public string Type { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string Location { get; set; }
        public string RetentionPrd { get; set; }
        public string PIC { get; set; }
        public string FilingSystem { get; set; }
        public int RevisionNo { get; set; }
        public DateTime RevisionDate { get; set; }
        public string Body { get; set; }

        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public int PrintCount { get; set; }


        public class UpdateLionSteelCommandHandler : IRequestHandler<UpdateLionSteelCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILionSteelRepository _lionSteelRepository;

            public UpdateLionSteelCommandHandler(ILionSteelRepository lionSteelRepository, IUnitOfWork unitOfWork)
            {
                _lionSteelRepository = lionSteelRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateLionSteelCommand command, CancellationToken cancellationToken)
            {
                var lionSteel = await _lionSteelRepository.GetByIdAsync(command.Id);

                if (lionSteel == null)
                {
                    return Result<int>.Fail($"LionSteel Not Found.");
                }
                else
                {
                    lionSteel.Title = command.Title ?? lionSteel.Title;
                    lionSteel.Section = command.Section ?? lionSteel.Section;
                    lionSteel.Type = command.Type ?? lionSteel.Type;
                    lionSteel.CompanyId = (command.CompanyId == 0) ? lionSteel.CompanyId : command.CompanyId;

                    lionSteel.DepartmentId = (command.DepartmentId == 0) ? lionSteel.DepartmentId : command.DepartmentId;

                    lionSteel.Location = command.Location ?? lionSteel.Location;
                    lionSteel.RetentionPrd = command.RetentionPrd ?? lionSteel.RetentionPrd;
                    lionSteel.PIC = command.PIC ?? lionSteel.PIC;
                    lionSteel.RevisionNo = (command.RevisionNo == 0) ? lionSteel.RevisionNo : command.RevisionNo;
                    lionSteel.Body = command.Body ?? lionSteel.Body;

                    lionSteel.IsActive = command.IsActive;
                    lionSteel.IsArchive = command.IsArchive ? true : lionSteel.IsArchive;
                    lionSteel.ArchiveId = (command.ArchiveId == 0) ? lionSteel.ArchiveId : command.ArchiveId;
                    lionSteel.ArchiveDate = command.ArchiveDate ?? lionSteel.ArchiveDate;
                    lionSteel.PrintCount = (command.PrintCount == 0) ? lionSteel.PrintCount : command.PrintCount;
                    lionSteel.FilingSystem = command.FilingSystem ?? lionSteel.FilingSystem;
                    await _lionSteelRepository.UpdateAsync(lionSteel);
                    await _unitOfWork.CommitQuality(cancellationToken);
                    return Result<int>.Success(lionSteel.Id);
                }
            }
        }
    }
}
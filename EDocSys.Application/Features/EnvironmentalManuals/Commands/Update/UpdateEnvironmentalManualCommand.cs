using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.EnvironmentalManuals.Commands.Update
{
    public class UpdateEnvironmentalManualCommand : IRequest<Result<int>>
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

        public class UpdateEnvironmentalManualCommandHandler : IRequestHandler<UpdateEnvironmentalManualCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEnvironmentalManualRepository _environmentalManualRepository;

            public UpdateEnvironmentalManualCommandHandler(IEnvironmentalManualRepository environmentalManualRepository, IUnitOfWork unitOfWork)
            {
                _environmentalManualRepository = environmentalManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateEnvironmentalManualCommand command, CancellationToken cancellationToken)
            {
                var environmentalmanual = await _environmentalManualRepository.GetByIdAsync(command.Id);

                if (environmentalmanual == null)
                {
                    return Result<int>.Fail($"Environmental Manual Not Found.");
                }
                else
                {
                    environmentalmanual.Title = command.Title ?? environmentalmanual.Title;
                    environmentalmanual.Body = command.Body ?? environmentalmanual.Body;

                    environmentalmanual.EffectiveDate = command.EffectiveDate ?? environmentalmanual.EffectiveDate;
                    environmentalmanual.RevisionDate = command.RevisionDate ?? environmentalmanual.RevisionDate;
                    environmentalmanual.RevisionNo = command.RevisionNo ?? environmentalmanual.RevisionNo;
                    environmentalmanual.EstalishedDate = command.EstalishedDate ?? environmentalmanual.EstalishedDate;

                    environmentalmanual.CompanyId = (command.CompanyId == 0) ? environmentalmanual.CompanyId : command.CompanyId;

                    environmentalmanual.Concurred1 = command.Concurred1 ?? environmentalmanual.Concurred1;
                    environmentalmanual.Concurred2 = command.Concurred2;
                    environmentalmanual.ApprovedBy = command.ApprovedBy ?? environmentalmanual.ApprovedBy;

                    environmentalmanual.PreparedBy = command.PreparedBy ?? environmentalmanual.PreparedBy;
                    environmentalmanual.PreparedByDate = command.PreparedByDate ?? environmentalmanual.PreparedByDate;
                    environmentalmanual.PreparedByPosition = command.PreparedByPosition ?? environmentalmanual.PreparedByPosition;

                    environmentalmanual.IsActive = command.IsActive;
                    environmentalmanual.IsArchive = command.IsArchive ? true : environmentalmanual.IsArchive;
                    environmentalmanual.ArchiveId = (command.ArchiveId == 0) ? environmentalmanual.ArchiveId : command.ArchiveId;
                    environmentalmanual.PrintCount = (command.PrintCount == 0) ? environmentalmanual.PrintCount : command.PrintCount;
                    await _environmentalManualRepository.UpdateAsync(environmentalmanual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(environmentalmanual.Id);
                }
            }
        }
    }
}
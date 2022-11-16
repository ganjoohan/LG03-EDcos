using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.SOPs.Commands.Update
{
    public class UpdateSOPCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string WSCPNo { get; set; }
        public string Title { get; set; }
        public string SOPNo { get; set; }
        public string WINo { get; set; }
        public string Purpose { get; set; }
        public string PIC { get; set; }
        //public string Definition { get; set; }
        public string Body { get; set; }
        public int DepartmentId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionNo { get; set; }
        public DateTime? EstalishedDate { get; set; }
        public int CompanyId { get; set; }
        public bool hasWI { get; set; }
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
        public int WSCPId { get; set; }

        public class UpdateSOPCommandHandler : IRequestHandler<UpdateSOPCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ISOPRepository _sopRepository;

            public UpdateSOPCommandHandler(ISOPRepository sopRepository, IUnitOfWork unitOfWork)
            {
                _sopRepository = sopRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateSOPCommand command, CancellationToken cancellationToken)
            {
                var sop = await _sopRepository.GetByIdAsync(command.Id);

                if (sop == null)
                {
                    return Result<int>.Fail($"SOP Not Found.");
                }
                else
                {
                    sop.WSCPNo = command.WSCPNo ?? sop.WSCPNo;
                    sop.Title = command.Title ?? sop.Title;

                    sop.SOPNo = command.SOPNo ?? sop.SOPNo;
                    sop.WINo = command.WINo ?? sop.WINo;

                    sop.Purpose = command.Purpose ?? sop.Purpose;
                    sop.PIC = command.PIC ?? sop.PIC;
                    //sop.Definition = command.Definition ?? sop.Definition;
                    sop.Body = command.Body ?? sop.Body;

                    sop.EffectiveDate = command.EffectiveDate ?? sop.EffectiveDate;
                    sop.RevisionDate = command.RevisionDate ?? sop.RevisionDate;
                    sop.RevisionNo = command.RevisionNo ?? sop.RevisionNo;
                    sop.EstalishedDate = command.EstalishedDate ?? sop.EstalishedDate;

                    sop.DepartmentId = (command.DepartmentId == 0) ? sop.DepartmentId : command.DepartmentId;
                    sop.CompanyId = (command.CompanyId == 0) ? sop.CompanyId : command.CompanyId;

                    sop.hasWI = command.hasWI == false ? sop.hasWI : command.hasWI;

                    sop.Concurred1 = command.Concurred1 ?? sop.Concurred1;
                    sop.Concurred2 = command.Concurred2;
                    sop.ApprovedBy = command.ApprovedBy ?? sop.ApprovedBy;

                    sop.PreparedBy = command.PreparedBy ?? sop.PreparedBy;
                    sop.PreparedByDate = command.PreparedByDate ?? sop.PreparedByDate;
                    sop.PreparedByPosition = command.PreparedByPosition ?? sop.PreparedByPosition;
                    sop.IsActive = command.IsActive;
                    sop.IsArchive = command.IsArchive ? true : sop.IsArchive;
                    sop.ArchiveId = (command.ArchiveId == 0) ? sop.ArchiveId : command.ArchiveId;
                    sop.PrintCount = (command.PrintCount == 0) ? sop.PrintCount : command.PrintCount;
                    sop.WSCPId = (command.WSCPId == 0) ? sop.WSCPId : command.WSCPId;
                    sop.ArchiveDate = command.ArchiveDate ?? sop.ArchiveDate;
                    await _sopRepository.UpdateAsync(sop);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(sop.Id);
                }
            }
        }
    }
}
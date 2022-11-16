using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.WIs.Commands.Update
{
    public class UpdateWICommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string WSCPNo { get; set; }
        public string Title { get; set; }
        public string SOPNo { get; set; }
        public string WINo { get; set; }
        //public string Purpose { get; set; }
        //public string Scope { get; set; }
        //public string Definition { get; set; }
        public string Body { get; set; }
        public int DepartmentId { get; set; }
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
        public int WSCPId { get; set; }

        public int SOPId { get; set; }

        public class UpdateWICommandHandler : IRequestHandler<UpdateWICommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IWIRepository _wiRepository;

            public UpdateWICommandHandler(IWIRepository wiRepository, IUnitOfWork unitOfWork)
            {
                _wiRepository = wiRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateWICommand command, CancellationToken cancellationToken)
            {
                var wi = await _wiRepository.GetByIdAsync(command.Id);

                if (wi == null)
                {
                    return Result<int>.Fail($"WI Not Found.");
                }
                else
                {
                    wi.WSCPNo = command.WSCPNo ?? wi.WSCPNo;
                    wi.Title = command.Title ?? wi.Title;

                    wi.SOPNo = command.SOPNo ?? wi.SOPNo;
                    wi.WINo = command.WINo ?? wi.WINo;

                    //wi.Purpose = command.Purpose ?? wi.Purpose;
                    //wi.Scope = command.Scope ?? wi.Scope;
                    //wi.Definition = command.Definition ?? wi.Definition;
                    wi.Body = command.Body ?? wi.Body;

                    wi.EffectiveDate = command.EffectiveDate ?? wi.EffectiveDate;
                    wi.RevisionDate = command.RevisionDate ?? wi.RevisionDate;
                    wi.RevisionNo = command.RevisionNo ?? wi.RevisionNo;
                    wi.EstalishedDate = command.EstalishedDate ?? wi.EstalishedDate;

                    wi.DepartmentId = (command.DepartmentId == 0) ? wi.DepartmentId : command.DepartmentId;
                    wi.CompanyId = (command.CompanyId == 0) ? wi.CompanyId : command.CompanyId;

                    

                    wi.Concurred1 = command.Concurred1 ?? wi.Concurred1;
                    wi.Concurred2 = command.Concurred2;
                    wi.ApprovedBy = command.ApprovedBy ?? wi.ApprovedBy;

                    wi.PreparedBy = command.PreparedBy ?? wi.PreparedBy;
                    wi.PreparedByDate = command.PreparedByDate ?? wi.PreparedByDate;
                    wi.PreparedByPosition = command.PreparedByPosition ?? wi.PreparedByPosition;
                    wi.IsActive = command.IsActive;
                    wi.IsArchive = command.IsArchive ? true : wi.IsArchive;
                    wi.ArchiveId = (command.ArchiveId == 0) ? wi.ArchiveId : command.ArchiveId;
                    wi.PrintCount = (command.PrintCount == 0) ? wi.PrintCount : command.PrintCount;
                    wi.WSCPId = (command.WSCPId == 0) ? wi.WSCPId : command.WSCPId;
                    wi.SOPId = (command.SOPId == 0) ? wi.SOPId : command.SOPId;
                    wi.ArchiveDate = command.ArchiveDate ?? wi.ArchiveDate;
                    await _wiRepository.UpdateAsync(wi);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(wi.Id);
                }
            }
        }
    }
}
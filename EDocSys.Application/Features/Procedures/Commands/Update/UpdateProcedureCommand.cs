using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.Procedures.Commands.Update
{
    public class UpdateProcedureCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string WSCPNo { get; set; }
        public string Title { get; set; }
        public string SOPNo { get; set; }
        public string WINo { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public string Definition { get; set; }
        public string Body { get; set; }
        public int DepartmentId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionNo { get; set; }
        public DateTime? EstalishedDate { get; set; }
        public int CompanyId { get; set; }
        public bool hasSOP { get; set; }
        public string Concurred1 { get; set; }
        public string Concurred2 { get; set; }
        public string ApprovedBy { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }

        public class UpdateProcedureCommandHandler : IRequestHandler<UpdateProcedureCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IProcedureRepository _procedureRepository;

            public UpdateProcedureCommandHandler(IProcedureRepository procedureRepository, IUnitOfWork unitOfWork)
            {
                _procedureRepository = procedureRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateProcedureCommand command, CancellationToken cancellationToken)
            {
                var procedure = await _procedureRepository.GetByIdAsync(command.Id);

                if (procedure == null)
                {
                    return Result<int>.Fail($"Procedure Not Found.");
                }
                else
                {
                    procedure.WSCPNo = command.WSCPNo ?? procedure.WSCPNo;
                    procedure.Title = command.Title ?? procedure.Title;

                    procedure.SOPNo = command.SOPNo ?? procedure.SOPNo;
                    procedure.WINo = command.WINo ?? procedure.WINo;

                    procedure.Purpose = command.Purpose ?? procedure.Purpose;
                    procedure.Scope = command.Scope ?? procedure.Scope;
                    procedure.Definition = command.Definition ?? procedure.Definition;
                    procedure.Body = command.Body ?? procedure.Body;

                    procedure.EffectiveDate = command.EffectiveDate ?? procedure.EffectiveDate;
                    procedure.RevisionDate = command.RevisionDate ?? procedure.RevisionDate;
                    procedure.RevisionNo = command.RevisionNo ?? procedure.RevisionNo;
                    procedure.EstalishedDate = command.EstalishedDate ?? procedure.EstalishedDate;

                    procedure.DepartmentId = (command.DepartmentId == 0) ? procedure.DepartmentId : command.DepartmentId;
                    procedure.CompanyId = (command.CompanyId == 0) ? procedure.CompanyId : command.CompanyId;

                    procedure.hasSOP = command.hasSOP == false ? procedure.hasSOP : command.hasSOP;

                    procedure.Concurred1 = command.Concurred1 ?? procedure.Concurred1;
                    procedure.Concurred2 = command.Concurred2 ?? procedure.Concurred2;
                    procedure.ApprovedBy = command.ApprovedBy ?? procedure.ApprovedBy;

                    procedure.PreparedBy = command.PreparedBy ?? procedure.PreparedBy;
                    procedure.PreparedByDate = command.PreparedByDate ?? procedure.PreparedByDate;
                    procedure.PreparedByPosition = command.PreparedByPosition ?? procedure.PreparedByPosition;

                    await _procedureRepository.UpdateAsync(procedure);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(procedure.Id);
                }
            }
        }
    }
}
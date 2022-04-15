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

                    wi.Purpose = command.Purpose ?? wi.Purpose;
                    wi.Scope = command.Scope ?? wi.Scope;
                    wi.Definition = command.Definition ?? wi.Definition;
                    wi.Body = command.Body ?? wi.Body;

                    wi.EffectiveDate = command.EffectiveDate ?? wi.EffectiveDate;
                    wi.RevisionDate = command.RevisionDate ?? wi.RevisionDate;
                    wi.RevisionNo = command.RevisionNo ?? wi.RevisionNo;
                    wi.EstalishedDate = command.EstalishedDate ?? wi.EstalishedDate;

                    wi.DepartmentId = (command.DepartmentId == 0) ? wi.DepartmentId : command.DepartmentId;
                    wi.CompanyId = (command.CompanyId == 0) ? wi.CompanyId : command.CompanyId;
                    
                    await _wiRepository.UpdateAsync(wi);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(wi.Id);
                }
            }
        }
    }
}
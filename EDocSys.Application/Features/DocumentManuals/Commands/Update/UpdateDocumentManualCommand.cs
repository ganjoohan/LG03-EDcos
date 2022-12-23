using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.DocumentManuals.Commands.Update
{
    public class UpdateDocumentManualCommand : IRequest<Result<int>>
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

        public class UpdateDocumentManualCommandHandler : IRequestHandler<UpdateDocumentManualCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IDocumentManualRepository _documentManualRepository;

            public UpdateDocumentManualCommandHandler(IDocumentManualRepository documentManualRepository, IUnitOfWork unitOfWork)
            {
                _documentManualRepository = documentManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateDocumentManualCommand command, CancellationToken cancellationToken)
            {
                var documentmanual = await _documentManualRepository.GetByIdAsync(command.Id);

                if (documentmanual == null)
                {
                    return Result<int>.Fail($"Document Manual Not Found.");
                }
                else
                {
                    documentmanual.Title = command.Title ?? documentmanual.Title;
                    documentmanual.Body = command.Body ?? documentmanual.Body;

                    documentmanual.EffectiveDate = command.EffectiveDate ?? documentmanual.EffectiveDate;
                    documentmanual.RevisionDate = command.RevisionDate ?? documentmanual.RevisionDate;
                    documentmanual.RevisionNo = command.RevisionNo ?? documentmanual.RevisionNo;
                    documentmanual.EstalishedDate = command.EstalishedDate ?? documentmanual.EstalishedDate;

                    documentmanual.CompanyId = (command.CompanyId == 0) ? documentmanual.CompanyId : command.CompanyId;

                    documentmanual.Concurred1 = command.Concurred1 ?? documentmanual.Concurred1;
                    documentmanual.Concurred2 = command.Concurred2;
                    documentmanual.ApprovedBy = command.ApprovedBy ?? documentmanual.ApprovedBy;

                    documentmanual.PreparedBy = command.PreparedBy ?? documentmanual.PreparedBy;
                    documentmanual.PreparedByDate = command.PreparedByDate ?? documentmanual.PreparedByDate;
                    documentmanual.PreparedByPosition = command.PreparedByPosition ?? documentmanual.PreparedByPosition;

                    documentmanual.IsActive = command.IsActive;
                    documentmanual.IsArchive = command.IsArchive ? true : documentmanual.IsArchive;
                    documentmanual.ArchiveId = (command.ArchiveId == 0) ? documentmanual.ArchiveId : command.ArchiveId;
                    documentmanual.PrintCount = (command.PrintCount == 0) ? documentmanual.PrintCount : command.PrintCount;
                    documentmanual.ArchiveDate = command.ArchiveDate ?? documentmanual.ArchiveDate;
                    await _documentManualRepository.UpdateAsync(documentmanual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(documentmanual.Id);
                }
            }
        }
    }
}
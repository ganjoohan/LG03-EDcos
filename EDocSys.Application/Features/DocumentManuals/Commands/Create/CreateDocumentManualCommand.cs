using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.DocumentManuals.Commands.Create
{
    public partial class CreateDocumentManualCommand : IRequest<Result<int>>
    {
        public string DOCNo { get; set; }
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
        public bool IsActive { get; set; } = true;
        public bool IsArchive { get; set; } = false;
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; } = 0;
    }

    public class CreateDocumentManualCommandHandler : IRequestHandler<CreateDocumentManualCommand, Result<int>>
    {
        private readonly IDocumentManualRepository _documentManualRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateDocumentManualCommandHandler(IDocumentManualRepository documentManualRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _documentManualRepository = documentManualRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateDocumentManualCommand request, CancellationToken cancellationToken)
        {
            var documentManual = _mapper.Map<DocumentManual>(request);
            await _documentManualRepository.InsertAsync(documentManual);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(documentManual.Id);
        }
    }
}
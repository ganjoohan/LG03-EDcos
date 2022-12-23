using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.SafetyHealthManuals.Commands.Create
{
    public partial class CreateSafetyHealthManualCommand : IRequest<Result<int>>
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
        public DateTime? ArchiveDate { get; set; }
    }

    public class CreateSafetyHealthManualCommandHandler : IRequestHandler<CreateSafetyHealthManualCommand, Result<int>>
    {
        private readonly ISafetyHealthManualRepository _safetyHealthManualRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSafetyHealthManualCommandHandler(ISafetyHealthManualRepository safetyHealthManualRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _safetyHealthManualRepository = safetyHealthManualRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateSafetyHealthManualCommand request, CancellationToken cancellationToken)
        {
            var safetyHealthManual = _mapper.Map<SafetyHealthManual>(request);
            await _safetyHealthManualRepository.InsertAsync(safetyHealthManual);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(safetyHealthManual.Id);
        }
    }
}
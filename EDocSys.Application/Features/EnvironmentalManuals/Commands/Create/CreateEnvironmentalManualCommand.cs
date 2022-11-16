using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.EnvironmentalManuals.Commands.Create
{
    public partial class CreateEnvironmentalManualCommand : IRequest<Result<int>>
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

    public class CreateEnvironmentalManualCommandHandler : IRequestHandler<CreateEnvironmentalManualCommand, Result<int>>
    {
        private readonly IEnvironmentalManualRepository _environmentalManualRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateEnvironmentalManualCommandHandler(IEnvironmentalManualRepository environmentalManualRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _environmentalManualRepository = environmentalManualRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateEnvironmentalManualCommand request, CancellationToken cancellationToken)
        {
            var environmentalManual = _mapper.Map<EnvironmentalManual>(request);
            await _environmentalManualRepository.InsertAsync(environmentalManual);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(environmentalManual.Id);
        }
    }
}
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.Issuances.Commands.Create
{
    public partial class CreateIssuanceCommand : IRequest<Result<int>>
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

    public class CreateIssuanceCommandHandler : IRequestHandler<CreateIssuanceCommand, Result<int>>
    {
        private readonly IIssuanceRepository _issuanceRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateIssuanceCommandHandler(IIssuanceRepository issuanceRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _issuanceRepository = issuanceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateIssuanceCommand request, CancellationToken cancellationToken)
        {
            var issuance = _mapper.Map<Issuance>(request);
            await _issuanceRepository.InsertAsync(issuance);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(issuance.Id);
        }
    }
}
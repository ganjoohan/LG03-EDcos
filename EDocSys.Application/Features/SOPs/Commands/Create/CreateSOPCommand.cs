using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.SOPs.Commands.Create
{
    public partial class CreateSOPCommand : IRequest<Result<int>>
    {
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
        public int WSCPId { get; set; } = 0;
    }

    public class CreateSOPCommandHandler : IRequestHandler<CreateSOPCommand, Result<int>>
    {
        private readonly ISOPRepository _sopRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSOPCommandHandler(ISOPRepository sopRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _sopRepository = sopRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateSOPCommand request, CancellationToken cancellationToken)
        {
            var sop = _mapper.Map<SOP>(request);
            await _sopRepository.InsertAsync(sop);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(sop.Id);
        }
    }
}
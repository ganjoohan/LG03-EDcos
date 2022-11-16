using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using EDocSys.Domain.Entities.QualityRecord;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using EDocSys.Application.Interfaces.Repositories;

namespace EDocSys.Application.Features.QualityFeatures.LionSteels.Commands.Create
{
    public partial class CreateLionSteelCommand : IRequest<Result<int>>
    {
        public string FormNo { get; set; }
        public string Title { get; set; }
        public string Section { get; set; }
        public string Type { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string Location { get; set; }
        public string RetentionPrd { get; set; }
        public string PIC { get; set; }
        public string FilingSystem { get; set; }
        public int RevisionNo { get; set; }
        public DateTime RevisionDate { get; set; }
        public string Body { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsArchive { get; set; } = false;
        public int ArchiveId { get; set; }
        public DateTime ArchiveDate { get; set; }
        public int PrintCount { get; set; } = 0;
    }

    public class CreateLionSteelCommandHandler : IRequestHandler<CreateLionSteelCommand, Result<int>>
    {
        private readonly ILionSteelRepository _lionSteelRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateLionSteelCommandHandler(ILionSteelRepository lionSteelRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _lionSteelRepository = lionSteelRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateLionSteelCommand request, CancellationToken cancellationToken)
        {
            var lionSteel = _mapper.Map<LionSteel>(request);
            await _lionSteelRepository.InsertAsync(lionSteel);
            await _unitOfWork.CommitQuality (cancellationToken);
            return Result<int>.Success(lionSteel.Id);
        }
    }
}
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManualStatuses.Commands.Create
{
    public partial class CreateQualityManualStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int QualityManualId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateQualityManualStatusCommandHandler : IRequestHandler<CreateQualityManualStatusCommand, Result<int>>
    {
        private readonly IQualityManualStatusRepository _qualityManualStatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateQualityManualStatusCommandHandler(IQualityManualStatusRepository qualityManualstatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _qualityManualStatusRepository = qualityManualstatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateQualityManualStatusCommand request, CancellationToken cancellationToken)
        {
            var qualityManualStatus = _mapper.Map<QualityManualStatus>(request);
            await _qualityManualStatusRepository.InsertAsync(qualityManualStatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(qualityManualStatus.Id);
        }
    }
}

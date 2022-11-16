using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SafetyHealthManualStatuses.Commands.Create
{
    public partial class CreateSafetyHealthManualStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int SafetyHealthManualId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateSafetyHealthManualStatusCommandHandler : IRequestHandler<CreateSafetyHealthManualStatusCommand, Result<int>>
    {
        private readonly ISafetyHealthManualStatusRepository _safetyHealthManualStatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSafetyHealthManualStatusCommandHandler(ISafetyHealthManualStatusRepository safetyHealthManualstatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _safetyHealthManualStatusRepository = safetyHealthManualstatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateSafetyHealthManualStatusCommand request, CancellationToken cancellationToken)
        {
            var safetyHealthManualStatus = _mapper.Map<SafetyHealthManualStatus>(request);
            await _safetyHealthManualStatusRepository.InsertAsync(safetyHealthManualStatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(safetyHealthManualStatus.Id);
        }
    }
}

using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManualStatuses.Commands.Create
{
    public partial class CreateLabAccreditationManualStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int LabAccreditationManualId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateLabAccreditationManualStatusCommandHandler : IRequestHandler<CreateLabAccreditationManualStatusCommand, Result<int>>
    {
        private readonly ILabAccreditationManualStatusRepository _labAccreditationManualStatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateLabAccreditationManualStatusCommandHandler(ILabAccreditationManualStatusRepository labAccreditationManualstatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _labAccreditationManualStatusRepository = labAccreditationManualstatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateLabAccreditationManualStatusCommand request, CancellationToken cancellationToken)
        {
            var labAccreditationManualStatus = _mapper.Map<LabAccreditationManualStatus>(request);
            await _labAccreditationManualStatusRepository.InsertAsync(labAccreditationManualStatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(labAccreditationManualStatus.Id);
        }
    }
}

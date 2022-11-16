using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManualStatuses.Commands.Create
{
    public partial class CreateEnvironmentalManualStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int EnvironmentalManualId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateEnvironmentalManualStatusCommandHandler : IRequestHandler<CreateEnvironmentalManualStatusCommand, Result<int>>
    {
        private readonly IEnvironmentalManualStatusRepository _environmentalManualStatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateEnvironmentalManualStatusCommandHandler(IEnvironmentalManualStatusRepository environmentalManualstatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _environmentalManualStatusRepository = environmentalManualstatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateEnvironmentalManualStatusCommand request, CancellationToken cancellationToken)
        {
            var environmentalManualStatus = _mapper.Map<EnvironmentalManualStatus>(request);
            await _environmentalManualStatusRepository.InsertAsync(environmentalManualStatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(environmentalManualStatus.Id);
        }
    }
}

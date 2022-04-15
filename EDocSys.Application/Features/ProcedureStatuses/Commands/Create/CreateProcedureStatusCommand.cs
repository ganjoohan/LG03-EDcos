using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.ProcedureStatuses.Commands.Create
{
    public partial class CreateProcedureStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int ProcedureId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateProcedureStatusCommandHandler : IRequestHandler<CreateProcedureStatusCommand, Result<int>>
    {
        private readonly IProcedureStatusRepository _proceduretatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateProcedureStatusCommandHandler(IProcedureStatusRepository procedurestatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _proceduretatusRepository = procedurestatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateProcedureStatusCommand request, CancellationToken cancellationToken)
        {
            var procedurestatus = _mapper.Map<ProcedureStatus>(request);
            await _proceduretatusRepository.InsertAsync(procedurestatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(procedurestatus.Id);
        }
    }
}

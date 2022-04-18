using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SOPStatuses.Commands.Create
{
    public partial class CreateSOPStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int SOPId { get; set; }
        public int DocumentStatusId { get; set; }

    }

    public class CreateSOPStatusCommandHandler : IRequestHandler<CreateSOPStatusCommand, Result<int>>
    {
        private readonly ISOPStatusRepository _sopstatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSOPStatusCommandHandler(ISOPStatusRepository sopstatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _sopstatusRepository = sopstatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateSOPStatusCommand request, CancellationToken cancellationToken)
        {
            var sopstatus = _mapper.Map<SOPStatus>(request);
            await _sopstatusRepository.InsertAsync(sopstatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(sopstatus.Id);
        }
    }
}

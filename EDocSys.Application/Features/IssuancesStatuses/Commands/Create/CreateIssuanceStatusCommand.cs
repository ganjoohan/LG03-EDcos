using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.IssuanceStatuses.Commands.Create
{
    public partial class CreateIssuanceStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int IssuanceId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateIssuanceStatusCommandHandler : IRequestHandler<CreateIssuanceStatusCommand, Result<int>>
    {
        private readonly IIssuanceStatusRepository _issuanceStatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateIssuanceStatusCommandHandler(IIssuanceStatusRepository issuanceStatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _issuanceStatusRepository = issuanceStatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateIssuanceStatusCommand request, CancellationToken cancellationToken)
        {
            var docStatus = _mapper.Map<IssuanceStatus>(request);
            await _issuanceStatusRepository.InsertAsync(docStatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(docStatus.Id);
        }
    }
}

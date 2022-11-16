using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManualStatuses.Commands.Create
{
    public partial class CreateDocumentManualStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int DocumentManualId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateDocumentManualStatusCommandHandler : IRequestHandler<CreateDocumentManualStatusCommand, Result<int>>
    {
        private readonly IDocumentManualStatusRepository _documentManualStatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateDocumentManualStatusCommandHandler(IDocumentManualStatusRepository documentManualstatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _documentManualStatusRepository = documentManualstatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateDocumentManualStatusCommand request, CancellationToken cancellationToken)
        {
            var documentManualStatus = _mapper.Map<DocumentManualStatus>(request);
            await _documentManualStatusRepository.InsertAsync(documentManualStatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(documentManualStatus.Id);
        }
    }
}

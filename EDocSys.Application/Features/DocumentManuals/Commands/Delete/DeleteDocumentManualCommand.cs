using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManuals.Commands.Delete
{
    public class DeleteDocumentManualCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteDocumentManualCommandHandler : IRequestHandler<DeleteDocumentManualCommand, Result<int>>
        {
            private readonly IDocumentManualRepository _documentManualRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteDocumentManualCommandHandler(IDocumentManualRepository documentManualRepository, IUnitOfWork unitOfWork)
            {
                _documentManualRepository = documentManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteDocumentManualCommand command, CancellationToken cancellationToken)
            {
                var documentmanual = await _documentManualRepository.GetByIdAsync(command.Id);
                await _documentManualRepository.DeleteAsync(documentmanual);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(documentmanual.Id);
            }
        }
    }
}
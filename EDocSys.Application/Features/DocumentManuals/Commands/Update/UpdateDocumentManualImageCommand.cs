using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManuals.Commands.Update
{
    public class UpdateDocumentManualImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateDocumentManualImageCommandHandler : IRequestHandler<UpdateDocumentManualImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IDocumentManualRepository _documentManualRepository;

            public UpdateDocumentManualImageCommandHandler(IDocumentManualRepository documentManualRepository, IUnitOfWork unitOfWork)
            {
                _documentManualRepository = documentManualRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateDocumentManualImageCommand command, CancellationToken cancellationToken)
            {
                var docManual = await _documentManualRepository.GetByIdAsync(command.Id);

                if (docManual == null)
                {
                    throw new ApiException($"Document Manual Not Found.");
                }
                else
                {
                    // docManual.Image = command.Image;
                    await _documentManualRepository.UpdateAsync(docManual);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(docManual.Id);
                }
            }
        }
    }
}
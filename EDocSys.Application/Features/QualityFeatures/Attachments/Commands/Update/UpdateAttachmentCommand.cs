using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;

namespace EDocSys.Application.Features.QualityFeatures.Attachments.Commands.Update
{
    public class UpdateAttachmentCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNameBatch { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FileLoc { get; set; }
        public int DocId { get; set; }
        public string DocName { get; set; }
        public bool IsActive { get; set; }

        public class UpdateAttachmentCommandHandler : IRequestHandler<UpdateAttachmentCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IAttachmentRepository _attachmentRepository;

            public UpdateAttachmentCommandHandler(IAttachmentRepository attachmentRepository, IUnitOfWork unitOfWork)
            {
                _attachmentRepository = attachmentRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateAttachmentCommand command, CancellationToken cancellationToken)
            {
                var attachment = await _attachmentRepository.GetByIdAsync(command.Id);

                if (attachment == null)
                {
                    return Result<int>.Fail($"Attachment Not Found.");
                }
                else
                {
                    attachment.FileName = command.FileName ?? attachment.FileName;
                    attachment.FileNameBatch = command.FileNameBatch ?? attachment.FileNameBatch;
                    attachment.FileSize = command.FileSize;
                    attachment.FileType = command.FileType ?? attachment.FileType;
                    attachment.FileLoc = command.FileLoc ?? attachment.FileLoc;
                    attachment.DocId = (command.DocId == 0) ? attachment.DocId : command.DocId;
                    attachment.DocName = command.DocName ?? attachment.DocName;
                    attachment.IsActive = command.IsActive;
                    await _attachmentRepository.UpdateAsync(attachment);
                    await _unitOfWork.CommitQuality(cancellationToken);
                    return Result<int>.Success(attachment.Id);
                }
            }
        }
    }
}
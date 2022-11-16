using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using EDocSys.Domain.Entities.QualityRecord;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.DocumentationMaster;
using EDocSys.Application.Interfaces.Repositories;

namespace EDocSys.Application.Features.QualityFeatures.Attachments.Commands.Create
{
    public partial class CreateAttachmentCommand : IRequest<Result<int>>
    {
        public string FileName { get; set; }
        public string FileNameBatch { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FileLoc { get; set; }
        public int DocId { get; set; }
        public string DocName { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateAttachmentCommandHandler : IRequestHandler<CreateAttachmentCommand, Result<int>>
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateAttachmentCommandHandler(IAttachmentRepository attachmentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _attachmentRepository = attachmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateAttachmentCommand request, CancellationToken cancellationToken)
        {
            var attachment = _mapper.Map<Attachment>(request);
            await _attachmentRepository.InsertAsync(attachment);
            await _unitOfWork.CommitQuality(cancellationToken);
            return Result<int>.Success(attachment.Id);
        }
    }
}
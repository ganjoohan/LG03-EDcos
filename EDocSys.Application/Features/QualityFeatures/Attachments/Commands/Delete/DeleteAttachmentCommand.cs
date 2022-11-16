using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.Repositories;

namespace EDocSys.Application.Features.QualityFeatures.Attachments.Commands.Delete
{
    public class DeleteAttachmentCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result<int>>
        {
            private readonly IAttachmentRepository _departmentRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteAttachmentCommandHandler(IAttachmentRepository departmentRepository, IUnitOfWork unitOfWork)
            {
                _departmentRepository = departmentRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteAttachmentCommand command, CancellationToken cancellationToken)
            {
                var procedure = await _departmentRepository.GetByIdAsync(command.Id);
                await _departmentRepository.DeleteAsync(procedure);
                await _unitOfWork.CommitQuality(cancellationToken);
                return Result<int>.Success(procedure.Id);
            }
        }
    }
}
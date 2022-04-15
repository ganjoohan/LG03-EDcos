using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SOPs.Commands.Delete
{
    public class DeleteSOPCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteSOPCommandHandler : IRequestHandler<DeleteSOPCommand, Result<int>>
        {
            private readonly ISOPRepository _sopRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteSOPCommandHandler(ISOPRepository sopRepository, IUnitOfWork unitOfWork)
            {
                _sopRepository = sopRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteSOPCommand command, CancellationToken cancellationToken)
            {
                var sop = await _sopRepository.GetByIdAsync(command.Id);
                await _sopRepository.DeleteAsync(sop);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(sop.Id);
            }
        }
    }
}
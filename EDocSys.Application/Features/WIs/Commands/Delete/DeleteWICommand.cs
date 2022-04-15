using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.WIs.Commands.Delete
{
    public class DeleteWICommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteWICommandHandler : IRequestHandler<DeleteWICommand, Result<int>>
        {
            private readonly IWIRepository _wiRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteWICommandHandler(IWIRepository wiRepository, IUnitOfWork unitOfWork)
            {
                _wiRepository = wiRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteWICommand command, CancellationToken cancellationToken)
            {
                var wi = await _wiRepository.GetByIdAsync(command.Id);
                await _wiRepository.DeleteAsync(wi);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(wi.Id);
            }
        }
    }
}
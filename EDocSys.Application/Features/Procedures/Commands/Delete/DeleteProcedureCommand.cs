using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Procedures.Commands.Delete
{
    public class DeleteProcedureCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteProcedureCommandHandler : IRequestHandler<DeleteProcedureCommand, Result<int>>
        {
            private readonly IProcedureRepository _procedureRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteProcedureCommandHandler(IProcedureRepository procedureRepository, IUnitOfWork unitOfWork)
            {
                _procedureRepository = procedureRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteProcedureCommand command, CancellationToken cancellationToken)
            {
                var procedure = await _procedureRepository.GetByIdAsync(command.Id);
                await _procedureRepository.DeleteAsync(procedure);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(procedure.Id);
            }
        }
    }
}
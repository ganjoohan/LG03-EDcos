using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Procedures.Commands.Update
{
    public class UpdateProcedureImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateProcedureImageCommandHandler : IRequestHandler<UpdateProcedureImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IProcedureRepository _procedureRepository;

            public UpdateProcedureImageCommandHandler(IProcedureRepository procedureRepository, IUnitOfWork unitOfWork)
            {
                _procedureRepository = procedureRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateProcedureImageCommand command, CancellationToken cancellationToken)
            {
                var procedure = await _procedureRepository.GetByIdAsync(command.Id);

                if (procedure == null)
                {
                    throw new ApiException($"Procedure Not Found.");
                }
                else
                {
                    // procedure.Image = command.Image;
                    await _procedureRepository.UpdateAsync(procedure);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(procedure.Id);
                }
            }
        }
    }
}
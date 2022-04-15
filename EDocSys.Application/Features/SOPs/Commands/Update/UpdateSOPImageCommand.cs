using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SOPs.Commands.Update
{
    public class UpdateSOPImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateSOPImageCommandHandler : IRequestHandler<UpdateSOPImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ISOPRepository _sopRepository;

            public UpdateSOPImageCommandHandler(ISOPRepository sopRepository, IUnitOfWork unitOfWork)
            {
                _sopRepository = sopRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateSOPImageCommand command, CancellationToken cancellationToken)
            {
                var sop = await _sopRepository.GetByIdAsync(command.Id);

                if (sop == null)
                {
                    throw new ApiException($"SOP Not Found.");
                }
                else
                {
                    // sop.Image = command.Image;
                    await _sopRepository.UpdateAsync(sop);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(sop.Id);
                }
            }
        }
    }
}
using EDocSys.Application.Exceptions;
using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.WIs.Commands.Update
{
    public class UpdateWIImageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public class UpdateWIImageCommandHandler : IRequestHandler<UpdateWIImageCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IWIRepository _wiRepository;

            public UpdateWIImageCommandHandler(IWIRepository wiRepository, IUnitOfWork unitOfWork)
            {
                _wiRepository = wiRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateWIImageCommand command, CancellationToken cancellationToken)
            {
                var wi = await _wiRepository.GetByIdAsync(command.Id);

                if (wi == null)
                {
                    throw new ApiException($"WI Not Found.");
                }
                else
                {
                    // wi.Image = command.Image;
                    await _wiRepository.UpdateAsync(wi);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(wi.Id);
                }
            }
        }
    }
}
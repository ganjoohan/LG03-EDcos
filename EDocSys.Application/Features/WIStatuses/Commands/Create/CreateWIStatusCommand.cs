using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.WIStatuses.Commands.Create
{
    public partial class CreateWIStatusCommand : IRequest<Result<int>>
    {
        public string Remarks { get; set; }
        public int WIId { get; set; }
        public int DocumentStatusId { get; set; }
        
    }

    public class CreateWIStatusCommandHandler : IRequestHandler<CreateWIStatusCommand, Result<int>>
    {
        private readonly IWIStatusRepository _wistatusRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateWIStatusCommandHandler(IWIStatusRepository wistatusRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _wistatusRepository = wistatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateWIStatusCommand request, CancellationToken cancellationToken)
        {
            var wistatus = _mapper.Map<WIStatus>(request);
            await _wistatusRepository.InsertAsync(wistatus);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(wistatus.Id);
        }
    }
}

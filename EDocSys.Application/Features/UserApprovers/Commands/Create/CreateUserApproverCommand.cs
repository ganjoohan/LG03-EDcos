using AspNetCoreHero.Results;
using AutoMapper;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.UserMaster;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace EDocSys.Application.Features.UserApprovers.Commands.Create
{
    public partial class CreateUserApproverCommand : IRequest<Result<int>>
    {
        public string UserId { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string ApprovalType { get; set; }
    }

    public class CreateUserApproverCommandHandler : IRequestHandler<CreateUserApproverCommand, Result<int>>
    {
        private readonly IUserApproverRepository _userapproverRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateUserApproverCommandHandler(IUserApproverRepository userapproverRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userapproverRepository = userapproverRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateUserApproverCommand request, CancellationToken cancellationToken)
        {
            var userapprover = _mapper.Map<UserApprover>(request);
            await _userapproverRepository.InsertAsync(userapprover);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(userapprover.Id);
        }
    }
}

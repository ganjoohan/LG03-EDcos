using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.DocumentationMaster;

namespace EDocSys.Application.Features.Companies.Commands.Create
{
    public partial class CreateCompanyCommand : IRequest<Result<int>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<int>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var procedure = _mapper.Map<Company>(request);
            await _companyRepository.InsertAsync(procedure);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(procedure.Id);
        }
    }
}
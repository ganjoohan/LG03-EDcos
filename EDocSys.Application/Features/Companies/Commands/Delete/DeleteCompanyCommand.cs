using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Companies.Commands.Delete
{
    public class DeleteCompanyCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<int>>
        {
            private readonly ICompanyRepository _companyRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
            {
                _companyRepository = companyRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
            {
                var procedure = await _companyRepository.GetByIdAsync(command.Id);
                await _companyRepository.DeleteAsync(procedure);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(procedure.Id);
            }
        }
    }
}
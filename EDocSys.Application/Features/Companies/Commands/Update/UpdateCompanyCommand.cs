using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Companies.Commands.Update
{
    public class UpdateCompanyCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public class UpdateProductCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICompanyRepository _companyRepository;

            public UpdateProductCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
            {
                _companyRepository = companyRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
            {
                var company = await _companyRepository.GetByIdAsync(command.Id);

                if (company == null)
                {
                    return Result<int>.Fail($"Company Not Found.");
                }
                else
                {
                    company.Name = command.Name ?? company.Name;
                    company.Description = command.Description ?? company.Description;
                    await _companyRepository.UpdateAsync(company);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(company.Id);
                }
            }
        }
    }
}
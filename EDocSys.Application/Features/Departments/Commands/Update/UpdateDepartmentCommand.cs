using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Departments.Commands.Update
{
    public class UpdateDepartmentCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InformedList { get; set; }

        public class UpdateProductCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IDepartmentRepository _departmentRepository;

            public UpdateProductCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
            {
                _departmentRepository = departmentRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
            {
                var department = await _departmentRepository.GetByIdAsync(command.Id);

                if (department == null)
                {
                    return Result<int>.Fail($"Department Not Found.");
                }
                else
                {
                    department.Name = command.Name ?? department.Name;
                    department.Description = command.Description ?? department.Description;
                    department.InformedList = command.InformedList ?? department.InformedList;
                    await _departmentRepository.UpdateAsync(department);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(department.Id);
                }
            }
        }
    }
}
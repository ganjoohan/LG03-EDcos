using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.Procedures.Commands.Create
{
    public partial class CreateProcedureCommand : IRequest<Result<int>>
    {
        public string WSCPNo { get; set; }
        public string Title { get; set; }
        public string SOPNo { get; set; }
        public string WINo { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public string Definition { get; set; }
        public string Body { get; set; }
        public int DepartmentId { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionNo { get; set; }
        public DateTime? EstalishedDate { get; set; }

        public int CompanyId { get; set; }

        public string Concurred1 { get; set; }
        public string Concurred2 { get; set; }
        public string ApprovedBy { get; set; }

        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }
    }

    public class CreateProcedureCommandHandler : IRequestHandler<CreateProcedureCommand, Result<int>>
    {
        private readonly IProcedureRepository _procedureRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateProcedureCommandHandler(IProcedureRepository procedureRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _procedureRepository = procedureRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateProcedureCommand request, CancellationToken cancellationToken)
        {
            var procedure = _mapper.Map<Procedure>(request);
            await _procedureRepository.InsertAsync(procedure);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(procedure.Id);
        }
    }
}
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.WIs.Commands.Create
{
    public partial class CreateWICommand : IRequest<Result<int>>
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
    }

    public class CreateWICommandHandler : IRequestHandler<CreateWICommand, Result<int>>
    {
        private readonly IWIRepository _sopRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateWICommandHandler(IWIRepository sopRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _sopRepository = sopRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateWICommand request, CancellationToken cancellationToken)
        {
            var sop = _mapper.Map<WI>(request);
            await _sopRepository.InsertAsync(sop);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(sop.Id);
        }
    }
}
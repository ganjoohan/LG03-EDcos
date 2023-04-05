using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace EDocSys.Application.Features.Issuances.Commands.Create
{
    public partial class CreateIssuanceInfoPrintCommand : IRequest<Result<int>>
    {
        public int IssInfoId { get; set; }
        public string RecipientName { get; set; }
        public bool IsPrinted { get; set; }
        public DateTime? PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string ReturnedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateIssuanceInfoPrintCommandHandler : IRequestHandler<CreateIssuanceInfoPrintCommand, Result<int>>
    {
        private readonly IIssuanceInfoPrintRepository _issuanceInfoPrintRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateIssuanceInfoPrintCommandHandler(IIssuanceInfoPrintRepository issuanceInfoPrintRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _issuanceInfoPrintRepository = issuanceInfoPrintRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateIssuanceInfoPrintCommand request, CancellationToken cancellationToken)
        {
            var issuanceInfoPrint = _mapper.Map<IssuanceInfoPrint>(request);
            await _issuanceInfoPrintRepository.InsertAsync(issuanceInfoPrint);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(issuanceInfoPrint.Id);
        }
    }
}
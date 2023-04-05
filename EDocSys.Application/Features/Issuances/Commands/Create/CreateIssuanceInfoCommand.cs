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
    public partial class CreateIssuanceInfoCommand : IRequest<Result<int>>
    {
        public int HId { get; set; }
        public int No { get; set; }
        public string DOCId { get; set; }
        public string DocType { get; set; }
        public string RecipientName1 { get; set; }
        public string RecipientName2 { get; set; }
        public string RecipientName3 { get; set; }
        public string RecipientName4 { get; set; }
        public string RecipientName5 { get; set; }
        public string RecipientName6 { get; set; }
        public string Purpose { get; set; }
        public string Amendment { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAmend { get; set; } = false;
    }

    public class CreateIssuanceInfoCommandHandler : IRequestHandler<CreateIssuanceInfoCommand, Result<int>>
    {
        private readonly IIssuanceInfoRepository _issuanceInfoRepository;
        private readonly IMapper _mapper;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateIssuanceInfoCommandHandler(IIssuanceInfoRepository issuanceInfoRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _issuanceInfoRepository = issuanceInfoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateIssuanceInfoCommand request, CancellationToken cancellationToken)
        {
            var issuanceInfo = _mapper.Map<IssuanceInfo>(request);
            await _issuanceInfoRepository.InsertAsync(issuanceInfo);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(issuanceInfo.Id);
        }
    }
}
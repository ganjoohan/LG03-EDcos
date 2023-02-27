using EDocSys.Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EDocSys.Application.Features.Issuances.Commands.Update
{
    public class UpdateIssuanceInfoCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int HId { get; set; }
        public int No { get; set; }
        public string DOCNo { get; set; }
        public string DocType { get; set; }
        public string RecipientName1 { get; set; }
        public string RecipientName2 { get; set; }
        public string RecipientName3 { get; set; }
        public string RecipientName4 { get; set; }
        public string RecipientName5 { get; set; }
        public string RecipientName6 { get; set; }
        public string Purpose { get; set; }
        public string Amendment { get; set; }
        public bool IsActive { get; set; }

        public string DocUrl { get; set; }

        public class UpdateIssuanceInfoCommandHandler : IRequestHandler<UpdateIssuanceInfoCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IIssuanceInfoRepository _issuanceInfoRepository;

            public UpdateIssuanceInfoCommandHandler(IIssuanceInfoRepository issuanceInfoRepository, IUnitOfWork unitOfWork)
            {
                _issuanceInfoRepository = issuanceInfoRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateIssuanceInfoCommand command, CancellationToken cancellationToken)
            {
                var issuanceInfo = await _issuanceInfoRepository.GetByIdAsync(command.Id);

                if (issuanceInfo == null)
                {
                    return Result<int>.Fail($"Issuance Info Not Found.");
                }
                else
                {
                    issuanceInfo.DOCNo = command.DOCNo ?? issuanceInfo.DOCNo;
                    issuanceInfo.DocType = command.DocType ?? issuanceInfo.DocType;
                    issuanceInfo.RecipientName1 = command.RecipientName1 ?? issuanceInfo.RecipientName1;
                    issuanceInfo.RecipientName2 = command.RecipientName2 ?? issuanceInfo.RecipientName2;
                    issuanceInfo.RecipientName3 = command.RecipientName3 ?? issuanceInfo.RecipientName3;
                    issuanceInfo.RecipientName4 = command.RecipientName4 ?? issuanceInfo.RecipientName4;
                    issuanceInfo.RecipientName5 = command.RecipientName5 ?? issuanceInfo.RecipientName5;
                    issuanceInfo.RecipientName6 = command.RecipientName6 ?? issuanceInfo.RecipientName6;
                    issuanceInfo.Purpose = command.Purpose ?? issuanceInfo.Purpose;
                    issuanceInfo.Amendment = command.Amendment ?? issuanceInfo.Amendment;
                    issuanceInfo.DocUrl = command.DocUrl ?? issuanceInfo.DocUrl;
                    issuanceInfo.IsActive = command.IsActive;
                    await _issuanceInfoRepository.UpdateAsync(issuanceInfo);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(issuanceInfo.Id);
                }
            }
        }
    }
}
﻿using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IIssuanceInfoRepository
    {
        IQueryable<IssuanceInfo> IssuancesInfo { get; }

        Task<List<IssuanceInfo>> GetListAsync();

        Task<IssuanceInfo> GetByIdAsync(int issuanceInfoId);
        Task<List<IssuanceInfo>> GetByHIdAsync(int HId);
        Task<List<IssuanceInfo>> GetByDOCNoAsync(string docNo, string docType);

        Task<int> InsertAsync(IssuanceInfo issuanceInfo);

        Task UpdateAsync(IssuanceInfo issuanceInfo);

        Task DeleteAsync(IssuanceInfo issuanceInfo);
    }
}
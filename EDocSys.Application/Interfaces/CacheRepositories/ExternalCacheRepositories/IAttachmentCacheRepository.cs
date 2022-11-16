using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.ExternalRecord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories
{
    public interface IAttachmentCacheRepository
    {
        Task<List<Attachment>> GetCachedListAsync();

        Task<Attachment> GetByIdAsync(int departmentId);
        //Task<Attachment> GetByDOCNoAsync(string docno);
        Task<List<Attachment>> GetByDocIdAsync(int docid);
    }
}

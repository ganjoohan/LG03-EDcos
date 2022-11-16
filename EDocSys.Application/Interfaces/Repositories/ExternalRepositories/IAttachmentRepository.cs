using EDocSys.Domain.Entities.ExternalRecord;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories.ExternalRepositories
{
    public interface IAttachmentRepository
    {
        IQueryable<Attachment> Attachments { get; }

        Task<List<Attachment>> GetListAsync();

        Task<Attachment> GetByIdAsync(int attachmentId);
        Task<List<Attachment>> GetByDocIdAsync(int docId);
        //Task<Attachment> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(Attachment attachment);

        Task UpdateAsync(Attachment attachment);

        Task DeleteAsync(Attachment attachment);
    }
}
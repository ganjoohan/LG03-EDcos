using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllCached
{
    public class GetAllDocumentManualStatusCachedResponse
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int DocumentManualId { get; set; }
        
        public DocumentManual DocumentManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

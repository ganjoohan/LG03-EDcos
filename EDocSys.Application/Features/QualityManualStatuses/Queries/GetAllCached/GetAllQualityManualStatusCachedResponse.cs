using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllCached
{
    public class GetAllQualityManualStatusCachedResponse
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int QualityManualId { get; set; }
        
        public QualityManual QualityManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

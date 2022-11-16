using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.LabAccreditationManualStatuses.Queries.GetAllCached
{
    public class GetAllLabAccreditationManualStatusCachedResponse
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int LabAccreditationManualId { get; set; }
        
        public LabAccreditationManual LabAccreditationManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

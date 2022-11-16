using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.EnvironmentalManualStatuses.Queries.GetAllCached
{
    public class GetAllEnvironmentalManualStatusCachedResponse
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int EnvironmentalManualId { get; set; }
        
        public EnvironmentalManual EnvironmentalManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

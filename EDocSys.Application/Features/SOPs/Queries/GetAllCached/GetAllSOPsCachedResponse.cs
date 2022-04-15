using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.SOPs.Queries.GetAllCached
{
    public class GetAllSOPsCachedResponse
    {
        public int Id { get; set; }
        public string WSCPNo { get; set; }
        public string SOPNo { get; set; }
        public string WINo { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public string Definition { get; set; }
        public string Body { get; set; }
        public int DepartmentId { get; set; }
        public string ProcessName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime RevisionDate { get; set; }
        public int RevisionNo { get; set; }
        public DateTime EstalishedDate { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool hasWI { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;

namespace EDocSys.Domain.Entities.Documentation
{
    public class Issuance : AuditableEntity
    {
        public string DOCNo { get; set; }
    
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public ICollection<IssuanceStatus> IssuanceStatus { get; set; }

        public string IssuanceStatusView { get; set; }

        
        public string VerifiedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AcknowledgedBy { get; set; }

        public string RequestedBy { get; set; }
        public string RequestedByPosition { get; set; }
        public DateTime? RequestedByDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public string DOCStatus { get; set; }
    }
}

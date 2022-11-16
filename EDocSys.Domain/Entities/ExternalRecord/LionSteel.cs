using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;

namespace EDocSys.Domain.Entities.ExternalRecord
{
    public class LionSteel : AuditableEntity
    {
        public string FormNo { get; set; }
        public string Title { get; set; }
        public string Section { get; set; }
        public string Type { get; set; }
        public int CompanyId { get; set; }

        //public virtual Company Company { get; set; }

        //public string CompanyName { get; set; }

        public int DepartmentId { get; set; }

        //public virtual Department Department { get; set; }

        //public string ProcessName { get; set; }

        public string SubType { get; set; }
        public string InformedList { get; set; }
        public string Location { get; set; }
        public string RetentionPrd { get; set; }
        public string PIC { get; set; }
        public int RevisionNo { get; set; }
        public DateTime? RevisionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Body { get; set; }

        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public int PrintCount { get; set; }
        public string FilingSystem { get; set; }
        public string Description { get; set; }
    }
}

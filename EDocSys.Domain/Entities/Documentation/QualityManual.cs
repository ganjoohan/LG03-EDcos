﻿using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;

namespace EDocSys.Domain.Entities.Documentation
{
    public class QualityManual : AuditableEntity
    {
        public string DOCNo { get; set; }
        public string SectionNo { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Body { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionNo { get; set; }
        public DateTime? EstalishedDate { get; set; }     

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public ICollection<QualityManualStatus> QualityManualStatus { get; set; }

        public string QualityManualStatusView { get; set; }

        
        public string Concurred1 { get; set; }
        public string Concurred2 { get; set; }
        public string ApprovedBy { get; set; }

        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }
        public DateTime? ArchiveDate { get; set; }
    }
}

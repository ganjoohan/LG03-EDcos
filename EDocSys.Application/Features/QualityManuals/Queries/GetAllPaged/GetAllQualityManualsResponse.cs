﻿using System;

namespace EDocSys.Application.Features.QualityManuals.Queries.GetAllPaged
{
    public class GetAllQualityManualsResponse
    {
        public int Id { get; set; }
        public string DOCNo { get; set; }
        public string SectionNo { get; set; }

        public string Title { get; set; }
        public string Category { get; set; }

        public string Body { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime RevisionDate { get; set; }
        public int RevisionNo { get; set; }
        public DateTime EstalishedDate { get; set; }
        
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }
    }
}


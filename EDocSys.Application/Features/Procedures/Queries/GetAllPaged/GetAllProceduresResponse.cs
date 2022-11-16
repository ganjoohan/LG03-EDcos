using System;

namespace EDocSys.Application.Features.Procedures.Queries.GetAllPaged
{
    public class GetAllProceduresResponse
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
        public DateTime EffectiveDate { get; set; }
        public DateTime RevisionDate { get; set; }
        public int RevisionNo { get; set; }
        public DateTime EstalishedDate { get; set; }
        public bool hasSOP { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }

        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }

        public DateTime? ArchiveDate { get; set; }
    }
}


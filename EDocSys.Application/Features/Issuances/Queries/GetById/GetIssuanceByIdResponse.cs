using System;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceByIdResponse
    {
        public int Id { get; set; }
        public string DOCNo { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string ProcessName { get; set; }
        public string RequestedBy { get; set; }
        public string VerifiedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AcknowledgedBy { get; set; }
        public string RequestedByPosition { get; set; }
        public int PrintCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public DateTime? RequestedByDate { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public string IssuanceStatusView { get; set; }
        public string DOCStatus { get; set; }
    }
}



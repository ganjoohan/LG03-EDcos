using System;

namespace EDocSys.Application.Features.Issuances.Queries.GetById
{
    public class GetIssuanceInfoPrintByHIdResponse
    {
        public int Id { get; set; }
        public int IssInfoId { get; set; }
        public string RecipientName { get; set; }
        public bool IsPrinted { get; set; }
        public DateTime? PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string ReturnedBy { get; set; }
        public bool IsActive { get; set; }
    }
}



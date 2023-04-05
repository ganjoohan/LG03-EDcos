using System;

namespace EDocSys.Application.Features.Issuances.Queries.GetByDOCPNo
{
    public class GetIssuanceInfoByDOCNoResponse
    {
        public int Id { get; set; }
        public int HId { get; set; }
        public int No { get; set; }
        public string DOCId { get; set; }
        public string DocType { get; set; }
        public string RecipientName1 { get; set; }
        public string RecipientName2 { get; set; }
        public string RecipientName3 { get; set; }
        public string RecipientName4 { get; set; }
        public string RecipientName5 { get; set; }
        public string RecipientName6 { get; set; }
        public string Purpose { get; set; }
        public string Amendment { get; set; }
        public bool IsActive { get; set; }
        public bool IsAmend { get; set; }
    }
}



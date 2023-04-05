using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetAllCached
{
    public class GetAllIssuancesInfoCachedResponse
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

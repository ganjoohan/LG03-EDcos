using EDocSys.Domain.Entities.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Features.Issuances.Queries.GetAllCached
{
    public class GetAllIssuancesInfoPrintCachedResponse
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

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;

namespace EDocSys.Domain.Entities.Documentation
{
    public class IssuanceInfoPrint : AuditableEntity
    {
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

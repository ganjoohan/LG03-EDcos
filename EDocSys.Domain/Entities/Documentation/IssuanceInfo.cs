using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;

namespace EDocSys.Domain.Entities.Documentation
{
    public class IssuanceInfo : AuditableEntity
    {
        public int HId { get; set; }
        public int No { get; set; }
        public string DOCNo { get; set; }
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

        public string DocUrl { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class IssuanceStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int IssuanceId { get; set; }
        public int DocumentStatusId { get; set; }

        public Issuance Issuance { get; set; }
        
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

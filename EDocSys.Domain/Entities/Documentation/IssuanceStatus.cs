using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Text.Json.Serialization;

namespace EDocSys.Domain.Entities.Documentation
{
    public class IssuanceStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int IssuanceId { get; set; }
        public int DocumentStatusId { get; set; }
        [JsonIgnore]
        public Issuance Issuance { get; set; }
        
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class SafetyHealthManualStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int SafetyHealthManualId { get; set; }
        
        public SafetyHealthManual SafetyHealthManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class EnvironmentalManualStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int EnvironmentalManualId { get; set; }
        
        public EnvironmentalManual EnvironmentalManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

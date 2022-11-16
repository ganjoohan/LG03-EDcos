using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class QualityManualStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int QualityManualId { get; set; }
        
        public QualityManual QualityManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class LabAccreditationManualStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int LabAccreditationManualId { get; set; }
        
        public LabAccreditationManual LabAccreditationManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class DocumentManualStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int DocumentManualId { get; set; }
        
        public DocumentManual DocumentManual { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

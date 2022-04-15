using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class ProcedureStatus : AuditableEntity
    {
        public string Remarks { get; set; }
        public int ProcedureId { get; set; }
        
        public Procedure Procedure { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
    }
}

using AspNetCoreHero.Abstractions.Domain;
using System;

namespace EDocSys.Domain.Entities.Documentation
{
    public class DocumentStatus : AuditableEntity
    {
        public string Name { get; set; }
    }
}

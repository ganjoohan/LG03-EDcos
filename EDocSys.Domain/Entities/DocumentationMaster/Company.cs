using AspNetCoreHero.Abstractions.Domain;

namespace EDocSys.Domain.Entities.DocumentationMaster
{
    public class Company : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

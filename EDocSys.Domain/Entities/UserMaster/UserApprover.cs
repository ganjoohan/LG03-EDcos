using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;

namespace EDocSys.Domain.Entities.UserMaster
{
    public class UserApprover : AuditableEntity
    {
        public string UserId { get; set; }
        public int CompanyId { get; set; }
        
        public virtual Company Company { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public string ApprovalType { get; set; }
        
    }
}

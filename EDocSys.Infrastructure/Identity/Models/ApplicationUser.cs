using Microsoft.AspNetCore.Identity;

namespace EDocSys.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; } = false;
        public int UserCompanyId { get; set; }
        public int UserDepartmentId { get; set; }
        public string Position { get; set; }
    }
}
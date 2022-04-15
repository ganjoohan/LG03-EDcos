using Microsoft.AspNetCore.Mvc.Rendering;

namespace EDocSys.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } = true;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }

        public int UserCompanyId { get; set; }
        public string UserCompanyName { get; set; }
        public SelectList UserCompanies { get; set; }

        public int UserDepartmentId { get; set; }
        public string UserDepartmentName { get; set; }
        public SelectList UserDepartments { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public SelectList RoleList { get; set; }



        //////
        
        public int CurrentCompanyId { get; set; }
        public string CurrentCompanyName { get; set; }

        public int CurrentDepartmentId { get; set; }
        public string CurrentDepartmentName { get; set; }

        public string CurrentRoleId { get; set; }
        public string CurrentRoleName { get; set; }

        public string Position { get; set; }



    }
}
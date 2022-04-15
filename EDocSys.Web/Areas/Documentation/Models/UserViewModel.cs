using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
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

        public string UserConcurred1Id { get; set; }
        public SelectList UserConcurred1List { get; set; }

        public string UserConcurred2Id { get; set; }
        public SelectList UserConcurred2List { get; set; }

        public string UserApproveBy { get; set; }
        public SelectList UserApproveByList { get; set; }
    }
}

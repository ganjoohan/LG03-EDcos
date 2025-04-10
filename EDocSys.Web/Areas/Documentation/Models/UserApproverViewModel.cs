﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace EDocSys.Web.Areas.Documentation.Models
{
    public class UserApproverViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public SelectList UserList { get; set; }

        public string EmailAddress { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public SelectList UserCompanies { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public SelectList UserDepartments { get; set; }

        public string ApprovalType { get; set; }

        public string Role { get; set; }





        //// ----------------------------------

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        //public string UserName { get; set; }
        //public bool IsActive { get; set; } = true;
        


        //public int UserCompanyId { get; set; }
        //public string UserCompanyName { get; set; }
        //public SelectList UserCompanies { get; set; }

        public int UserDepartmentId { get; set; }
        public string UserDepartmentName { get; set; }
        

        public string UserConcurred1Id { get; set; }
        public SelectList UserConcurred1List { get; set; }

        public string UserConcurred2Id { get; set; }
        public SelectList UserConcurred2List { get; set; }

        public string UserApproveBy { get; set; }
        public SelectList UserApproveByList { get; set; }
        public string UserApprovedId { get; set; }

        public string UserVerifiedId { get; set; }
        public SelectList UserVerifiedByList { get; set; }

        public string UserAcknowledgedBy { get; set; }
        public SelectList UserAcknowledgedByList { get; set; }

    }
}

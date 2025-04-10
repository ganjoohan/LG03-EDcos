﻿using System.Collections.Generic;

namespace EDocSys.Web.Areas.Admin.Models
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public IList<UserRolesViewModel> UserRoles { get; set; }
    }

    public class UserRolesViewModel
    {
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public bool Selected { get; set; }
    }
}
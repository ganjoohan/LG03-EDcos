using Microsoft.AspNetCore.Authorization;

namespace EDocSys.Web.Authorization
{
    public class CanManageUsersRequirement : IAuthorizationRequirement
    {
        public CanManageUsersRequirement()
        {

        }
    }
}

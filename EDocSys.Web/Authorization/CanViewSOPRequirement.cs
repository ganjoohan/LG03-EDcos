using Microsoft.AspNetCore.Authorization;

namespace EDocSys.Web.Authorization
{
    public class CanViewSOPRequirement : IAuthorizationRequirement
    {
        public CanViewSOPRequirement()
        {

        }
    }
}

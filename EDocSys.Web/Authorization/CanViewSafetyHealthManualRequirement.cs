using Microsoft.AspNetCore.Authorization;

namespace EDocSys.Web.Authorization
{
    public class CanViewSafetyHealthManualRequirement : IAuthorizationRequirement
    {
        public CanViewSafetyHealthManualRequirement()
        {

        }
    }
}

using Microsoft.AspNetCore.Authorization;

namespace EDocSys.Web.Authorization
{
    public class CanViewIssuanceRequirement : IAuthorizationRequirement
    {
        public CanViewIssuanceRequirement()
        {

        }
    }
}

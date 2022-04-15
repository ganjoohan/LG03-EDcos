using Microsoft.AspNetCore.Authorization;

namespace EDocSys.Web.Authorization
{
    public class CanViewProcedureRequirement : IAuthorizationRequirement
    {
        public CanViewProcedureRequirement()
        {

        }
    }
}

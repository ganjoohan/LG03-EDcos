using Microsoft.AspNetCore.Authorization;

namespace EDocSys.Web.Authorization
{
    public class CanViewDocumentManualRequirement : IAuthorizationRequirement
    {
        public CanViewDocumentManualRequirement()
        {

        }
    }
}

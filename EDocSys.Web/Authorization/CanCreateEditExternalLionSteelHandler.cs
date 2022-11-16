using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDocSys.Web.Authorization
{
    public class CanCreateEditExternalLionSteelHandler : AuthorizationHandler<CanCreateEditExternalLionSteelRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationExternalDbContext _dbContext;
        private readonly IdentityContext _identityContext;

        public CanCreateEditExternalLionSteelHandler(IHttpContextAccessor httpContextAccessor, ApplicationExternalDbContext dbContext, IdentityContext identityContext)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbContext = dbContext;
            _identityContext = identityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanCreateEditExternalLionSteelRequirement requirement)
        {

            
            //MUST BE AUTHENTICATED
            //=====================
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            //get user
            var userId = context.User.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Select(c => c.Value).FirstOrDefault();
            var user = _identityContext.Users.FirstOrDefault(x => x.Id == userId);
            var userRoles = context.User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value).ToList();

            //Document Manual MUST EXISTS
            //=====================
            // https://domain.com/area/controller/id
            //var documentManualId = _httpContextAccessor.HttpContext.GetRouteValue("id")?.ToString();
            // https://domain.com/area/controller?id=x
            var lionSteelId = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("id");

            var lionSteelCreate = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("ce");
            // ce = f11c9d1f-abbb-45a1-8d11-b0a5590e5893 => create code
            // ce = bd773a2a-ffc6-4ed2-9ba3-c7ea631628be => edit code
            int id;
            
            bool success = int.TryParse(lionSteelId, out id);
            if (!success && lionSteelCreate == "f11c9d1f-abbb-45a1-8d11-b0a5590e5893") // Create
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else if (lionSteelCreate == "bd773a2a-ffc6-4ed2-9ba3-c7ea631628be")   // Edit
            {
                var lionSteel = _dbContext.LionSteels.FirstOrDefault(x => x.Id == id);
                if (lionSteel == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                if (context.User.IsInRole("D") || context.User.IsInRole("E") )
                {
                    //if (documentManual.CompanyId == user.UserCompanyId && documentManual.DepartmentId == user.UserDepartmentId)
                    if (lionSteel.CompanyId == user.UserCompanyId)
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                    else
                    {
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }
                else if (context.User.IsInRole("SuperAdmin"))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                else
                {
                    if (lionSteel.CompanyId != user.UserCompanyId)
                    {
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }
            }
            
            context.Fail();
            return Task.CompletedTask;
        }
    }
}

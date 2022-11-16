using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDocSys.Web.Authorization
{
    public class CanCreateEditEnvironmentalManualHandler : AuthorizationHandler<CanCreateEditEnvironmentalManualRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly IdentityContext _identityContext;

        public CanCreateEditEnvironmentalManualHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext, IdentityContext identityContext)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbContext = dbContext;
            _identityContext = identityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanCreateEditEnvironmentalManualRequirement requirement)
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
            //var environmentalManualId = _httpContextAccessor.HttpContext.GetRouteValue("id")?.ToString();
            // https://domain.com/area/controller?id=x
            var environmentalManualId = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("id");

            var environmentalManualCreate = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("ce");
            // ce = f11c9d1f-abbb-45a1-8d11-b0a5590e5893 => create code
            // ce = bd773a2a-ffc6-4ed2-9ba3-c7ea631628be => edit code
            int id;
            
            bool success = int.TryParse(environmentalManualId, out id);
            if (!success && environmentalManualCreate == "f11c9d1f-abbb-45a1-8d11-b0a5590e5893") // Create
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else if (environmentalManualCreate == "bd773a2a-ffc6-4ed2-9ba3-c7ea631628be")   // Edit
            {
                var environmentalManual = _dbContext.EnvironmentalManuals.FirstOrDefault(x => x.Id == id);
                if (environmentalManual == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                if (context.User.IsInRole("D") || context.User.IsInRole("E") )
                {
                    //if (EnvironmentalManual.CompanyId == user.UserCompanyId && EnvironmentalManual.DepartmentId == user.UserDepartmentId)
                    if (environmentalManual.CompanyId == user.UserCompanyId)
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
                    if (environmentalManual.CompanyId != user.UserCompanyId)
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

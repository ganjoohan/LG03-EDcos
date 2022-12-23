using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDocSys.Web.Authorization
{
    public class CanCreateEditIssuanceHandler : AuthorizationHandler<CanCreateEditIssuanceRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly IdentityContext _identityContext;

        public CanCreateEditIssuanceHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext, IdentityContext identityContext)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbContext = dbContext;
            _identityContext = identityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanCreateEditIssuanceRequirement requirement)
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

            //Issuance MUST EXISTS
            //=====================
            // https://domain.com/area/controller/id
            //var issuanceId = _httpContextAccessor.HttpContext.GetRouteValue("id")?.ToString();
            // https://domain.com/area/controller?id=x
            var issuanceId = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("id");

            var issuanceCreate = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("ce");
            // ce = f11c9d1f-abbb-45a1-8d11-b0a5590e5893 => create code
            // ce = bd773a2a-ffc6-4ed2-9ba3-c7ea631628be => edit code
            int id;
            
            bool success = int.TryParse(issuanceId, out id);
            if (!success && issuanceCreate == "f11c9d1f-abbb-45a1-8d11-b0a5590e5893") // Create
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else if (issuanceCreate == "bd773a2a-ffc6-4ed2-9ba3-c7ea631628be")   // Edit
            {
                var issuance = _dbContext.Issuances.FirstOrDefault(x => x.Id == id);
                if (issuance == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                if (context.User.IsInRole("D") || context.User.IsInRole("E") )
                {
                    //if (issuance.CompanyId == user.UserCompanyId && issuance.DepartmentId == user.UserDepartmentId)
                    if (issuance.CompanyId == user.UserCompanyId)
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
                    if (issuance.CompanyId != user.UserCompanyId)
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

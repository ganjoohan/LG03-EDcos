using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDocSys.Web.Authorization
{
    public class CanViewWIHandler : AuthorizationHandler<CanViewWIRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly IdentityContext _identityContext;

        public CanViewWIHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext, IdentityContext identityContext)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbContext = dbContext;
            _identityContext = identityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanViewWIRequirement requirement)
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

            //PROCEDURE MUST EXISTS
            //=====================
            // https://domain.com/area/controller/id
            var wiId_GetRoute = _httpContextAccessor.HttpContext.GetRouteValue("id")?.ToString();
            int id_GetRoute;
            // https://domain.com/area/controller?id=x
            var wiId = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("id");
            int id;
            bool success = int.TryParse(wiId, out id);
            bool success_GetRoute = int.TryParse(wiId_GetRoute, out id_GetRoute);
            if (!success)
            {
                if (success_GetRoute == true)
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
            else if (!success_GetRoute)
            {
                if (success == true)
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

            var wi = _dbContext.WorkInstructions.FirstOrDefault(x => x.Id == id);
            if (wi == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            //CUSTOM CONDITIONS CHECKING
            //==============================
            //user must be from same company and department

            //if (context.User.IsInRole("CompanyAdmin"))
            //{
            //    if (wi.CompanyId != user.UserCompanyId)
            //    {
            //        context.Fail();
            //        return Task.CompletedTask;
            //    }
            //}
            if (context.User.IsInRole("D"))
            {
                if (wi.CompanyId == user.UserCompanyId)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;

            }
            if (context.User.IsInRole("E"))
            {
                if (wi.CompanyId == user.UserCompanyId)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            if (context.User.IsInRole("B1"))
            {
                if (wi.CompanyId == user.UserCompanyId && wi.Concurred1 == user.Id)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            if (context.User.IsInRole("A"))
            {
                if (wi.Concurred1 == user.Id)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            //else
            //{
            //    if (wi.DepartmentId != user.UserDepartmentId || wi.CompanyId != user.UserCompanyId)
            //    {
            //        context.Fail();
            //        return Task.CompletedTask;
            //    } 
            //}


            //if document not yet effective, only those involved in the concur/approve process can see
            /*
            if (wi.RevisionDate == null)
            {
                if (userId != wi.Concurred1 && userId != wi.Concurred2 && userId != wi.ApprovedBy)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            */

            //context.Succeed(requirement);
            context.Fail();
            return Task.CompletedTask;
        }
    }
}

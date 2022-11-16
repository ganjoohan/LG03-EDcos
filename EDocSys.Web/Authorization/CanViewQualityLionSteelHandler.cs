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
    public class CanViewQualityLionSteelHandler : AuthorizationHandler<CanViewQualityLionSteelRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationQualityDbContext _dbContext;
        private readonly IdentityContext _identityContext;

        public CanViewQualityLionSteelHandler(IHttpContextAccessor httpContextAccessor, ApplicationQualityDbContext dbContext, IdentityContext identityContext)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _dbContext = dbContext;
            _identityContext = identityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanViewQualityLionSteelRequirement requirement)
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
            var lionSteelId_GetRoute = _httpContextAccessor.HttpContext.GetRouteValue("id")?.ToString();
            int id_GetRoute;
            // https://domain.com/area/controller?id=x
            var lionSteelId = HttpUtility.ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value).Get("id");
            int id;
            bool success = int.TryParse(lionSteelId, out id);
            bool success_GetRoute = int.TryParse(lionSteelId_GetRoute, out id_GetRoute);
            if (!success)
            {
                if(success_GetRoute == true)
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
                if (success==true)
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

            var lionSteel = _dbContext.LionSteels.FirstOrDefault(x => x.Id == id);
            if (lionSteel == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            //CUSTOM CONDITIONS CHECKING
            //==============================
            //user must be from same company and department

            //if (context.User.IsInRole("CompanyAdmin"))
            //{
            //    if (documentManual.CompanyId != user.UserCompanyId)
            //    {
            //        context.Fail();
            //        return Task.CompletedTask;
            //    }
            //}
            if (context.User.IsInRole("D"))
            {
                if (lionSteel.CompanyId == user.UserCompanyId)
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
                if (lionSteel.CompanyId == user.UserCompanyId)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            //if (context.User.IsInRole("B1"))
            //{
            //    if (documentManual.CompanyId == user.UserCompanyId && documentManual.Concurred1 == user.Id)
            //    {
            //        context.Succeed(requirement);
            //        return Task.CompletedTask;
            //    }
            //}
            //if (context.User.IsInRole("A"))
            //{
            //    if (documentManual.Concurred1 == user.Id)
            //    {
            //        context.Succeed(requirement);
            //        return Task.CompletedTask;
            //    }
            //}
            //else
            //{
            //    if (documentManual.DepartmentId != user.UserDepartmentId || documentManual.CompanyId != user.UserCompanyId)
            //    {
            //        context.Fail();
            //        return Task.CompletedTask;
            //    } 
            //}


            //if document not yet effective, only those involved in the concur/approve process can see
            /*
            if (documentManual.RevisionDate == null)
            {
                if (userId != documentManual.Concurred1 && userId != documentManual.Concurred2 && userId != documentManual.ApprovedBy)
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

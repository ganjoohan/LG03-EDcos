using EDocSys.Application.Enums;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.CompanyAdmin.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.DepartmentAdmin.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.BasicUser.ToString()));
            // await roleManager.CreateAsync(new IdentityRole(Roles.Uploader.ToString()));
        }
    }
}
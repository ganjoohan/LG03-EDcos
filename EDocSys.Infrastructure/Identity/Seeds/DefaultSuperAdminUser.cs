using EDocSys.Application.Constants;
using EDocSys.Application.Enums;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdminUser
    {
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
                }
            }
        }

        private async static Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("SuperAdmin");
            await roleManager.AddPermissionClaim(adminRole, "Users");
            await roleManager.AddPermissionClaim(adminRole, "Products");
            await roleManager.AddPermissionClaim(adminRole, "Brands");
            await roleManager.AddPermissionClaim(adminRole, "Departments");
            await roleManager.AddPermissionClaim(adminRole, "Companies");
            await roleManager.AddPermissionClaim(adminRole, "Procedures");
            await roleManager.AddPermissionClaim(adminRole, "SOPs");
            await roleManager.AddPermissionClaim(adminRole, "WIs");
            await roleManager.AddPermissionClaim(adminRole, "DocumentManuals");
            await roleManager.AddPermissionClaim(adminRole, "QualityManuals");
            await roleManager.AddPermissionClaim(adminRole, "EnvironmentalManuals");
            await roleManager.AddPermissionClaim(adminRole, "LabAccreditationManuals");
            await roleManager.AddPermissionClaim(adminRole, "SafetyHealthManuals");
        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "superadmin@lion.com.my",
                FirstName = "Lito",
                LastName = "Juliano",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.C.ToString());
                    //await userManager.AddToRoleAsync(defaultUser, Roles.DepartmentAdmin.ToString());
                    //await userManager.AddToRoleAsync(defaultUser, Roles.CompanyAdmin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                }
                await roleManager.SeedClaimsForSuperAdmin();
            }
        }
    }
}
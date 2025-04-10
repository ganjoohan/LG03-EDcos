﻿using EDocSys.Application.Constants;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.Admin.Models;
using EDocSys.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PermissionController : BaseController<PermissionController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index(string roleId)
        {
            var model = new PermissionViewModel();
            var allPermissions = new List<RoleClaimsViewModel>();
            
            // disable later
            //allPermissions.GetPermissions(typeof(Permissions.Brands), roleId);
            
            //allPermissions.GetPermissions(typeof(Permissions.Dashboard), roleId);
            
            // disable later
            //allPermissions.GetPermissions(typeof(Permissions.Products), roleId);
            
            allPermissions.GetPermissions(typeof(Permissions.Users), roleId);
            allPermissions.GetPermissions(typeof(Permissions.Procedures), roleId);
            allPermissions.GetPermissions(typeof(Permissions.SOPs), roleId);
            allPermissions.GetPermissions(typeof(Permissions.WIs), roleId);
            allPermissions.GetPermissions(typeof(Permissions.Departments), roleId);
            allPermissions.GetPermissions(typeof(Permissions.Companies), roleId);
            allPermissions.GetPermissions(typeof(Permissions.DocumentManuals), roleId);
            allPermissions.GetPermissions(typeof(Permissions.QualityManuals), roleId);

            allPermissions.GetPermissions(typeof(Permissions.EnvironmentalManuals), roleId);
            allPermissions.GetPermissions(typeof(Permissions.LabAccreditationManuals), roleId);
            allPermissions.GetPermissions(typeof(Permissions.SafetyHealthManuals), roleId);
            allPermissions.GetPermissions(typeof(Permissions.LionSteels), roleId);
            allPermissions.GetPermissions(typeof(Permissions.Issuances), roleId);
            var role = await _roleManager.FindByIdAsync(roleId);
            model.RoleId = roleId;
            var claims = await _roleManager.GetClaimsAsync(role);
            var claimsModel = _mapper.Map<List<RoleClaimsViewModel>>(claims);
            var allClaimValues = allPermissions.Select(a => a.Value).ToList();
            var roleClaimValues = claimsModel.Select(a => a.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permission in allPermissions)
            {
                if (authorizedClaims.Any(a => a == permission.Value))
                {
                    permission.Selected = true;
                }
            }
            model.RoleClaims = _mapper.Map<List<RoleClaimsViewModel>>(allPermissions);
            ViewData["Title"] = $"Permissions for {role.Name} Role";
            ViewData["Caption"] = $"Manage {role.Name} Role Permissions.";
            _notify.Success($"Updated Claims / Permissions for Role '{role.Name}'");
            return View(model);
        }

        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            //Remove all displayed Claims First
            var claims = await _roleManager.GetClaimsAsync(role);
            claims = claims.Where(w => model.RoleClaims.Select(s=> s.Value).Contains(w.Value)).ToList();
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaim(role, claim.Value);
            }
            _notify.Success($"Updated Claims / Permissions for Role '{role.Name}'");
            //var user = await _userManager.GetUserAsync(User);
            //await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Index", new { roleId = model.RoleId });
        }
    }
}
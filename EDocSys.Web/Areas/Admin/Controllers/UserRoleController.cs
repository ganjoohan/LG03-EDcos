﻿using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Infrastructure.DbContexts;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRoleController : BaseController<UserRoleController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityContext _identityContext;


        public UserRoleController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
                              IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _identityContext = identityContext;
        }

        public async Task<IActionResult> Index(string userId)
        {
            var viewModel = new List<UserRolesViewModel>();
            
            var user = await _userManager.FindByIdAsync(userId);
            ViewData["Title"] = $"{user.UserName} - Roles";
            ViewData["Caption"] = $"Manage {user.Email}'s Roles.";
            foreach (var role in _roleManager.Roles.Where(a => a.Name != "SuperAdmin"))
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                
                viewModel.Add(userRolesViewModel);
            }
            var model = new ManageUserRolesViewModel()
            {
                UserId = userId,
                UserRoles = viewModel
            };

            return View(model);
        }

        public async Task<IActionResult> Update(string id, ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));
            var currentUser = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(currentUser);
            await Infrastructure.Identity.Seeds.DefaultSuperAdminUser.SeedAsync(_userManager, _roleManager);
            _notify.Success($"Updated Roles for User '{user.Email}'");
            return RedirectToAction("Index", new { userId = id });

            //var claims = await _userManager.GetClaimsAsync(user);
            //var claim = new Claim("department", "HR");
            //var cresult = await _userManager.AddClaimAsync(user, claim);
        }

       
    }
}
using EDocSys.Application.Constants;
using EDocSys.Application.Enums;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Infrastructure.DbContexts;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : BaseController<UserController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IdentityContext _identityContext;

        private List<ApplicationUser> allUsersExceptCurrentUser;
        private List<GetAllCompaniesCachedResponse> responseCompaniesSingle;
        private List<GetAllDepartmentsCachedResponse> responseDepartmentAll;

        public UserController(ApplicationDbContext context,
                              UserManager<ApplicationUser> userManager, 
                              SignInManager<ApplicationUser> signInManager, 
                              RoleManager<IdentityRole> roleManager,
                              IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _identityContext = identityContext;
        }

        // [Authorize(Policy = Permissions.Users.View)]
        public IActionResult Index()
        {
           return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            ViewBag.RoleA = false;
            ViewBag.RoleB1 = false;
            ViewBag.RoleB2 = false;
            ViewBag.RoleC = false;
            ViewBag.RoleE = false;
            ViewBag.RoleD = false;
            ViewBag.RoleSA = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }
            if (rolesList.Contains("A"))
            {
                ViewBag.RoleA = true;
            }
            if (rolesList.Contains("B1"))
            {
                ViewBag.RoleB1 = true;
            }
            if (rolesList.Contains("B2"))
            {
                ViewBag.RoleB2 = true;
            }
            if (rolesList.Contains("C"))
            {
                ViewBag.RoleC = true;
            }
            if (rolesList.Contains("E"))
            {
                ViewBag.RoleE = true;
            }
            if (rolesList.Contains("D"))
            {
                ViewBag.RoleD = true;
            }
            if (rolesList.Contains("SuperAdmin"))
            {
                ViewBag.RoleSA = true;
            }

            if (rolesList.Contains("SuperAdmin"))
            {
                allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
            }
            else if (rolesList.Contains("E"))
            {
                allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id && a.UserCompanyId == currentUser.UserCompanyId && a.UserName != "superadmin" && a.Position != "Super Admin").ToListAsync();
                var userRoleA = await _userManager.Users.Where(a => a.Id != currentUser.Id && a.UserCompanyId == 0 && a.UserName != "superadmin" && a.Position != "Super Admin").ToListAsync();
                allUsersExceptCurrentUser.AddRange(userRoleA);
            }

            var responseCompany = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var responseDepartment = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            // var asdfa = _identityContext.UserClaims.ToList();

            var allUsersExceptCurrentUserList = (from a1 in allUsersExceptCurrentUser
                                                 join a2 in responseCompany.Data on a1.UserCompanyId equals a2.Id
                                                 join a3 in responseDepartment.Data on a1.UserDepartmentId equals a3.Id
                                                 join a4 in _identityContext.UserRoles on a1.Id equals a4.UserId
                                                 join a5 in _roleManager.Roles on a4.RoleId equals a5.Id
                                                 select new UserViewModel
                                                 {
                                                     Id = a1.Id,
                                                     FirstName = a1.FirstName,
                                                     LastName = a1.LastName,
                                                     ProfilePicture = a1.ProfilePicture,
                                                     IsActive = a1.IsActive,
                                                     UserName = a1.UserName,
                                                     Email = a1.Email,
                                                     EmailConfirmed = a1.EmailConfirmed,
                                                     UserCompanyName = a2.Name,
                                                     UserDepartmentName = a3.Name,
                                                     RoleName = a5.Name
                                                 }).ToList();

                //var allUsersExceptCurrentUser0 = allUsersExceptCurrentUser.Where(w => w.UserCompanyId == 0 && w.UserDepartmentId == 0).ToList();
                var allSuperUsers = (from a1 in allUsersExceptCurrentUser
                                     join a4 in _identityContext.UserRoles on a1.Id equals a4.UserId
                                     join a5 in _roleManager.Roles on a4.RoleId equals a5.Id
                                     where !allUsersExceptCurrentUserList.Select(x => x.Id).Contains(a1.Id) && a5.Name != "SuperAdmin"
                                     select new UserViewModel
                                     {
                                         Id = a1.Id,
                                         FirstName = a1.FirstName,
                                         LastName = a1.LastName,
                                         ProfilePicture = a1.ProfilePicture,
                                         IsActive = a1.IsActive,
                                         UserName = a1.UserName,
                                         Email = a1.Email,
                                         EmailConfirmed = a1.EmailConfirmed,
                                         UserCompanyName = "",
                                         UserDepartmentName = "",
                                         RoleName = a5.Name
                                     }).ToList();
                allUsersExceptCurrentUserList.AddRange(allSuperUsers);

            // var model_ORI = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser_ORI);

            var model = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUserList);
            // return PartialView("_ViewAll", model_ORI);
            return PartialView("_ViewAll", model);
        }

        public async Task<IActionResult> OnGetCreate()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var userViewModel = new UserViewModel();

            var responseCompanies = await _mediator.Send(new GetAllCompaniesCachedQuery());

            var user = await _userManager.FindByNameAsync(currentUser.UserName);
            var roles = await _userManager.GetRolesAsync(user);
            if (currentUser.UserName == "superadmin" || roles.Contains("SuperAdmin"))
            {
                responseCompaniesSingle = responseCompanies.Data;
            }
            else
            {
                responseCompaniesSingle = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).ToList();
            }

            var companyViewModel = (from a1 in responseCompaniesSingle
                                   select new
                                   {
                                       a1.Id,
                                        a1.Name
                                   }).OrderBy( a => a.Name).ToList();

            userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

            var responseDepartments = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModel = (from a1 in responseDepartments.Data
                                    select new
                                    {
                                        a1.Id,
                                        a1.Name
                                    }).OrderBy(a => a.Name).ToList();
                        
            userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");

            var roles0 = _roleManager.Roles.ToList();
            if (currentUser.UserName == "superadmin" || roles.Contains("SuperAdmin"))
            {
                userViewModel.RoleList = new SelectList(roles0, "Name", "Name");
            }
            else
            {
                roles0 = roles0.Where(w => w.Name != "SuperAdmin").ToList();
                userViewModel.RoleList = new SelectList(roles0, "Name", "Name");
            }

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", userViewModel) });
        }

        public async Task<IActionResult> OnGetEdit(string id)
        {
            var currentLogUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUser = await _userManager.FindByIdAsync(id);
            var user = await _userManager.FindByNameAsync(currentLogUser.UserName);
            var roles = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserViewModel();

            var responseCompanies = await _mediator.Send(new GetAllCompaniesCachedQuery());

            var responseDepartments = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            if (currentLogUser.UserName == "superadmin" || roles.Contains("SuperAdmin"))
            {
                responseCompaniesSingle = responseCompanies.Data;

                responseDepartmentAll = responseDepartments.Data;
            }
            else
            {
                responseCompaniesSingle = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).ToList();
                responseDepartmentAll = responseDepartments.Data.ToList();
            }

            var companyViewModel = responseCompanies.Data.ToList();
            userViewModel.UserCompanies = new SelectList(responseCompaniesSingle, "Id", "Name", null, null);

            var departmentViewModel = (from a1 in responseDepartmentAll
                                       select new
                                       {
                                           a1.Id,
                                           a1.Name
                                       }).OrderBy(a => a.Name).ToList();

            userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");

            var roles0 = _roleManager.Roles.ToList();
            if (currentLogUser.UserName == "superadmin" || roles.Contains("SuperAdmin"))
            {
                userViewModel.RoleList = new SelectList(roles0, "Name", "Name");
            }
            else
            {
                roles0 = roles0.Where(w => w.Name != "SuperAdmin").ToList();
                userViewModel.RoleList = new SelectList(roles0, "Name", "Name");
            }

            userViewModel.FirstName = currentUser.FirstName;
            userViewModel.LastName = currentUser.LastName;
            userViewModel.Email = currentUser.Email;
            userViewModel.Position = currentUser.Position;
            userViewModel.UserName = currentUser.UserName;

            // Get current user company
            userViewModel.CurrentCompanyId = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).Select(a=>a.Id).SingleOrDefault();
            userViewModel.CurrentCompanyName = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).Select(a => a.Name).SingleOrDefault();

            // Get current user department
            userViewModel.CurrentDepartmentId = responseDepartments.Data.Where(a => a.Id == currentUser.UserDepartmentId).Select(a => a.Id).SingleOrDefault();
            userViewModel.CurrentDepartmentName = responseDepartments.Data.Where(a => a.Id == currentUser.UserDepartmentId).Select(a => a.Name).SingleOrDefault();

            
            // Get current user role
            var rolesCurrent = await _userManager.GetRolesAsync(currentUser);
            userViewModel.CurrentRoleId = rolesCurrent.SingleOrDefault();
            userViewModel.CurrentRoleName = rolesCurrent.SingleOrDefault();

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Edit", userViewModel) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreate(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                MailAddress address = new MailAddress(userModel.Email);
                string userName = address.User;
                var oldUsers = _userManager.Users.Where(w => w.UserName == userName).ToList();
                if(oldUsers.Count > 0)
                {
                    if (oldUsers.Select(s=> s.Email).Where(w=> w == userModel.Email).FirstOrDefault() == null)
                    {
                        List<string> lstEmail = userModel.Email.Split('@').ToList();
                        List<string> lstEmail2 = lstEmail[1].Split('.').ToList();
                        userName = (lstEmail[0] + ((lstEmail2.Count > 3 ? (lstEmail2[0] + lstEmail2[1]) : lstEmail2[0])));
                    }
                }
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userModel.Email,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    EmailConfirmed = true,
                    UserCompanyId = (userModel.RoleName == "SuperAdmin" || userModel.RoleName == "A" ? 0 : userModel.UserCompanyId),
                    UserDepartmentId = (userModel.RoleName == "SuperAdmin" || userModel.RoleName == "A" ? 0 : userModel.UserDepartmentId),
                    Position = userModel.Position,
                    IsActive = (userModel.RoleName == "SuperAdmin" ? false : true),
                };
                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userModel.RoleName == "4" ? "None" : userModel.RoleName);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
                    var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                    var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
                    _notify.Success($"Account for {user.Email} created.");
                    return new JsonResult(new { isValid = true, html = htmlData });
                }
                foreach (var error in result.Errors)
                {
                    _notify.Error(error.Description);
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_Create", userModel);
                return new JsonResult(new { isValid = false, html = html });
            }
            return default;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostEdit(UserViewModel userModel)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userModel.UserName);

            if (ModelState.IsValid)
            {
                user.UserName = userModel.UserName;
                user.Email = userModel.Email;
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.EmailConfirmed = true;
                user.UserCompanyId = (userModel.RoleName == "SuperAdmin" || userModel.RoleName == "A" ? 0 : userModel.UserCompanyId);
                user.UserDepartmentId = (userModel.RoleName == "SuperAdmin" || userModel.RoleName == "A" ? 0 : userModel.UserDepartmentId);
                user.Position = userModel.Position;
                user.IsActive = (userModel.RoleName == "SuperAdmin" ? false : true);

                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    var userR = await _userManager.FindByNameAsync(userModel.UserName);
                    var rolesR = await _userManager.GetRolesAsync(userR);
                    var resultR = await _userManager.RemoveFromRolesAsync(userR, rolesR);
                    resultR = await _userManager.AddToRoleAsync(user, userModel.RoleName);

                    var currentLogUser = await _userManager.GetUserAsync(HttpContext.User);

                    var responseCompany = await _mediator.Send(new GetAllCompaniesCachedQuery());
                    var responseDepartment = await _mediator.Send(new GetAllDepartmentsCachedQuery());


                    allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentLogUser.Id).ToListAsync();


                    var allUsersExceptCurrentUserList = (from a1 in allUsersExceptCurrentUser
                                                         join a2 in responseCompany.Data on a1.UserCompanyId equals a2.Id
                                                         join a3 in responseDepartment.Data on a1.UserDepartmentId equals a3.Id
                                                         select new UserViewModel
                                                         {
                                                             Id = a1.Id,
                                                             FirstName = a1.FirstName,
                                                             LastName = a1.LastName,
                                                             ProfilePicture = a1.ProfilePicture,
                                                             IsActive = a1.IsActive,
                                                             UserName = a1.UserName,
                                                             Email = a1.Email,
                                                             EmailConfirmed = a1.EmailConfirmed,
                                                             UserCompanyName = a2.Name,
                                                             UserDepartmentName = a3.Name,
                                                         }).ToList();

                    var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUserList);

                    var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
                    _notify.Success($"Account for {user.Email} modified.");
                    return new JsonResult(new { isValid = true, html = htmlData });
                }                
                foreach (var error in result.Errors)
                {
                    _notify.Error(error.Description);
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", userModel);
                return new JsonResult(new { isValid = false, html = html });
            }
            return default;
        }
        public async Task<IActionResult> OnPostCreateUserRole(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                MailAddress address = new MailAddress(userModel.Email);
                int count = 0;
                string userName = await GetDummyUserName(address.User, count);
                var user = new ApplicationUser
                {
                    UserName = userName,
                    NormalizedUserName = userName.ToUpper(),
                    Email = userModel.Email,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    EmailConfirmed = true,
                    UserCompanyId = (userModel.RoleName == "SuperAdmin" || userModel.RoleName == "A" ? 0 : userModel.UserCompanyId),
                    UserDepartmentId = (userModel.RoleName == "SuperAdmin" || userModel.RoleName == "A" ? 0 : userModel.UserDepartmentId),
                    Position = userModel.Position,
                    IsActive = (userModel.RoleName == "SuperAdmin" ? false : true),
                };
                string tempPwd = "Urole@Pwd1";
                userModel.Password = tempPwd;
                userModel.ConfirmPassword = tempPwd;
                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userModel.RoleName);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
                    var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                    var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
                    _notify.Success($"Account for {user.Email} created.");
                    return new JsonResult(new { isValid = true, html = htmlData });
                }
                foreach (var error in result.Errors)
                {
                    _notify.Error(error.Description);
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateUserRole", userModel);
                return new JsonResult(new { isValid = false, html = html });
            }
            return default;
        }

        public async Task<string> GetDummyUserName(string s_prefix, int count)
        {
            count++;
            string s_userName = s_prefix + count;
            var chkUserName = await _userManager.FindByNameAsync(s_userName);
            if (chkUserName == null)
                return s_userName;
            else
                return await GetDummyUserName(s_prefix, count);
        }

        public async Task<IActionResult> OnGetCreateUserRole(string id)
        {
            var currentLogUser = await _userManager.GetUserAsync(HttpContext.User);

            var currentUser = await _userManager.FindByIdAsync(id);
            var user = await _userManager.FindByNameAsync(currentLogUser.UserName);
            var roles = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserViewModel();

            var responseCompanies = await _mediator.Send(new GetAllCompaniesCachedQuery());

            var responseDepartments = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            if (currentLogUser.UserName == "superadmin" || roles.Contains("SuperAdmin"))
            {
                responseCompaniesSingle = responseCompanies.Data;

                responseDepartmentAll = responseDepartments.Data;
            }
            else
            {
                responseCompaniesSingle = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).ToList();
                responseDepartmentAll = responseDepartments.Data.ToList();
            }

            var companyViewModel = responseCompanies.Data.ToList();
            userViewModel.UserCompanies = new SelectList(responseCompaniesSingle, "Id", "Name", null, null);

            var departmentViewModel = (from a1 in responseDepartmentAll
                                       select new
                                       {
                                           a1.Id,
                                           a1.Name
                                       }).OrderBy(a => a.Name).ToList();

            userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");

            var roles0 = _roleManager.Roles.ToList();
            if (currentLogUser.UserName == "superadmin" || roles.Contains("SuperAdmin"))
            {
                userViewModel.RoleList = new SelectList(roles0, "Name", "Name");
            }
            else
            {
                roles0 = roles0.Where(w => w.Name != "SuperAdmin").ToList();
                userViewModel.RoleList = new SelectList(roles0, "Name", "Name");
            }

            userViewModel.FirstName = currentUser.FirstName;
            userViewModel.LastName = currentUser.LastName;
            userViewModel.Email = currentUser.Email;
            userViewModel.Position = currentUser.Position;

            // Get current user company
            userViewModel.CurrentCompanyId = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).Select(a => a.Id).SingleOrDefault();
            userViewModel.CurrentCompanyName = responseCompanies.Data.Where(a => a.Id == currentUser.UserCompanyId).Select(a => a.Name).SingleOrDefault();

            // Get current user department
            userViewModel.CurrentDepartmentId = responseDepartments.Data.Where(a => a.Id == currentUser.UserDepartmentId).Select(a => a.Id).SingleOrDefault();
            userViewModel.CurrentDepartmentName = responseDepartments.Data.Where(a => a.Id == currentUser.UserDepartmentId).Select(a => a.Name).SingleOrDefault();


            // Get current user role
            var rolesCurrent = await _userManager.GetRolesAsync(currentUser);
            userViewModel.CurrentRoleId = rolesCurrent.SingleOrDefault();
            userViewModel.CurrentRoleName = rolesCurrent.SingleOrDefault();

            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateUserRole", userViewModel) });
        }
    }
}
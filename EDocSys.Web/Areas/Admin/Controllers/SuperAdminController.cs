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
    public class SuperAdminController : BaseController<SuperAdminController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IdentityContext _identityContext;

        private List<ApplicationUser> allUsersExceptCurrentUser;
        private List<GetAllCompaniesCachedResponse> responseCompaniesSingle;
        private List<GetAllDepartmentsCachedResponse> responseDepartmentAll;

        public SuperAdminController(ApplicationDbContext context,
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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var usersInSuperAdminRole = await _userManager.GetUsersInRoleAsync("SuperAdmin");

            var superAdminUsers = usersInSuperAdminRole.ToList();

            var responseCompany = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var responseDepartment = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            var superAdminUserList = (from user in superAdminUsers
                                      join company in responseCompany.Data on user.UserCompanyId equals company.Id into userCompanyGroup
                                      from userCompany in userCompanyGroup.DefaultIfEmpty()
                                      join department in responseDepartment.Data on user.UserDepartmentId equals department.Id into userDepartmentGroup
                                      from userDepartment in userDepartmentGroup.DefaultIfEmpty()
                                      join userRole in _identityContext.UserRoles on user.Id equals userRole.UserId
                                      join role in _roleManager.Roles on userRole.RoleId equals role.Id
                                      where role.Name == "SuperAdmin"
                                      select new UserViewModel
                                      {
                                          Id = user.Id,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          ProfilePicture = user.ProfilePicture,
                                          IsActive = user.IsActive,
                                          UserName = user.UserName,
                                          Email = user.Email,
                                          EmailConfirmed = user.EmailConfirmed,
                                          UserCompanyName = userCompany?.Name,
                                          UserDepartmentName = userDepartment?.Name,
                                          RoleName = role.Name
                                      }).ToList();

            var model = _mapper.Map<IEnumerable<UserViewModel>>(superAdminUserList);
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
                    IsActive = (userModel.RoleName == "SuperAdmin" ? true : true),
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
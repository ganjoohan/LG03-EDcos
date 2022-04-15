using EDocSys.Application.Features.UserApprovers.Commands.Create;
using EDocSys.Application.Features.UserApprovers.Commands.Delete;
using EDocSys.Application.Features.UserApprovers.Commands.Update;
using EDocSys.Application.Features.UserApprovers.Queries.GetAllCached;
using EDocSys.Application.Features.UserApprovers.Queries.GetById;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.Documentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Departments.Queries.GetById;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class UserApproverController : BaseController<CompanyController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        //private List<ApplicationUser> allUsersExceptCurrentUser;
        private List<GetAllCompaniesCachedResponse> responseCompaniesSingle;

        public UserApproverController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index(string apt)
        {
            ViewBag.APT = apt;
            var model = new UserApproverViewModel();
            return View(model);
        }

        public IActionResult IndexC1()
        {
            ViewBag.APT = "C1";
            var model = new UserApproverViewModel();
            return View(model);
        }

        public IActionResult IndexC2()
        {
            ViewBag.APT = "C2";
            var model = new UserApproverViewModel();
            return View(model);
        }

        public IActionResult IndexAPP()
        {
            ViewBag.APT = "APP";
            var model = new UserApproverViewModel();
            return View(model);
        }

        public async Task<IActionResult> LoadAll(string apt)
        {
            var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

            if (response.Succeeded)
            {
                var allUsers = _mapper.Map<List<UserApproverViewModel>>(response.Data);

                var viewModel = (from a1 in allUsers
                                 join a2 in _userManager.Users on a1.UserId equals a2.Id
                                 select new UserApproverViewModel
                                 {
                                     Id = a1.Id,
                                     EmailAddress = a2.Email,
                                     CompanyName = a1.CompanyName,
                                     DepartmentName = a1.DepartmentName,
                                     ApprovalType = a1.ApprovalType
                                 }).ToList();

                if (apt == "C1")
                {
                    viewModel = viewModel.Where(a => a.ApprovalType == "C1").ToList();
                    return PartialView("_ViewAll_C1", viewModel);
                }
                if (apt == "C2")
                {
                    viewModel = viewModel.Where(a => a.ApprovalType == "C2").ToList();
                    return PartialView("_ViewAll_C2", viewModel);
                }
                if (apt == "APPR")
                {
                    viewModel = viewModel.Where(a => a.ApprovalType == "APP").ToList();
                    return PartialView("_ViewAll_APP", viewModel);
                }
            }
            return null;
        }

        public async Task<IActionResult> LoadAllC1(string apt)
        {
            var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

            if (response.Succeeded)
            {
                var allUsers = _mapper.Map<List<UserApproverViewModel>>(response.Data);

                var viewModel = (from a1 in allUsers
                                 join a2 in _userManager.Users on a1.UserId equals a2.Id
                                 select new UserApproverViewModel
                                 {
                                     Id = a1.Id,
                                     EmailAddress = a2.Email,
                                     CompanyName = a1.CompanyName,
                                     DepartmentName = a1.DepartmentName,
                                     ApprovalType = a1.ApprovalType
                                 }).ToList();

                viewModel = viewModel.Where(a => a.ApprovalType == "C1").ToList();
                return PartialView("_ViewAll_C1", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAllC2(string apt)
        {
            var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

            if (response.Succeeded)
            {
                var allUsers = _mapper.Map<List<UserApproverViewModel>>(response.Data);

                var viewModel = (from a1 in allUsers
                                 join a2 in _userManager.Users on a1.UserId equals a2.Id
                                 select new UserApproverViewModel
                                 {
                                     Id = a1.Id,
                                     EmailAddress = a2.Email,
                                     CompanyName = a1.CompanyName,
                                     DepartmentName = a1.DepartmentName,
                                     ApprovalType = a1.ApprovalType
                                 }).ToList();

                viewModel = viewModel.Where(a => a.ApprovalType == "C2").ToList();
                return PartialView("_ViewAll_C2", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAllAPP(string apt)
        {
            var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

            if (response.Succeeded)
            {
                var allUsers = _mapper.Map<List<UserApproverViewModel>>(response.Data);

                var viewModel = (from a1 in allUsers
                                 join a2 in _userManager.Users on a1.UserId equals a2.Id
                                 select new UserApproverViewModel
                                 {
                                     Id = a1.Id,
                                     EmailAddress = a2.Email,
                                     CompanyName = a1.CompanyName,
                                     DepartmentName = a1.DepartmentName,
                                     ApprovalType = a1.ApprovalType
                                 }).ToList();

                viewModel = viewModel.Where(a => a.ApprovalType == "APP").ToList();
                return PartialView("_ViewAll_APP", viewModel);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEditC1(int id = 0)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var responseCompanies = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var responseDepartments = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            var roleM = _roleManager.Roles.Where(a => a.Name != "B1");

            if (id == 0)
            {
                var userViewModel = new UserApproverViewModel();

                if (responseDepartments.Succeeded)
                {
                    var departmentViewModel = (from a1 in responseDepartments.Data.Where(a => a.Name != "All Departments")
                                               select new
                                               {
                                                   a1.Id,
                                                   a1.Name
                                               }).OrderBy(a => a.Name).ToList();

                    userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");
                }

                if (currentUser.UserName == "superadmin")
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
                                        }).OrderBy(a => a.Name).ToList();

                userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

                #region Filter Users By Role = B1
                var allUsers = _userManager.Users.ToList();

                var allUsersWithRoles = allUsers.Select(c => new UserApproverViewModel()
                {
                    UserId = c.Id,
                    EmailAddress = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
                }).ToList();

                var concurred1User = allUsersWithRoles.Where(a => a.Role.Contains("B1")).ToList();
                userViewModel.UserList = new SelectList(concurred1User, "UserId", "EmailAddress");
                #endregion

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AddEditC1", userViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetUserApproverByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var userViewModel = _mapper.Map<UserApproverViewModel>(response.Data);
                    if (responseDepartments.Succeeded)
                    {
                        var departmentViewModel = (from a1 in responseDepartments.Data.Where(a => a.Name != "All Departments")
                                                   select new
                                                   {
                                                       a1.Id,
                                                       a1.Name
                                                   }).OrderBy(a => a.Name).ToList();

                        userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");
                    }

                    if (currentUser.UserName == "superadmin")
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
                                            }).OrderBy(a => a.Name).ToList();

                    userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

                    //var concurred1User = _userManager.Users.ToList();
                    //userViewModel.UserList = new SelectList(concurred1User, "Id", "Email");

                    #region Filter Users By Role = B1
                    var allUsers = _userManager.Users.ToList();


                    var allUsersWithRoles = allUsers.Select(c => new UserApproverViewModel()
                    {
                        UserId = c.Id,
                        EmailAddress = c.Email,
                        Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
                    }).ToList();

                    var concurred1User = allUsersWithRoles.Where(a => a.Role.Contains("B1")).ToList();

                    userViewModel.UserList = new SelectList(concurred1User, "UserId", "EmailAddress");
                    #endregion


                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AddEditC1", userViewModel) });
                }
                return null;
            }
        }

        public async Task<JsonResult> OnGetCreateOrEditC2(int id = 0)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var responseCompanies = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var responseDepartments = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            var roleM = _roleManager.Roles.Where(a => a.Name != "B1");

            if (id == 0)
            {
                var userViewModel = new UserApproverViewModel();

                if (responseDepartments.Succeeded)
                {
                    var departmentViewModel = (from a1 in responseDepartments.Data.Where(a => a.Name != "All Departments")
                                               select new
                                               {
                                                   a1.Id,
                                                   a1.Name
                                               }).OrderBy(a => a.Name).ToList();

                    userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");
                }

                if (currentUser.UserName == "superadmin")
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
                                        }).OrderBy(a => a.Name).ToList();

                userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

                #region Filter Users By Role = B1
                var allUsers = _userManager.Users.ToList();

                var allUsersWithRoles = allUsers.Select(c => new UserApproverViewModel()
                {
                    UserId = c.Id,
                    EmailAddress = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
                }).ToList();

                var concurred1User = allUsersWithRoles.Where(a => a.Role.Contains("B1")).ToList();
                userViewModel.UserList = new SelectList(concurred1User, "UserId", "EmailAddress");
                #endregion

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AddEditC2", userViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetUserApproverByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var userViewModel = _mapper.Map<UserApproverViewModel>(response.Data);
                    if (responseDepartments.Succeeded)
                    {
                        var departmentViewModel = (from a1 in responseDepartments.Data.Where(a => a.Name != "All Departments")
                                                   select new
                                                   {
                                                       a1.Id,
                                                       a1.Name
                                                   }).OrderBy(a => a.Name).ToList();

                        userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");
                    }

                    if (currentUser.UserName == "superadmin")
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
                                            }).OrderBy(a => a.Name).ToList();

                    userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

                    //var concurred1User = _userManager.Users.ToList();
                    //userViewModel.UserList = new SelectList(concurred1User, "Id", "Email");

                    #region Filter Users By Role = B1
                    var allUsers = _userManager.Users.ToList();


                    var allUsersWithRoles = allUsers.Select(c => new UserApproverViewModel()
                    {
                        UserId = c.Id,
                        EmailAddress = c.Email,
                        Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
                    }).ToList();

                    var concurred1User = allUsersWithRoles.Where(a => a.Role.Contains("B1")).ToList();

                    userViewModel.UserList = new SelectList(concurred1User, "UserId", "EmailAddress");
                    #endregion


                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AddEditC2", userViewModel) });
                }
                return null;
            }
        }
                                      
        public async Task<JsonResult> OnGetCreateOrEditAPP(int id = 0)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var responseCompanies = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var responseDepartments = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            // var roleM = _roleManager.Roles.Where(a => a.Name != "B1");

            if (id == 0)
            {
                var userViewModel = new UserApproverViewModel();

                if (responseDepartments.Succeeded)
                {
                    var departmentViewModel = (from a1 in responseDepartments.Data.Where(a => a.Name != "All Departments")
                                               select new
                                               {
                                                   a1.Id,
                                                   a1.Name
                                               }).OrderBy(a => a.Name).ToList();

                    userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");
                }

                if (currentUser.UserName == "superadmin")
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
                                        }).OrderBy(a => a.Name).ToList();

                userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

                #region Filter Users By Role = B1
                var allUsers = _userManager.Users.ToList();

                var allUsersWithRoles = allUsers.Select(c => new UserApproverViewModel()
                {
                    UserId = c.Id,
                    EmailAddress = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
                }).ToList();

                var concurred1User = allUsersWithRoles.Where(a => a.Role.Contains("B1")).ToList();
                userViewModel.UserList = new SelectList(concurred1User, "UserId", "EmailAddress");
                #endregion

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AddEditAPP", userViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetUserApproverByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var userViewModel = _mapper.Map<UserApproverViewModel>(response.Data);
                    if (responseDepartments.Succeeded)
                    {
                        var departmentViewModel = (from a1 in responseDepartments.Data.Where(a => a.Name != "All Departments")
                                                   select new
                                                   {
                                                       a1.Id,
                                                       a1.Name
                                                   }).OrderBy(a => a.Name).ToList();

                        userViewModel.UserDepartments = new SelectList(departmentViewModel, "Id", "Name");
                    }

                    if (currentUser.UserName == "superadmin")
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
                                            }).OrderBy(a => a.Name).ToList();

                    userViewModel.UserCompanies = new SelectList(companyViewModel, "Id", "Name");

                    //var concurred1User = _userManager.Users.ToList();
                    //userViewModel.UserList = new SelectList(concurred1User, "Id", "Email");

                    #region Filter Users By Role = B1
                    var allUsers = _userManager.Users.ToList();


                    var allUsersWithRoles = allUsers.Select(c => new UserApproverViewModel()
                    {
                        UserId = c.Id,
                        EmailAddress = c.Email,
                        Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray())
                    }).ToList();

                    var concurred1User = allUsersWithRoles.Where(a => a.Role.Contains("B1")).ToList();

                    userViewModel.UserList = new SelectList(concurred1User, "UserId", "EmailAddress");
                    #endregion


                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AddEditAPP", userViewModel) });
                }
                return null;
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> OnPostCreateOrEdit(int id, UserApproverViewModel userApproverViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == 0)
        //        {
        //            var createUserApproverVCommand = _mapper.Map<CreateUserApproverCommand>(userApproverViewModel);
        //            var result = await _mediator.Send(createUserApproverVCommand);
        //            if (result.Succeeded)
        //            {
        //                id = result.Data;
        //                _notify.Success($"User Approver with ID {result.Data} Created.");
        //            }
        //            else _notify.Error(result.Message);
        //        }
        //        else
        //        {
        //            var updateUserApproverCommand = _mapper.Map<UpdateUserApproverCommand>(userApproverViewModel);
        //            var result = await _mediator.Send(updateUserApproverCommand);
        //            if (result.Succeeded) _notify.Information($"User Approver with ID {result.Data} Updated.");
        //        }
        //        var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

        //        if (response.Succeeded)
        //        {


        //            var viewModel = _mapper.Map<List<UserApproverViewModel>>(response.Data);


        //            var viewModel2 = (from a1 in viewModel
        //                              join a2 in _userManager.Users on a1.UserId equals a2.Id
        //                             select new UserApproverViewModel
        //                             {
        //                                 Id = a1.Id,
        //                                 EmailAddress = a2.Email,
        //                                 CompanyName = a1.CompanyName,
        //                                 DepartmentName = a1.DepartmentName,
        //                                 ApprovalType = a1.ApprovalType
        //                             }).ToList();


        //            if(userApproverViewModel.ApprovalType == "C1")
        //            {
        //                viewModel2 = viewModel2.Where(a => a.ApprovalType == "C1").ToList();
        //                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_C1", viewModel2);
        //                return new JsonResult(new { isValid = true, html = html });
        //            }
        //            if (userApproverViewModel.ApprovalType == "C2")
        //            {
        //                viewModel2 = viewModel2.Where(a => a.ApprovalType == "C2").ToList();
        //                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_C2", viewModel2);
        //                return new JsonResult(new { isValid = true, html = html });
        //            }
        //            if (userApproverViewModel.ApprovalType == "APP")
        //            {
        //                viewModel2 = viewModel2.Where(a => a.ApprovalType == "APP").ToList();
        //                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_APP", viewModel2);
        //                return new JsonResult(new { isValid = true, html = html });
        //            }
        //            return null;

        //        }
        //        else
        //        {
        //            _notify.Error(response.Message);
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        var html = await _viewRenderer.RenderViewToStringAsync("_AddOrEditC1", userApproverViewModel);
        //        return new JsonResult(new { isValid = false, html = html });
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEditC1(int id, UserApproverViewModel userApproverViewModel)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createUserApproverVCommand = _mapper.Map<CreateUserApproverCommand>(userApproverViewModel);
                    var result = await _mediator.Send(createUserApproverVCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"User Approver with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateUserApproverCommand = _mapper.Map<UpdateUserApproverCommand>(userApproverViewModel);
                    var result = await _mediator.Send(updateUserApproverCommand);
                    if (result.Succeeded) _notify.Information($"User Approver with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

                if (response.Succeeded)
                {


                    var viewModel = _mapper.Map<List<UserApproverViewModel>>(response.Data);


                    var viewModel2 = (from a1 in viewModel
                                      join a2 in _userManager.Users on a1.UserId equals a2.Id
                                      select new UserApproverViewModel
                                      {
                                          Id = a1.Id,
                                          EmailAddress = a2.Email,
                                          CompanyName = a1.CompanyName,
                                          DepartmentName = a1.DepartmentName,
                                          ApprovalType = a1.ApprovalType
                                      }).Where( a => a.ApprovalType == "C1").ToList();

                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_C1", viewModel2);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_AddEditC1", userApproverViewModel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEditC2(int id, UserApproverViewModel userApproverViewModel)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createUserApproverVCommand = _mapper.Map<CreateUserApproverCommand>(userApproverViewModel);
                    var result = await _mediator.Send(createUserApproverVCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"User Approver with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateUserApproverCommand = _mapper.Map<UpdateUserApproverCommand>(userApproverViewModel);
                    var result = await _mediator.Send(updateUserApproverCommand);
                    if (result.Succeeded) _notify.Information($"User Approver with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

                if (response.Succeeded)
                {


                    var viewModel = _mapper.Map<List<UserApproverViewModel>>(response.Data);


                    var viewModel2 = (from a1 in viewModel
                                      join a2 in _userManager.Users on a1.UserId equals a2.Id
                                      select new UserApproverViewModel
                                      {
                                          Id = a1.Id,
                                          EmailAddress = a2.Email,
                                          CompanyName = a1.CompanyName,
                                          DepartmentName = a1.DepartmentName,
                                          ApprovalType = a1.ApprovalType
                                      }).Where(a => a.ApprovalType == "C2").ToList();

                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_C2", viewModel2);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_AddEditC1", userApproverViewModel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEditAPP(int id, UserApproverViewModel userApproverViewModel)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createUserApproverVCommand = _mapper.Map<CreateUserApproverCommand>(userApproverViewModel);
                    var result = await _mediator.Send(createUserApproverVCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"User Approver with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateUserApproverCommand = _mapper.Map<UpdateUserApproverCommand>(userApproverViewModel);
                    var result = await _mediator.Send(updateUserApproverCommand);
                    if (result.Succeeded) _notify.Information($"User Approver with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllUserApproversCachedQuery());

                if (response.Succeeded)
                {


                    var viewModel = _mapper.Map<List<UserApproverViewModel>>(response.Data);


                    var viewModel2 = (from a1 in viewModel
                                      join a2 in _userManager.Users on a1.UserId equals a2.Id
                                      select new UserApproverViewModel
                                      {
                                          Id = a1.Id,
                                          EmailAddress = a2.Email,
                                          CompanyName = a1.CompanyName,
                                          DepartmentName = a1.DepartmentName,
                                          ApprovalType = a1.ApprovalType
                                      }).Where(a => a.ApprovalType == "APP").ToList();

                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_APP", viewModel2);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_AddEditAPP", userApproverViewModel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDeleteC1(int id)
        {
            var deleteCommand = await _mediator.Send(new DeleteUserApproverCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"User Approver with Id {id} Deleted.");
                var response = await _mediator.Send(new GetAllUserApproversCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<UserApproverViewModel>>(response.Data);

                    var viewModel2 = (from a1 in viewModel
                                      join a2 in _userManager.Users on a1.UserId equals a2.Id
                                      select new UserApproverViewModel
                                      {
                                          Id = a1.Id,
                                          EmailAddress = a2.Email,
                                          CompanyName = a1.CompanyName,
                                          DepartmentName = a1.DepartmentName,
                                          ApprovalType = a1.ApprovalType
                                      }).Where( a => a.ApprovalType == "C1").ToList();


                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_C1", viewModel2);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                _notify.Error(deleteCommand.Message);
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDeleteC2(int id)
        {
            var deleteCommand = await _mediator.Send(new DeleteUserApproverCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"User Approver with Id {id} Deleted.");
                var response = await _mediator.Send(new GetAllUserApproversCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<UserApproverViewModel>>(response.Data);

                    var viewModel2 = (from a1 in viewModel
                                      join a2 in _userManager.Users on a1.UserId equals a2.Id
                                      select new UserApproverViewModel
                                      {
                                          Id = a1.Id,
                                          EmailAddress = a2.Email,
                                          CompanyName = a1.CompanyName,
                                          DepartmentName = a1.DepartmentName,
                                          ApprovalType = a1.ApprovalType
                                      }).Where(a => a.ApprovalType == "C2").ToList();


                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll_C2", viewModel2);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                _notify.Error(deleteCommand.Message);
                return null;
            }
        }
    }
}

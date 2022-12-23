using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManuals.Commands.Create;
using EDocSys.Application.Features.SafetyHealthManuals.Commands.Delete;
using EDocSys.Application.Features.SafetyHealthManuals.Commands.Update;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetById;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.Documentation.Models;
using EDocSys.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using MediatR;
using EDocSys.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Html;
using EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllCached;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class SafetyHealthManualController : BaseController<SafetyHealthManualController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public SafetyHealthManualController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env,
            IWebHostEnvironment hostEnvironment)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            webHostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.RoleAB1 = false;
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
                ViewBag.RoleAB1 = true;
            }
            if (rolesList.Contains("B1"))
            {
                ViewBag.RoleB1 = true;
                ViewBag.RoleAB1 = true;
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
            var model = new SafetyHealthManualViewModel();
            return View(model);
        }

        //public IActionResult Preview1()
        //{
        //    var model = new SafetyHealthManualViewModel();
        //    return View(model);
        //}

        [Authorize(Policy = "CanViewSafetyHealthManual")]
        public async Task<IActionResult> Preview(int id, bool print = false, SafetyHealthManualViewModel shManual = null)
        {
            ViewBag.RoleAB1 = false;
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
            List<string> rolesListComp = new List<string>();
            List<string> rolesListDept = new List<string>();
            var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });
            var adg = _context.SafetyHealthManualStatus;
            var statusById = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == id).ToList();

            if (response.Succeeded)
            {
                var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                if (print || shManual.PrintCount != 0)
                {
                    if (shManual.PrintCount != safetyHealthManualViewModel.PrintCount)
                        safetyHealthManualViewModel.PrintCount = shManual.PrintCount;
                    else
                        safetyHealthManualViewModel.PrintCount = safetyHealthManualViewModel.PrintCount + 1;
                    var updateSafetyHealthManualCommand = _mapper.Map<UpdateSafetyHealthManualCommand>(safetyHealthManualViewModel);
                    var result = await _mediator.Send(updateSafetyHealthManualCommand);
                    return RedirectToAction("Preview", new { Id = id });
                }
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == safetyHealthManualViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        var QADeptId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
                        if (user.UserDepartmentId == QADeptId)
                            rolesListDept.AddRange(roles);
                    }
                }
                if (rolesList.Contains("A"))
                {
                    ViewBag.RoleA = true;
                    ViewBag.RoleAB1 = true;
                }
                if (rolesListComp.Contains("B1"))
                {
                    ViewBag.RoleB1 = true;
                    ViewBag.RoleAB1 = true;
                }
                if (rolesListComp.Contains("B2"))
                {
                    ViewBag.RoleB2 = true;
                }
                if (rolesListDept.Contains("C"))
                {
                    ViewBag.RoleC = true;
                }
                if (rolesListComp.Contains("E"))
                {
                    ViewBag.RoleE = true;
                }
                if (rolesListDept.Contains("D"))
                {
                    ViewBag.RoleD = true;
                }
                if (rolesList.Contains("SuperAdmin"))
                {
                    ViewBag.RoleSA = true;
                }
                // safetyHealthManualViewModel.pro String.Format("{0:y yy yyy yyyy}", dt);  // "8 08 008 2008"   year

                // safetyHealthManualViewModel.PreparedByDate = String.Format("{0:dd/mm/yyyy}", safetyHealthManualViewModel.PreparedByDate);

                if (statusById.Count() != 0)
                {
                    var StatusId = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    safetyHealthManualViewModel.SafetyHealthManualStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    safetyHealthManualViewModel.SafetyHealthManualStatusView = "New";
                }

                if (safetyHealthManualViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == safetyHealthManualViewModel.Concurred1).SingleOrDefault();
                    safetyHealthManualViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    safetyHealthManualViewModel.Concurred1 = concurred1User.Id;
                    safetyHealthManualViewModel.PositionC1 = concurred1User.Position;

                    if (statusById.Count() != 0)
                    {
                        var c1Status = statusById.Where(a => a.DocumentStatusId == 2).OrderBy(a => a.CreatedOn).ToList();
                        if (c1Status.Count > 0)
                        {
                            var c1StatusDt = c1Status.Select(a => a.CreatedOn).Last();
                            var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                            if (rejectStatus.Count > 0)
                            {
                                var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                                if (c1StatusDt > rejectStatusDt)
                                {
                                    safetyHealthManualViewModel.DateApprovedC1 = c1StatusDt;
                                }
                            }
                            else
                            {
                                safetyHealthManualViewModel.DateApprovedC1 = c1StatusDt;
                            }
                        }
                    }
                }

                if (safetyHealthManualViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == safetyHealthManualViewModel.Concurred2).SingleOrDefault();
                    safetyHealthManualViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                    safetyHealthManualViewModel.Concurred2 = concurred2User.Id;
                    safetyHealthManualViewModel.PositionC2 = concurred2User.Position;

                    if (statusById.Count() != 0)
                    {
                        var c1Status = statusById.Where(a => a.DocumentStatusId == 3).OrderBy(a => a.CreatedOn).ToList();
                        if (c1Status.Count > 0)
                        {
                            var c1StatusDt = c1Status.Select(a => a.CreatedOn).Last();
                            var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                            if (rejectStatus.Count > 0)
                            {
                                var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                                if (c1StatusDt > rejectStatusDt)
                                {
                                    safetyHealthManualViewModel.DateApprovedC2 = c1StatusDt;
                                }
                            }
                            else
                            {
                                safetyHealthManualViewModel.DateApprovedC2 = c1StatusDt;
                            }
                        }
                    }
                }

                if (safetyHealthManualViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == safetyHealthManualViewModel.ApprovedBy).SingleOrDefault();
                    safetyHealthManualViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    safetyHealthManualViewModel.PositionApp = ApprovedByUser.Position;

                    if (statusById.Count() != 0)
                    {
                        var StatusId = _context.WIStatus.Where(a => a.WIId == id).OrderBy(a => a.CreatedOn).Select(a => a.DocumentStatusId).Last();

                        var c1Status = statusById.Where(a => a.DocumentStatusId == 4).OrderBy(a => a.CreatedOn).ToList();
                        if (c1Status.Count > 0)
                        {
                            var c1StatusDt = c1Status.Select(a => a.CreatedOn).Last();
                            var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                            if (rejectStatus.Count > 0)
                            {
                                var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                                if (c1StatusDt > rejectStatusDt)
                                {
                                    safetyHealthManualViewModel.DateApprovedAPP = c1StatusDt;
                                }
                            }
                            else
                            {
                                safetyHealthManualViewModel.DateApprovedAPP = c1StatusDt;
                            }
                        }
                    }
                }

                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                //var currentyUserId = currentUser.Id;
                var concurred1 = response.Data.Concurred1;
                var concurred2 = response.Data.Concurred2;
                var app = response.Data.ApprovedBy;

                if (users.Select(s => s.Id).Contains(concurred1))
                {
                    ViewBag.IsConcurred1 = true;
                }

                if (users.Select(s => s.Id).Contains(concurred2))
                {
                    ViewBag.IsConcurred2 = true;
                }

                if (users.Select(s => s.Id).Contains(app))
                {
                    ViewBag.IsApp = true;
                }





                return View(safetyHealthManualViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditSafetyHealthManual")]
        public async Task<IActionResult> CreateOrEdit(int id = 0, bool rev = false)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }

            var companiesResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
            var qaId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            if (id == 0)
            {
                var safetyHealthManualViewModel = new SafetyHealthManualViewModel();
                safetyHealthManualViewModel.EstalishedDate = DateTime.Now;
                safetyHealthManualViewModel.PreparedByDate = DateTime.Now;
                safetyHealthManualViewModel.Category = "Documentation";
                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }

                    safetyHealthManualViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }


                // Concurred 1
                var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC1 = (from a1 in responseC1
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred1Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                safetyHealthManualViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC2 = (from a1 in responseC2
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                safetyHealthManualViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                var userViewModelAPP = (from a1 in responseAPP
                                        join a2 in _userManager.Users on a1.UserId equals a2.Id
                                        select new UserApproverViewModel
                                        {
                                            UserApproveBy = a1.UserId,
                                            FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                        }).OrderBy(a => a.Email).ToList();
                safetyHealthManualViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "DOC_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;
                return View(safetyHealthManualViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                    var safetyHealthManualViewModelOld = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                    if (rev)
                    {
                        safetyHealthManualViewModel.Id = 0;
                        var revNo = safetyHealthManualViewModel.RevisionNo != null ? safetyHealthManualViewModel.RevisionNo : 0;
                        safetyHealthManualViewModel.RevisionNo = revNo + 1;
                        safetyHealthManualViewModel.RevisionDate = DateTime.Now;
                        safetyHealthManualViewModel.EffectiveDate = null;
                        safetyHealthManualViewModel.EstalishedDate = DateTime.Now;
                        safetyHealthManualViewModel.PreparedByDate = DateTime.Now;
                        safetyHealthManualViewModel.ArchiveId = safetyHealthManualViewModelOld.Id;
                    }
                    //safetyHealthManualViewModel.RevisionNo = safetyHealthManualViewModel.RevisionNo == null ? 1 : (safetyHealthManualViewModel.RevisionNo + 1);
                    //safetyHealthManualViewModel.RevisionDate = DateTime.Now;

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        safetyHealthManualViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }

                    // Concurred 1
                    var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC1 = (from a1 in responseC1
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred1Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    safetyHealthManualViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    safetyHealthManualViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelAPP = (from a1 in responseAPP
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserApproveBy = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    safetyHealthManualViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");


                    return View(safetyHealthManualViewModel);
                }
                return null;
            }
        }

        public async Task<IActionResult> LoadAll1(int id)
        {
            ViewBag.RoleAB1 = false;
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
            List<string> rolesListComp = new List<string>();
            List<string> rolesListDept = new List<string>();
            var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });

            var statusById = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == id).ToList();

            if (response.Succeeded)
            {
                var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == safetyHealthManualViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        var QADeptId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
                        if (user.UserDepartmentId == QADeptId)
                            rolesListDept.AddRange(roles);
                    }
                }
                if (rolesList.Contains("A"))
                {
                    ViewBag.RoleA = true;
                    ViewBag.RoleAB1 = true;
                }
                if (rolesListComp.Contains("B1"))
                {
                    ViewBag.RoleB1 = true;
                    ViewBag.RoleAB1 = true;
                }
                if (rolesListComp.Contains("B2"))
                {
                    ViewBag.RoleB2 = true;
                }
                if (rolesListDept.Contains("C"))
                {
                    ViewBag.RoleC = true;
                }
                if (rolesListComp.Contains("E"))
                {
                    ViewBag.RoleE = true;
                }
                if (rolesListDept.Contains("D"))
                {
                    ViewBag.RoleD = true;
                }
                if (rolesList.Contains("SuperAdmin"))
                {
                    ViewBag.RoleSA = true;
                }
                if (statusById.Count() != 0)
                {
                    var StatusId = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    safetyHealthManualViewModel.SafetyHealthManualStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    safetyHealthManualViewModel.SafetyHealthManualStatusView = "New";
                }

                if (safetyHealthManualViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == safetyHealthManualViewModel.Concurred1).SingleOrDefault();
                    safetyHealthManualViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    safetyHealthManualViewModel.Concurred1 = concurred1User.Id;
                }

                if (safetyHealthManualViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == safetyHealthManualViewModel.Concurred2).SingleOrDefault();
                    safetyHealthManualViewModel.Concurred2 = concurred2User.FirstName + " " + concurred2User.LastName;
                }

                if (safetyHealthManualViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == safetyHealthManualViewModel.ApprovedBy).SingleOrDefault();
                    safetyHealthManualViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                }

                return PartialView("_ViewSafetyHealthManualById", safetyHealthManualViewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll()
        {
            ViewBag.RoleA = false;
            ViewBag.RoleB1 = false;
            ViewBag.RoleE = false;
            ViewBag.RoleD = false;
            ViewBag.RoleSA = false;
            ViewBag.userIds = "";
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            List<int> listComp = new List<int>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
                listComp.Add(user.UserCompanyId);
                ViewBag.userIds = ViewBag.userIds != "" ? (ViewBag.userIds + "," + user.Id) : user.Id;
            }
            if (rolesList.Contains("A"))
            {
                ViewBag.RoleA = true;
            }
            if (rolesList.Contains("B1"))
            {
                ViewBag.RoleB1 = true;
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

            var response = await _mediator.Send(new GetAllSafetyHealthManualsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SafetyHealthManualViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId))).ToList();

                if (!rolesList.Contains("A") && !rolesList.Contains("SuperAdmin"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }

                foreach (SafetyHealthManualViewModel item in viewModel)
                {
                    var psStatat = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        item.SafetyHealthManualStatusView = StatusId.DocumentStatus.Name;
                    }
                    else
                    {
                        item.SafetyHealthManualStatusView = "New";
                    }
                }
                if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }
                else if(rolesList.Contains("C"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && a.SafetyHealthManualStatusView == "Approved").ToList();
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
        {
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            if (id == 0)
            {
                var safetyHealthManualViewModel = new SafetyHealthManualViewModel();
                safetyHealthManualViewModel.Category = "Documentation";
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", safetyHealthManualViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", safetyHealthManualViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, SafetyHealthManualViewModel SafetyHealthManual)
        {
            // SafetyHealthManual.EffectiveDate.Value.ToString("g");



            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createSafetyHealthManualCommand = _mapper.Map<CreateSafetyHealthManualCommand>(SafetyHealthManual);
                    var result = await _mediator.Send(createSafetyHealthManualCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Safety and Health Manual with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateSafetyHealthManualCommand = _mapper.Map<UpdateSafetyHealthManualCommand>(SafetyHealthManual);
                    var result = await _mediator.Send(updateSafetyHealthManualCommand);
                    if (result.Succeeded) _notify.Information($"Safety and Health Manual with ID {result.Data} Updated.");
                }
                if (SafetyHealthManual.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = SafetyHealthManual.ArchiveId });
                    if (response2.Succeeded)
                    {
                        var safetyHealthManualViewModelOld = _mapper.Map<SafetyHealthManualViewModel>(response2.Data);
                        safetyHealthManualViewModelOld.IsArchive = true;
                        var archiveDt = DateTime.Now.AddDays(30);
                        safetyHealthManualViewModelOld.ArchiveDate = archiveDt;
                        var updateSafetyHealthManualCommand = _mapper.Map<UpdateSafetyHealthManualCommand>(safetyHealthManualViewModelOld);
                        var result2 = await _mediator.Send(updateSafetyHealthManualCommand);
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateSafetyHealthManualImageCommand() { Id = id, Image = image });
                }

                var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                    return RedirectToAction("Preview", new { Id = id });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("Create", SafetyHealthManual);
                return View(nameof(IndexAsync));
            }
        }
        [HttpPost]
        public async Task<JsonResult> OnPostDeactivate(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);
                    safetyHealthManualViewModel.IsActive = false;
                    var updateSafetyHealthManualCommand = _mapper.Map<UpdateSafetyHealthManualCommand>(safetyHealthManualViewModel);
                    var result = await _mediator.Send(updateSafetyHealthManualCommand);
                    if (result.Succeeded)
                        _notify.Information($"Safety and Health Manual with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllSafetyHealthManualsCachedQuery());
                    if (response2.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<SafetyHealthManualViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                        }
                        foreach (SafetyHealthManualViewModel item in viewModel)
                        {
                            var psStatat = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == item.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.SafetyHealthManualStatusView = StatusId.DocumentStatus.Name;
                            }
                            else
                            {
                                item.SafetyHealthManualStatusView = "New";
                            }
                        }
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                }
            }
            return null;
        }

        //[HttpPost]
        //public async Task<JsonResult> OnPostDelete(int id)
        //{
        //    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
        //    var user = await _userManager.FindByIdAsync(currentUser.Id);
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var rolesList = roles.ToList();

        //    var deleteCommand = await _mediator.Send(new DeleteSafetyHealthManualCommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Safety and Health Manual with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllSafetyHealthManualsCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<SafetyHealthManualViewModel>>(response.Data);

        //            if (rolesList.Contains("D"))
        //            {
        //                viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
        //            }

        //            foreach (SafetyHealthManualViewModel item in viewModel)
        //            {
        //                var psStatat = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == item.Id).ToList();

        //                if (psStatat.Count != 0)
        //                {
        //                    var StatusId = _context.SafetyHealthManualStatus.Where(a => a.SafetyHealthManualId == item.Id).OrderBy(a => a.CreatedOn)
        //                        .Include(a => a.DocumentStatus)
        //                        .Last();
        //                    item.SafetyHealthManualStatusView = StatusId.DocumentStatus.Name;
        //                }
        //                else
        //                {
        //                    item.SafetyHealthManualStatusView = "New";
        //                }
        //            }

        //            var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
        //            return new JsonResult(new { isValid = true, html = html });
        //        }
        //        else
        //        {
        //            _notify.Error(response.Message);
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        _notify.Error(deleteCommand.Message);
        //        return null;
        //    }
        //}

        public ActionResult UploadImage(List<IFormFile> files)
        {
            var filepath = "";

            foreach (IFormFile file in Request.Form.Files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var fileModel = new FileOnDatabaseModel
                {
                    CreatedOn = DateTime.UtcNow,
                    FileType = file.ContentType,
                    Extension = extension,
                    Name = fileName,
                    Description = "--"

                };
                using (var dataStream = new MemoryStream())
                {
                    file.CopyTo(dataStream);
                    fileModel.Data = dataStream.ToArray();
                }

                filepath = ViewImage(fileModel.Data);
            }

            return Json(new { url = filepath });
        }

        [NonAction]

        private string ViewImage(byte[] arrayImage)
        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
            return "data:image/png;base64," + base64String;

        }

        public async Task<JsonResult> OnGetAssignment(int id)
        {
            var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Assignment", safetyHealthManualViewModel) });
            }

            return null;

        }

        public async Task<JsonResult> OnGetAssignConcurred1(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }
            var departmentsResponseSH1 = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModelSH1 = _mapper.Map<List<DepartmentViewModel>>(departmentsResponseSH1.Data);
            var qaId = departmentViewModelSH1.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptIdSH1 = departmentViewModelSH1.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });

            var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (a.DepartmentId == qaId || a.DepartmentId == allDeptIdSH1)).ToList();

            if (response.Succeeded)
            {
                var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);

                var userViewModel = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                safetyHealthManualViewModel.UserList = new SelectList(userViewModel, "UserConcurred1Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred1", safetyHealthManualViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignConcurred2(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }
            var departmentsResponseSH2 = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModelSH2 = _mapper.Map<List<DepartmentViewModel>>(departmentsResponseSH2.Data);
            var qaId = departmentViewModelSH2.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptIdSH2 = departmentViewModelSH2.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });

            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == qaId || a.DepartmentId == allDeptIdSH2)).ToList();

            if (response.Succeeded)
            {
                var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);

                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();

                safetyHealthManualViewModel.UserList = new SelectList(userViewModel, "UserConcurred2Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred2", safetyHealthManualViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignApprovedBy(int id)
        {
            var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });

            if (response.Succeeded)
            {
                var safetyHealthManualViewModel = _mapper.Map<SafetyHealthManualViewModel>(response.Data);


                var userViewModel = (from a1 in await _userManager.Users.ToListAsync()
                                     select new UserViewModel
                                     {
                                         UserApproveBy = a1.Id,
                                         FullName = a1.LastName + " " + a1.FirstName + " (" + a1.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                safetyHealthManualViewModel.UserList = new SelectList(userViewModel, "UserApproveBy", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignApprovedBy", safetyHealthManualViewModel) });
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmitAssignment(int id, SafetyHealthManualViewModel SafetyHealthManual)
        {
            if (id == 0)
            {
                var createSafetyHealthManualCommand = _mapper.Map<CreateSafetyHealthManualCommand>(SafetyHealthManual);
                var result = await _mediator.Send(createSafetyHealthManualCommand);
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Safety and Health Manual with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
            }

            else
            {
                SafetyHealthManual.Concurred1 = SafetyHealthManual.Concurred1;
                var updateSafetyHealthManualCommand = _mapper.Map<UpdateSafetyHealthManualCommand>(SafetyHealthManual);
                var result = await _mediator.Send(updateSafetyHealthManualCommand);
                if (result.Succeeded) _notify.Information($"Safety and Health Manual Assignment with ID {result.Data} Updated.");
            }
            var response = await _mediator.Send(new GetAllSafetyHealthManualsCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SafetyHealthManualViewModel>>(response.Data);
                var html2 = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                return new JsonResult(new { isValid = true, html = html2 });
            }
            else
            {
                _notify.Error(response.Message);
                return null;
            }
        }
    }
}

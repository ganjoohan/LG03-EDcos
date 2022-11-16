using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Commands.Create;
//using EDocSys.Application.Features.Procedures.Commands.Delete;
using EDocSys.Application.Features.Procedures.Commands.Update;
using EDocSys.Application.Features.Procedures.Queries.GetById;
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
using EDocSys.Application.Features.ProcedureStatuses.Queries.GetAllCached;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using EDocSys.Application.Features.SOPs.Commands.Create;
using EDocSys.Application.Features.WIs.Commands.Create;
using EDocSys.Application.Features.SOPs.Commands.Update;
using EDocSys.Application.Features.WIs.Commands.Update;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class ProcedureController : BaseController<ProcedureController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProcedureController(ApplicationDbContext context,
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
            var model = new ProcedureViewModel();
            return View(model);
        }

        //public IActionResult Preview1()
        //{
        //    var model = new ProcedureViewModel();
        //    return View(model);
        //}

        [Authorize(Policy = "CanViewProcedure")]
        public async Task<IActionResult> Preview(int id, bool print = false, ProcedureViewModel procedure = null)
        {
            ViewBag.RoleAB1 = false;
            ViewBag.RoleA = false;
            ViewBag.RoleB1 = false;
            ViewBag.RoleB2 = false;
            ViewBag.RoleC = false;
            ViewBag.RoleE = false;
            ViewBag.RoleD = false;
            ViewBag.RoleSA = false;
 
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var statusById = _context.ProcedureStatus.Where(a => a.ProcedureId == id).ToList();

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);
                if (print || procedure.PrintCount != 0)
                {
                    if (procedure.PrintCount != procedureViewModel.PrintCount)
                        procedureViewModel.PrintCount = procedure.PrintCount;
                    else
                        procedureViewModel.PrintCount = procedureViewModel.PrintCount + 1;
                    var updateProcedureCommand = _mapper.Map<UpdateProcedureCommand>(procedureViewModel);
                    var result = await _mediator.Send(updateProcedureCommand);
                    return RedirectToAction("Preview", new { Id = id });
                }
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
                List<string> rolesList = new List<string>();
                List<string> rolesListComp = new List<string>();
                List<string> rolesListDept = new List<string>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == procedureViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        if(user.UserDepartmentId == procedureViewModel.DepartmentId)
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
                // procedureViewModel.pro String.Format("{0:y yy yyy yyyy}", dt);  // "8 08 008 2008"   year

                // procedureViewModel.PreparedByDate = String.Format("{0:dd/mm/yyyy}", procedureViewModel.PreparedByDate);

                if (statusById.Count() != 0)
                {
                    var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    procedureViewModel.ProcedureStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    procedureViewModel.ProcedureStatusView = "New";
                }
                if (procedureViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == procedureViewModel.Concurred1).SingleOrDefault();
                    procedureViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    procedureViewModel.Concurred1 = concurred1User.Id;
                    procedureViewModel.PositionC1 = concurred1User.Position;

                    if (statusById.Count() != 0)
                    {
                        var c1Status = statusById.Where(a => a.DocumentStatusId == 2).OrderBy(a => a.CreatedOn).ToList();
                        if(c1Status.Count > 0)
                        {
                            var c1StatusDt = c1Status.Select(a => a.CreatedOn).Last();
                            var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                            if(rejectStatus.Count > 0)
                            {
                                var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();
                                
                                if (c1StatusDt > rejectStatusDt)
                                {
                                    procedureViewModel.DateApprovedC1 = c1StatusDt;
                                }
                            }
                            else
                            {
                                procedureViewModel.DateApprovedC1 = c1StatusDt;
                            }                          
                        }                       
                    }
                }

                if (procedureViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == procedureViewModel.Concurred2).SingleOrDefault();
                    procedureViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                    procedureViewModel.Concurred2 = concurred2User.Id;
                    procedureViewModel.PositionC2 = concurred2User.Position;

                    if (statusById.Count() != 0)
                    {
                        var c2Status = statusById.Where(a => a.DocumentStatusId == 3).OrderBy(a => a.CreatedOn).ToList();
                        if (c2Status.Count > 0)
                        {
                            var c2StatusDt = c2Status.Select(a => a.CreatedOn).Last();
                            var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                            if (rejectStatus.Count > 0)
                            {
                                var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                                if (c2StatusDt > rejectStatusDt)
                                {
                                    procedureViewModel.DateApprovedC2 = c2StatusDt;
                                }
                            }
                            else
                            {
                                procedureViewModel.DateApprovedC2 = c2StatusDt;
                            }
                        }
                    }
                }

                if (procedureViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == procedureViewModel.ApprovedBy).SingleOrDefault();
                    procedureViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    procedureViewModel.PositionApp = ApprovedByUser.Position;

                    if (statusById.Count() != 0)
                    {
                        var appStatus = statusById.Where(a => a.DocumentStatusId == 4).OrderBy(a => a.CreatedOn).ToList();
                        if (appStatus.Count > 0)
                        {
                            var appStatusDt = appStatus.Select(a => a.CreatedOn).Last();
                            var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                            if (rejectStatus.Count > 0)
                            {
                                var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                                if (appStatusDt > rejectStatusDt)
                                {
                                    procedureViewModel.DateApprovedAPP = appStatusDt;
                                }
                            }
                            else
                            {
                                procedureViewModel.DateApprovedAPP = appStatusDt;
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
            return View(procedureViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditProcedure")]
        public async Task<IActionResult> CreateOrEdit(int id = 0, bool rev = false, bool revAll = false)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }

            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            int allDeptId = 0;
            var companiesResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());
            if (id == 0)
            {
                var procedureViewModel = new ProcedureViewModel();
                procedureViewModel.EstalishedDate = DateTime.Now;
                procedureViewModel.PreparedByDate = DateTime.Now;
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
                    if (rolesList.Contains("D"))
                    {
                        departmentViewModel = departmentViewModel.Where(a => users.Select(s=> s.UserDepartmentId).Contains(a.Id)).ToList();
                    }

                    procedureViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }

                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }
                    procedureViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }


                // Concurred 1
                var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s=> s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC1 = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();
                procedureViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC2 = (from a1 in responseC2
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                procedureViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelAPP = (from a1 in responseAPP
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserApproveBy = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                procedureViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "WSCP_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;
                return View(procedureViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);
                    var procedureViewModelOld = _mapper.Map<ProcedureViewModel>(response.Data);
                    if (rev)
                    {
                        procedureViewModel.Id = 0;
                        var revNo = procedureViewModel.RevisionNo != null ? procedureViewModel.RevisionNo : 0;
                        procedureViewModel.RevisionNo = revNo + 1;
                        procedureViewModel.RevisionDate = DateTime.Now;
                        procedureViewModel.EffectiveDate = null;
                        procedureViewModel.EstalishedDate = DateTime.Now;
                        procedureViewModel.PreparedByDate = DateTime.Now;
                        procedureViewModel.ArchiveId = procedureViewModelOld.Id;
                    }
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
                        procedureViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        procedureViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }

                    // Concurred 1
                    var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC1 = (from a1 in responseC1
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred1Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    procedureViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    procedureViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelAPP = (from a1 in responseAPP
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserApproveBy = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    procedureViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");


                    return View(procedureViewModel);
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
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var statusById = _context.ProcedureStatus.Where(a => a.ProcedureId == id).ToList();

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);
                List<string> rolesList = new List<string>();
                List<string> rolesListComp = new List<string>();
                List<string> rolesListDept = new List<string>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == procedureViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        if (user.UserDepartmentId == procedureViewModel.DepartmentId)
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
                    var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    procedureViewModel.ProcedureStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    procedureViewModel.ProcedureStatusView = "New";
                }

                if (procedureViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == procedureViewModel.Concurred1).SingleOrDefault();
                    procedureViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    procedureViewModel.Concurred1 = concurred1User.Id;
                }

                if (procedureViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == procedureViewModel.Concurred2).SingleOrDefault();
                    procedureViewModel.Concurred2 = concurred2User.FirstName + " " + concurred2User.LastName;
                }

                if (procedureViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == procedureViewModel.ApprovedBy).SingleOrDefault();
                    procedureViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                }

                return PartialView("_ViewProcedureById", procedureViewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll()
        {
            ViewBag.RoleAB1 = false;
            ViewBag.RoleA = false;
            ViewBag.RoleB1 = false;
            ViewBag.RoleB2 = false;
            ViewBag.RoleC = false;
            ViewBag.RoleE = false;
            ViewBag.RoleD = false;
            ViewBag.RoleSA = false;
            ViewBag.userIds = "";
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            List<int> listComp = new List<int>();
            List<int> listDept = new List<int>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
                listComp.Add(user.UserCompanyId);
                listDept.Add(user.UserDepartmentId);
                ViewBag.userIds = ViewBag.userIds != "" ? (ViewBag.userIds + "," + user.Id) : user.Id;
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
            var response = await _mediator.Send(new GetAllProceduresCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<ProcedureViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2")) ? true : listDept.Contains(a.DepartmentId))).ToList();

                // Access Category = D  
                // SOP Department Admin (Full Access by Department)
                if (rolesList.Contains("D"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                }
                else if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    // Access Category = E  
                    // QMR / Lead Auditor / SOP Company Admin (Full Access by Company)
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }
                else if (rolesList.Contains("C"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) && a.ProcedureStatusView == "Approved").ToList();
                }
                foreach (ProcedureViewModel item in viewModel)
                {
                    var psStatat = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        item.ProcedureStatusView = StatusId.DocumentStatus.Name;
                    }
                    else
                    {
                        item.ProcedureStatusView = "New";
                    }
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
                var procedureViewModel = new ProcedureViewModel();
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    procedureViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", procedureViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        procedureViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", procedureViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, ProcedureViewModel procedure)
        {
            // procedure.EffectiveDate.Value.ToString("g");
            


            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createProcedureCommand = _mapper.Map<CreateProcedureCommand>(procedure);
                    var result = await _mediator.Send(createProcedureCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Procedure with ID {result.Data} Created.");                      
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateProcedureCommand = _mapper.Map<UpdateProcedureCommand>(procedure);
                    var result = await _mediator.Send(updateProcedureCommand);
                    if (result.Succeeded) _notify.Information($"Procedure with ID {result.Data} Updated.");
                }
                if(procedure.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetProcedureByIdQuery() { Id = procedure.ArchiveId });
                    var archiveDt = DateTime.Now.AddDays(30);
                    if (response2.Succeeded)
                    {
                        var procedureViewModelOld = _mapper.Map<ProcedureViewModel>(response2.Data);
                        procedureViewModelOld.IsArchive = true;
                        procedureViewModelOld.ArchiveDate = archiveDt;
                        var updateProcedureCommandOld = _mapper.Map<UpdateProcedureCommand>(procedureViewModelOld);
                        var result2 = await _mediator.Send(updateProcedureCommandOld);

                        var response3 = await _mediator.Send(new Application.Features.SOPs.Queries.GetAllCached.GetAllSOPsCachedQuery());
                        if (response3.Succeeded)
                        {
                            var viewModelSOP = _mapper.Map<List<SOPViewModel>>(response3.Data);
                            viewModelSOP = viewModelSOP.Where(a => a.WSCPNo == procedureViewModelOld.WSCPNo && a.WSCPId == procedureViewModelOld.Id).ToList(); //&& a.SOPStatusView == "Approved").ToList();

                            foreach (SOPViewModel item in viewModelSOP)
                            {
                                item.IsArchive = true;
                                item.ArchiveDate = archiveDt;
                                var updateSOPCommandOld = _mapper.Map<UpdateSOPCommand>(item);
                                var result3A = await _mediator.Send(updateSOPCommandOld);
                                var newRevSop = item;
                                newRevSop.ArchiveId = newRevSop.Id;
                                newRevSop.Id = 0;
                                newRevSop.IsArchive = false;
                                newRevSop.ArchiveDate = null;
                                newRevSop.WSCPId = id;
                                newRevSop.Concurred1 = null;
                                newRevSop.Concurred2 = null;
                                newRevSop.ApprovedBy = null;
                                var createSOPCommand = _mapper.Map<CreateSOPCommand>(newRevSop);
                                var result3 = await _mediator.Send(createSOPCommand);
                            }
                        }
                        var response2A = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });
                        if (response2A.Succeeded)
                        {
                            var viewModelProcedure = _mapper.Map<ProcedureViewModel>(response2A.Data);
                            var response3A = await _mediator.Send(new Application.Features.SOPs.Queries.GetAllCached.GetAllSOPsCachedQuery());
                            if (response3A.Succeeded)
                            {
                                var response4 = await _mediator.Send(new Application.Features.WIs.Queries.GetAllCached.GetAllWIsCachedQuery());
                                if (response4.Succeeded)
                                {
                                    var viewModelWI = _mapper.Map<List<WIViewModel>>(response4.Data);
                                    viewModelWI = viewModelWI.Where(a => a.WSCPNo == procedureViewModelOld.WSCPNo && a.WSCPId == procedureViewModelOld.Id).ToList(); //&& a.WIStatusView == "Approved").ToList();
                                    var viewModelSOP = _mapper.Map<List<SOPViewModel>>(response3A.Data);
                                    viewModelSOP = viewModelSOP.Where(a => a.WSCPNo == viewModelProcedure.WSCPNo && a.WSCPId == viewModelProcedure.Id).ToList(); //&& a.SOPStatusView == "Approved").ToList();

                                    foreach (SOPViewModel item in viewModelSOP)
                                    {
                                        var viewModelWI2 = viewModelWI.Where(w => w.SOPId == item.ArchiveId).ToList();
                                        foreach (WIViewModel item2 in viewModelWI2)
                                        {
                                            item2.IsArchive = true;
                                            item2.ArchiveDate = archiveDt;
                                            var updateWICommandOld = _mapper.Map<UpdateWICommand>(item2);
                                            var result4A = await _mediator.Send(updateWICommandOld);
                                            var newRevWi = item2;
                                            newRevWi.ArchiveId = newRevWi.Id;
                                            newRevWi.Id = 0;
                                            newRevWi.IsArchive = false;
                                            newRevWi.ArchiveDate = null;
                                            newRevWi.WSCPId = id;
                                            newRevWi.SOPId = item.Id;

                                            var createWICommand = _mapper.Map<CreateWICommand>(newRevWi);
                                            var result4 = await _mediator.Send(createWICommand);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateProcedureImageCommand() { Id = id, Image = image });
                }

                var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("Create", procedure);
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
                var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);
                    procedureViewModel.IsActive = false;
                    var updateProcedureCommand = _mapper.Map<UpdateProcedureCommand>(procedureViewModel);
                    var result = await _mediator.Send(updateProcedureCommand);
                    if (result.Succeeded)
                        _notify.Information($"Procedure with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllProceduresCachedQuery());
                    if (response2.Succeeded)
                    {
                        var response3 = await _mediator.Send(new Application.Features.SOPs.Queries.GetAllCached.GetAllSOPsCachedQuery());
                        if (response3.Succeeded)
                        {
                            var viewModelSOP = _mapper.Map<List<SOPViewModel>>(response3.Data);
                            viewModelSOP = viewModelSOP.Where(a => a.WSCPNo == procedureViewModel.WSCPNo && a.WSCPId == procedureViewModel.Id).ToList();
                            if (rolesList.Contains("D"))
                            {
                                viewModelSOP = viewModelSOP.Where(a => users.Select(s=> s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                            }
                            foreach (SOPViewModel item in viewModelSOP)
                            {
                                item.IsActive = false;
                                var updateSOPCommand = _mapper.Map<Application.Features.SOPs.Commands.Update.UpdateSOPCommand>(item);
                                var result2 = await _mediator.Send(updateSOPCommand);  
                            }
                        }
                        var response4 = await _mediator.Send(new Application.Features.WIs.Queries.GetAllCached.GetAllWIsCachedQuery());
                        if (response4.Succeeded)
                        {
                            var viewModelWI = _mapper.Map<List<WIViewModel>>(response4.Data);
                            viewModelWI = viewModelWI.Where(a => a.WSCPNo == procedureViewModel.WSCPNo && a.WSCPId == procedureViewModel.Id).ToList();
                            if (rolesList.Contains("D"))
                            {
                                viewModelWI = viewModelWI.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                            }
                            foreach (WIViewModel item2 in viewModelWI)
                            {
                                item2.IsActive = false;
                                var updateWICommand = _mapper.Map<Application.Features.WIs.Commands.Update.UpdateWICommand>(item2);
                                var result3 = await _mediator.Send(updateWICommand);
                            }
                        }

                        var viewModel = _mapper.Map<List<ProcedureViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                        }
                        foreach (ProcedureViewModel item in viewModel)
                        {
                            var psStatat = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.ProcedureStatusView = StatusId.DocumentStatus.Name;
                            }
                            else
                            {
                                item.ProcedureStatusView = "New";
                            }
                        }
                        var responseA = await _mediator.Send(new GetAllProceduresCachedQuery());
                        if (responseA.Succeeded)
                        {
                            var viewModelA = _mapper.Map<List<ProcedureViewModel>>(responseA.Data);
                            var htmlA = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModelA);
                            return new JsonResult(new { isValid = true, html = htmlA });
                        }
                        else
                        {
                            _notify.Error(response.Message);
                            return null;
                        }
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

        //    var deleteCommand = await _mediator.Send(new DeleteProcedureCommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Procedure with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllProceduresCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<ProcedureViewModel>>(response.Data);

        //            if (rolesList.Contains("D"))
        //            {
        //                viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
        //            }

        //            foreach (ProcedureViewModel item in viewModel)
        //            {
        //                var psStatat = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).ToList();

        //                if (psStatat.Count != 0)
        //                {
        //                    var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).OrderBy(a => a.CreatedOn)
        //                        .Include(a => a.DocumentStatus)
        //                        .Last();
        //                    item.ProcedureStatusView = StatusId.DocumentStatus.Name;
        //                }
        //                else
        //                {
        //                    item.ProcedureStatusView = "New";
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
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Assignment", procedureViewModel) });
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
            int allDeptId = 0;
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            if (departmentsResponse.Succeeded)
            {
                var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            }
                var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s=> s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);

                var userViewModel = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                procedureViewModel.UserList = new SelectList(userViewModel, "UserConcurred1Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred1", procedureViewModel) });
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
            int allDeptId = 0;
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            if (departmentsResponse.Succeeded)
            {
                var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            }
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);

                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();

                procedureViewModel.UserList = new SelectList(userViewModel, "UserConcurred2Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred2", procedureViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignApprovedBy(int id)
        {
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);


                var userViewModel = (from a1 in await _userManager.Users.ToListAsync()
                                     select new UserViewModel
                                     {
                                         UserApproveBy = a1.Id,
                                         FullName = a1.LastName + " " + a1.FirstName + " (" + a1.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                procedureViewModel.UserList = new SelectList(userViewModel, "UserApproveBy", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignApprovedBy", procedureViewModel) });
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmitAssignment(int id, ProcedureViewModel procedure)
        {
            if (id == 0)
            {
                var createProcedureCommand = _mapper.Map<CreateProcedureCommand>(procedure);
                var result = await _mediator.Send(createProcedureCommand);
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Procedure with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
            }

            else
            {
                procedure.Concurred1 = procedure.Concurred1;
                var updateProcedureCommand = _mapper.Map<UpdateProcedureCommand>(procedure);
                var result = await _mediator.Send(updateProcedureCommand);
                if (result.Succeeded) _notify.Information($"Procedure Assignment with ID {result.Data} Updated.");
            }
            var response = await _mediator.Send(new GetAllProceduresCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<ProcedureViewModel>>(response.Data);
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

using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.SOPs.Queries.GetAllCached;
using EDocSys.Application.Features.SOPs.Commands.Create;
using EDocSys.Application.Features.SOPs.Commands.Delete;
using EDocSys.Application.Features.SOPs.Commands.Update;
using EDocSys.Application.Features.SOPs.Queries.GetById;
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
using EDocSys.Application.Features.Departments.Queries.GetById;
using EDocSys.Application.Features.Procedures.Commands.Update;
using EDocSys.Application.Features.Procedures.Queries.GetById;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Commands.Update;
using Microsoft.Extensions.Logging;
using static EDocSys.Application.Constants.Permissions;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class SOPController : BaseController<SOPController>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public SOPController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //public IActionResult Index(string wscpno, int wscpId = 0)
        //{
        //    var model = new SOPViewModel();

        //    ViewBag.WSCPNo = wscpno;
        //    ViewBag.WSCPId = wscpId;

        //    return View(model);
        //}

        //Edit Date: 19/11/2024
        public async Task<IActionResult> Index(string wscpno, int wscpId = 0)
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
            List<int> listComp = new List<int>();
            List<int> listDept = new List<int>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
                listComp.Add(user.UserCompanyId);
                listDept.Add(user.UserDepartmentId);
            }

            // Set role flags
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


            var response = await _mediator.Send(new GetAllSOPsCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

                // Filter procedures based on user's company and department access
                var filteredProcedures = viewModel.Where(a =>
                    a.IsActive == true &&
                    (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) &&
                    ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                        ? true
                        : listDept.Contains(a.DepartmentId))
                ).ToList();

                // Apply role-specific filtering
                if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    // QMR / Lead Auditor / SOP Company Admin (Full Access by Company)
                    filteredProcedures = filteredProcedures
                        .Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId))
                        .ToList();
                }
                else if (rolesList.Contains("D"))
                {
                    // SOP Department Admin (Full Access by Department)
                    filteredProcedures = filteredProcedures
                        .Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)
                            && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId))
                        .ToList();
                }
                else if (rolesList.Contains("C"))
                {
                    filteredProcedures = filteredProcedures
                        .Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)
                            && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)
                            && a.SOPStatusView == "Approved")
                        .ToList();
                }

                // Get unique departments for filter
                var departments = filteredProcedures
                    .Select(x => x.ProcessName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                ViewBag.Departments = departments;
            }

            var model = new SOPViewModel();
            ViewBag.WSCPNo = wscpno;
            ViewBag.WSCPId = wscpId;

            return View(model);
        }

        public async Task<IActionResult> Preview(int id, bool print = false, int IPrint = 0, bool revert = false)
        {
            ViewBag.IPrint = false;
            ViewBag.IAmend = false;
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
            if (revert)
            {
                var response2 = await _mediator.Send(new GetSOPByIdQuery() { Id = id });
                var sopViewModelOld = _mapper.Map<SOPViewModel>(response2.Data);
                sopViewModelOld.IsArchive = false;
                sopViewModelOld.ArchiveDate = null;
                var updateSOPCommandOld = _mapper.Map<UpdateSOPCommand>(sopViewModelOld);
                var result2 = await _mediator.Send(updateSOPCommandOld);
            }
            var response = await _mediator.Send(new GetSOPByIdQuery() { Id = id });

            var statusById = _context.SOPStatus.Where(a => a.SOPId == id).ToList();

            if (response.Succeeded)
            {
                var sopViewModel = _mapper.Map<SOPViewModel>(response.Data);
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == sopViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        if (user.UserDepartmentId == sopViewModel.DepartmentId)
                            rolesListDept.AddRange(roles);
                    }
                }
                if (IPrint != 0 && !print)
                {
                    var responseInfo = await _mediator.Send(new GetIssuanceInfoByIdQuery() { Id = IPrint });
                    if (responseInfo.Succeeded)
                    {
                        var issuanceInfoViewModel = _mapper.Map<IssuanceInfoViewModel>(responseInfo.Data);
                        var responseInfoH = await _mediator.Send(new GetIssuanceByIdQuery() { Id = issuanceInfoViewModel.HId });
                        if (responseInfoH.Succeeded)
                        {
                            var issuanceViewModel = _mapper.Map<IssuanceViewModel>(responseInfoH.Data);
                            var psStatat = _context.IssuanceStatus.Where(a => a.IssuanceId == issuanceViewModel.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == issuanceViewModel.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                if (StatusId.DocumentStatus.Name == "Concurred1")
                                {
                                    issuanceViewModel.IssuanceStatusView = "Verified";
                                }
                                else
                                {
                                    issuanceViewModel.IssuanceStatusView = StatusId.DocumentStatus.Name;
                                }
                            }
                            else
                            {
                                issuanceViewModel.IssuanceStatusView = "New";
                            }
                            if (issuanceViewModel.IssuanceStatusView == "Acknowledged")
                            {
                                var responseP = await _mediator.Send(new GetAllIssuancesInfoPrintCachedQuery());
                                var viewModelP = _mapper.Map<List<IssuanceInfoPrintViewModel>>(responseP.Data);
                                var printed = viewModelP.Where(w => w.IsPrinted = true && w.PrintedDate != null && w.IsReturned == false).ToList();
                                issuanceViewModel.PrintCountAct = printed.Count();
                                viewModelP = viewModelP.Where(w => w.IssInfoId == IPrint).ToList();
                                if (viewModelP.Count > 0)
                                {
                                    if(rolesListComp.Contains("E") || rolesListComp.Contains("SuperAdmin"))
                                        ViewBag.IPrint = true;
                                }
                                if (issuanceViewModel.DOCStatus != "New" && sopViewModel.IsArchive == false)
                                {
                                    if (rolesListComp.Contains("D") || rolesListComp.Contains("SuperAdmin"))
                                    {
                                        if (issuanceViewModel.PrintCountAct == 0)
                                            ViewBag.IAmend = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (print)
                {
                    sopViewModel.PrintCount = sopViewModel.PrintCount + 1;
                    var updateSOPCommand = _mapper.Map<UpdateSOPCommand>(sopViewModel);
                    var result = await _mediator.Send(updateSOPCommand);
                    var responseP = await _mediator.Send(new GetAllIssuancesInfoPrintCachedQuery());
                    var viewModelP = _mapper.Map<List<IssuanceInfoPrintViewModel>>(responseP.Data);
                    viewModelP = viewModelP.Where(w => w.IsPrinted && w.PrintedDate == null).ToList();
                    if (viewModelP.Count > 0)
                    {
                        var vmP = viewModelP.FirstOrDefault();
                        vmP.PrintedDate = DateTime.Now;
                        var updateIssuanceInfoPrintCommand = _mapper.Map<UpdateIssuanceInfoPrintCommand>(vmP);
                        var resultP = await _mediator.Send(updateIssuanceInfoPrintCommand);
                    }
                    return RedirectToAction("Preview", new { Id = id });
                }
                var userChkE = users.Select(s => s.UserCompanyId).Contains(sopViewModel.CompanyId);
                var userChk = users.Select(s => s.UserCompanyId).Contains(sopViewModel.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(sopViewModel.DepartmentId);
                var validUser = false;
                if (rolesList.Contains("A"))
                {
                    ViewBag.RoleA = true;
                    ViewBag.RoleAB1 = true;
                    validUser = true;
                }
                if (rolesListComp.Contains("B1") && userChk)
                {
                    ViewBag.RoleB1 = true;
                    ViewBag.RoleAB1 = true;
                    validUser = true;
                }
                if (rolesListComp.Contains("B2") && userChk)
                {
                    ViewBag.RoleB2 = true;
                    validUser = true;
                }
                if (rolesListDept.Contains("C") && userChk)
                {
                    ViewBag.RoleC = true;
                    validUser = true;
                }
                if (rolesListComp.Contains("E") && userChkE)
                {
                    ViewBag.RoleE = true;
                    validUser = true;
                }
                if (rolesListDept.Contains("D") && userChk)
                {
                    ViewBag.RoleD = true;
                    validUser = true;
                }
                if (rolesList.Contains("SuperAdmin"))
                {
                    ViewBag.RoleSA = true;
                    validUser = true;
                }
                if (!validUser)
                {
                    _notify.Error("Access Denied");
                    return View(nameof(Index));
                }
                if (statusById.Count() != 0)
                {
                    var StatusId = _context.SOPStatus.Where(a => a.SOPId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    sopViewModel.SOPStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    sopViewModel.SOPStatusView = "New";
                }

                if (sopViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == sopViewModel.Concurred1).SingleOrDefault();
                    sopViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    sopViewModel.Concurred1 = concurred1User.Id;
                    sopViewModel.PositionC1 = concurred1User.Position;

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
                                    sopViewModel.DateApprovedC1 = c1StatusDt;
                                }
                            }
                            else
                            {
                                sopViewModel.DateApprovedC1 = c1StatusDt;
                            }
                        }
                    }
                }

                if (sopViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == sopViewModel.Concurred2).SingleOrDefault();
                    sopViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                    sopViewModel.Concurred2 = concurred2User.Id;
                    sopViewModel.PositionC2 = concurred2User.Position;

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
                                    sopViewModel.DateApprovedC2 = c1StatusDt;
                                }
                            }
                            else
                            {
                                sopViewModel.DateApprovedC2 = c1StatusDt;
                            }
                        }
                    }
                }

                if (sopViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == sopViewModel.ApprovedBy).SingleOrDefault();
                    sopViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    sopViewModel.PositionApp = ApprovedByUser.Position;

                    if (statusById.Count() != 0)
                    {
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
                                    sopViewModel.DateApprovedAPP = c1StatusDt;
                                }
                            }
                            else
                            {
                                sopViewModel.DateApprovedAPP = c1StatusDt;
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



                return View(sopViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditSOP")]
        public async Task<IActionResult> CreateOrEdit(int id = 0, string wscpno = "", int wscpid = 0, int departmentId = 0, int procedureId = 0, bool SOPrev = false)
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
            int allCompId = 0;

            if (id == 0)
            {
                var selectedDepartmentId = await _mediator.Send(new GetDepartmentByIdQuery() { Id = departmentId });

                var sopViewModel = new SOPViewModel();
                sopViewModel.EstalishedDate = DateTime.Now;
                sopViewModel.PreparedByDate = DateTime.Now;
                if (departmentsResponse.Succeeded)
                {
                    sopViewModel.DepartmentId = selectedDepartmentId.Data.Id;
                    sopViewModel.ProcessName = selectedDepartmentId.Data.Name;

                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
                    sopViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);

                    sopViewModel.ProcedureId = procedureId;
                    sopViewModel.WSCPNo = wscpno;
                    sopViewModel.WSCPId = wscpid;
                }

                //if (companiesResponse.Succeeded)
                //{
                //    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                //    sopViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                //}

                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }

                    sopViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }

                // Concurred 1
                var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC1 = (from a1 in responseC1
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred1Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                sopViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC2 = (from a1 in responseC2
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                sopViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelAPP = (from a1 in responseAPP
                                        join a2 in _userManager.Users on a1.UserId equals a2.Id
                                        select new UserApproverViewModel
                                        {
                                            UserApproveBy = a1.UserId,
                                            FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                        }).OrderBy(a => a.Email).ToList();
                sopViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");


                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "SOP_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;

                return View(sopViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetSOPByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var sopViewModel = _mapper.Map<SOPViewModel>(response.Data);
                    var sopViewModelOld = _mapper.Map<SOPViewModel>(response.Data);
                    if (SOPrev)
                    {
                        sopViewModel.Id = 0;
                        var revNo = sopViewModel.RevisionNo != null ? sopViewModel.RevisionNo : 0;
                        sopViewModel.RevisionNo = revNo + 1;
                        sopViewModel.RevisionDate = DateTime.Now;
                        sopViewModel.EffectiveDate = null;
                        sopViewModel.EstalishedDate = DateTime.Now;
                        sopViewModel.PreparedByDate = DateTime.Now;
                        sopViewModel.ArchiveId = sopViewModelOld.Id;
                        sopViewModel.PrintCount = 0;
                    }
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
                        sopViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        sopViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }

                    // Concurred 1
                    var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC1 = (from a1 in responseC1
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred1Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    sopViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    sopViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelAPP = (from a1 in responseAPP
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserApproveBy = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    sopViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");


                    return View(sopViewModel);
                }
                return null;
            }

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

            var response = await _mediator.Send(new GetAllSOPsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2")) ? true : listDept.Contains(a.DepartmentId))).ToList();

                if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    // Access Category = E  
                    // QMR / Lead Auditor / SOP Company Admin (Full Access by Company)
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }
                else if (rolesList.Contains("D"))
                {
                    // Access Category = D  
                    // SOP Department Admin (Full Access by Department)
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                }
                else if (rolesList.Contains("C"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) && a.SOPStatusView == "Approved").ToList();
                }    

                foreach (SOPViewModel item in viewModel)
                {
                    var psStatat = _context.SOPStatus.Where(a => a.SOPId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.SOPStatus.Where(a => a.SOPId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        item.SOPStatusView = StatusId.DocumentStatus.Name;
                    }
                    else
                    {
                        item.SOPStatusView = "New";
                    }
                   
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadByWSCP(string wscpno, int wscpid = 0)
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
            var response = await _mediator.Send(new GetAllSOPsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

                var viewModelbyWSCPNo = viewModel.Where(a => a.WSCPNo == wscpno && a.WSCPId == wscpid && a.IsActive == true).ToList();

                return PartialView("_ViewAll", viewModelbyWSCPNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
        {
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            if (id == 0)
            {
                var sopViewModel = new SOPViewModel();
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    sopViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", sopViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetSOPByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var sopViewModel = _mapper.Map<SOPViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        sopViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", sopViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, SOPViewModel sop)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createSOPCommand = _mapper.Map<CreateSOPCommand>(sop);
                    var result = await _mediator.Send(createSOPCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"SOP with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateSOPCommand = _mapper.Map<UpdateSOPCommand>(sop);
                    var result = await _mediator.Send(updateSOPCommand);
                    if (result.Succeeded) _notify.Information($"SOP with ID {result.Data} Updated.");
                }
                if (sop.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetSOPByIdQuery() { Id = sop.ArchiveId });
                    if (response2.Succeeded)
                    {
                        var SOPViewModelOld = _mapper.Map<SOPViewModel>(response2.Data);
                        SOPViewModelOld.IsArchive = true;
                        var archiveDt = DateTime.Now.AddDays(30);
                        SOPViewModelOld.ArchiveDate = archiveDt;
                        var updateSOPCommandOld = _mapper.Map<UpdateSOPCommand>(SOPViewModelOld);
                        var result2 = await _mediator.Send(updateSOPCommandOld);
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateSOPImageCommand() { Id = id, Image = image });
                }

                var response = await _mediator.Send(new GetSOPByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var sopViewModel = _mapper.Map<SOPViewModel>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("Create", sop);
                return View(nameof(Index));
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
                var response = await _mediator.Send(new GetSOPByIdQuery() { Id = id });
                if (response.Succeeded)
                {    
                    var sopViewModel = _mapper.Map<SOPViewModel>(response.Data);
                    sopViewModel.IsActive = false;
                    var updateSOPCommand = _mapper.Map<UpdateSOPCommand>(sopViewModel);
                    var result = await _mediator.Send(updateSOPCommand);
                    if (result.Succeeded)
                        _notify.Information($"SOP with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllSOPsCachedQuery());
                    if (response2.Succeeded)
                    {
                        var response3 = await _mediator.Send(new Application.Features.WIs.Queries.GetAllCached.GetAllWIsCachedQuery());
                        if (response3.Succeeded)
                        {
                            var viewModelWI = _mapper.Map<List<WIViewModel>>(response3.Data);
                            viewModelWI = viewModelWI.Where(a => a.SOPNo == sopViewModel.SOPNo && a.SOPId == sopViewModel.Id && a.IsActive == true).ToList();
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
                        var viewModel = _mapper.Map<List<SOPViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                        }
                        foreach (SOPViewModel item in viewModel)
                        {
                            var psStatat = _context.SOPStatus.Where(a => a.SOPId == item.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.SOPStatus.Where(a => a.SOPId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.SOPStatusView = StatusId.DocumentStatus.Name;
                            }
                            else
                            {
                                item.SOPStatusView = "New";
                            }
                        }

                        //var html = "";
                        try
                        {
                            return new JsonResult(new { isValid = true });
                            //return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel) });
                            //html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("Error:", ex);
                        }

                        //return new JsonResult(new { isValid = true, html = html });
                        return new JsonResult(new { isValid = true });
                    }
                }
            }
            return null;
        }
        //[HttpPost]
        //public async Task<JsonResult> OnPostDelete(int id)
        //{
        //    var deleteCommand = await _mediator.Send(new DeleteSOPCommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"SOP with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllSOPsCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);
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

            //foreach(IFormFile photo in Request.Form.Files)
            //{
            //    string serverMapPath = Path.Combine(_env.WebRootPath, "Image", photo.FileName);
            //    using (var stream = new FileStream(serverMapPath, FileMode.Create))
            //    {
            //        photo.CopyTo(stream);
            //    }
            //    // filepath = "https://localhost:44354/" + "Image/" + photo.FileName;
            //}

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
                    Description = "Blah"

                };
                using (var dataStream = new MemoryStream())
                {
                    file.CopyTo(dataStream);
                    fileModel.Data = dataStream.ToArray();
                }



                // fileModel.Data = file.OptimizeImageSize(720, 720);
                // fileModel.Data = file.OptimizeImageSize(1080, 1080);

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

    }
}

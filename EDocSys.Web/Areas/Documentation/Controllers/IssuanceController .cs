using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Commands.Create;
using EDocSys.Application.Features.Issuances.Commands.Delete;
using EDocSys.Application.Features.Issuances.Commands.Update;
using EDocSys.Application.Features.Issuances.Queries.GetById;
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
using EDocSys.Application.Features.IssuanceStatuses.Queries.GetAllCached;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using EDocSys.Application.Features.Procedures.Queries.GetAllCached;
using EDocSys.Application.Features.SOPs.Queries.GetAllCached;
using EDocSys.Application.Features.WIs.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Queries.GetById;
using EDocSys.Application.Features.SOPs.Queries.GetById;
using EDocSys.Application.Features.WIs.Queries.GetById;
using EDocSys.Application.Features.Procedures.Commands.Update;
using EDocSys.Application.Features.SOPs.Commands.Update;
using EDocSys.Application.Features.WIs.Commands.Update;
using EDocSys.Application.Features.Procedures.Queries.GetByParameter;
using EDocSys.Application.Features.SOPs.Queries.GetByParameter;
using EDocSys.Application.Features.WIs.Queries.GetByParameter;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class IssuanceController : BaseController<IssuanceController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public IssuanceController(ApplicationDbContext context,
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
        public async Task<IActionResult> IndexAsync(string docType)
        {
            if (docType == "new")
                return await IndexNewAsync();
            else
                return await IndexEditAsync();
        }


        public async Task<IActionResult> IndexNewAsync()
        {
            ViewBag.docStatus = "New";
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
            var model = new IssuanceViewModel();
            return View(model);
        }

        public async Task<IActionResult> IndexEditAsync()
        {
            ViewBag.docStatus = "Amend";
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
            var model = new IssuanceViewModel();
            return View(model);
        }

        public async Task<IActionResult> IndexPrintAsync()
        {
            ViewBag.docStatus = "Print";
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
            var model = new IssuanceInfoPrintViewModel();
            return View(model);
        }

        public IActionResult Preview1()
        {
            var model = new IssuanceViewModel();
            return View(model);
        }

        [Authorize(Policy = "CanViewIssuance")]
        public async Task<IActionResult> Preview(int id, bool print = false, IssuanceViewModel issuance = null, bool revert = false)
        {
            ViewBag.Amend = false;
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
                var response2 = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
                var issuanceViewModelOld = _mapper.Map<IssuanceViewModel>(response2.Data);
                issuanceViewModelOld.IsArchive = false;
                issuanceViewModelOld.ArchiveDate = null;
                var updateIssuanceCommandOld = _mapper.Map<UpdateIssuanceCommand>(issuanceViewModelOld);
                var result2 = await _mediator.Send(updateIssuanceCommandOld);
            }
            var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
            var adg = _context.IssuanceStatus;
            var statusById = _context.IssuanceStatus.Where(a => a.IssuanceId == id).ToList();

            if (response.Succeeded)
            {
                var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);

                if (print || issuance.PrintCount != 0)
                {
                    if (issuance.PrintCount != issuanceViewModel.PrintCount)
                        issuanceViewModel.PrintCount = issuance.PrintCount;
                    else
                        issuanceViewModel.PrintCount = issuanceViewModel.PrintCount + 1;
                    var updateIssuanceCommand = _mapper.Map<UpdateIssuanceCommand>(issuanceViewModel);
                    var result = await _mediator.Send(updateIssuanceCommand);
                    return RedirectToAction("Preview", new { Id = id });
                }
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == issuanceViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        //var QADeptId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id; //KIV
                        //if (user.UserDepartmentId == QADeptId)
                        //    rolesListDept.AddRange(roles);
                        if (user.UserDepartmentId == issuanceViewModel.DepartmentId)
                            rolesListDept.AddRange(roles);
                    }
                }

                var userChkE = users.Select(s => s.UserCompanyId).Contains(issuanceViewModel.CompanyId);
                var userChk = users.Select(s => s.UserCompanyId).Contains(issuanceViewModel.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(issuanceViewModel.DepartmentId);
                var userChkD = users.Select(s => s.UserCompanyId).Contains(issuanceViewModel.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(issuanceViewModel.DepartmentId) && (rolesList.Contains("D") || rolesList.Contains("SuperAdmin"));
                if (userChkD && issuanceViewModel.DOCStatus != "New")
                    ViewBag.Amend = true;
                var validUser = false;
                if (rolesList.Contains("A"))
                {
                    ViewBag.RoleA = true;
                    ViewBag.RoleAB1 = true;
                    validUser = true;
                }
                if (rolesListComp.Contains("B1") && userChkE)
                {
                    ViewBag.RoleB1 = true;
                    ViewBag.RoleAB1 = true;
                    validUser = true;
                }
                if (rolesListComp.Contains("B2") && userChkE)
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
                issuanceViewModel.TitleName = issuanceViewModel.DOCStatus == "New" ? "New Issue Requisition" : "Amendment Requisition";
                if (statusById.Count() != 0)
                {
                    var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == id).OrderBy(a => a.CreatedOn)
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
                if (issuanceViewModel.VerifiedBy != null)
                {
                    var VerifiedByUser = _userManager.Users.Where(a => a.Id == issuanceViewModel.VerifiedBy).SingleOrDefault();
                    issuanceViewModel.VerifiedName = VerifiedByUser.FirstName + " " + VerifiedByUser.LastName;
                    issuanceViewModel.VerifiedBy = VerifiedByUser.Id;
                    issuanceViewModel.PositionVer = VerifiedByUser.Position;

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
                                    issuanceViewModel.DateVerified = c1StatusDt;
                                }
                            }
                            else
                            {
                                issuanceViewModel.DateVerified = c1StatusDt;
                            }
                        }
                    }
                }

                if (issuanceViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == issuanceViewModel.ApprovedBy).SingleOrDefault();
                    issuanceViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    issuanceViewModel.PositionApp = ApprovedByUser.Position;

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
                                    issuanceViewModel.DateApproved = c1StatusDt;
                                }
                            }
                            else
                            {
                                issuanceViewModel.DateApproved = c1StatusDt;
                            }
                        }
                    }
                }

                if (issuanceViewModel.AcknowledgedBy != null)
                {
                    var AcknowledgeByUser = _userManager.Users.Where(a => a.Id == issuanceViewModel.AcknowledgedBy).SingleOrDefault();
                    issuanceViewModel.AcknowledgedBy = AcknowledgeByUser.FirstName + " " + AcknowledgeByUser.LastName;
                    issuanceViewModel.PositionAck = AcknowledgeByUser.Position;

                    if (statusById.Count() != 0)
                    {
                        // Check if WI status records exist first
                        var wiStatusRecords = _context.WIStatus
                            .Where(a => a.WIId == id)
                            .OrderBy(a => a.CreatedOn);

                        if (wiStatusRecords.Any())
                        {
                            var StatusId = wiStatusRecords.Select(a => a.DocumentStatusId).Last();

                            var c1Status = statusById.Where(a => a.DocumentStatusId == 7).OrderBy(a => a.CreatedOn).ToList();
                            if (c1Status.Count > 0)
                            {
                                var c1StatusDt = c1Status.Select(a => a.CreatedOn).Last();
                                var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                                if (rejectStatus.Count > 0)
                                {
                                    var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                                    if (c1StatusDt > rejectStatusDt)
                                    {
                                        issuanceViewModel.DateAcknowledged = c1StatusDt;
                                    }
                                }
                                else
                                {
                                    issuanceViewModel.DateAcknowledged = c1StatusDt;
                                }
                            }
                        }

                        //var StatusId = _context.WIStatus.Where(a => a.WIId == id).OrderBy(a => a.CreatedOn).Select(a => a.DocumentStatusId).Last();

                        //var c1Status = statusById.Where(a => a.DocumentStatusId == 7).OrderBy(a => a.CreatedOn).ToList();
                        //if (c1Status.Count > 0)
                        //{
                        //    var c1StatusDt = c1Status.Select(a => a.CreatedOn).Last();
                        //    var rejectStatus = statusById.Where(a => a.DocumentStatusId == 5).OrderBy(a => a.CreatedOn).ToList();
                        //    if (rejectStatus.Count > 0)
                        //    {
                        //        var rejectStatusDt = rejectStatus.Select(a => a.CreatedOn).Last();

                        //        if (c1StatusDt > rejectStatusDt)
                        //        {
                        //            issuanceViewModel.DateAcknowledged = c1StatusDt;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        issuanceViewModel.DateAcknowledged = c1StatusDt;
                        //    }
                        //}
                    }
                }

                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                //var currentyUserId = currentUser.Id;
                var verified = response.Data.VerifiedBy;
                var app = response.Data.ApprovedBy;
                var ack = response.Data.AcknowledgedBy;

                if (users.Select(s => s.Id).Contains(verified))
                {
                    ViewBag.IsVerified = true;
                }

                if (users.Select(s => s.Id).Contains(app))
                {
                    ViewBag.IsApp = true;
                }

                if (users.Select(s => s.Id).Contains(ack))
                {
                    ViewBag.IsAck = true;
                }

                var responseInfo = await _mediator.Send(new GetIssuanceInfoByHIdQuery() { HId = id });
                if (responseInfo.Succeeded)
                {
                    var issuanceInfoViewModel = _mapper.Map<List<IssuanceInfoViewModel>>(responseInfo.Data);
                    foreach (var issinfo in issuanceInfoViewModel)
                    {
                        if (issinfo.DocType == "WSCP")
                        {
                            var responseWSCP = await _mediator.Send(new GetProcedureByIdQuery() { Id = Convert.ToInt32(issinfo.DOCId) });
                            if (responseWSCP.Succeeded)
                            {
                                var procedureViewModel = _mapper.Map<ProcedureViewModel>(responseWSCP.Data);
                                issinfo.DOCNo = procedureViewModel.WSCPNo;
                                issinfo.DOCUrl = "/documentation/procedure/Preview?id=" + issinfo.DOCId;
                            }
                        }
                        else if (issinfo.DocType == "SOP")
                        {
                            var responseSOP = await _mediator.Send(new GetSOPByIdQuery() { Id = Convert.ToInt32(issinfo.DOCId) });
                            if (responseSOP.Succeeded)
                            {
                                var sopViewModel = _mapper.Map<SOPViewModel>(responseSOP.Data);
                                issinfo.DOCNo = sopViewModel.SOPNo + " <" + sopViewModel.WSCPNo + ">";
                                issinfo.DOCUrl = "/documentation/sop/Preview?id=" + issinfo.DOCId;
                            }
                        }
                        else if (issinfo.DocType == "WI")
                        {
                            var responseWI = await _mediator.Send(new GetWIByIdQuery() { Id = Convert.ToInt32(issinfo.DOCId) });
                            if (responseWI.Succeeded)
                            {
                                var wiViewModel = _mapper.Map<WIViewModel>(responseWI.Data);
                                issinfo.DOCNo = wiViewModel.WINo + " <" + wiViewModel.WSCPNo + "|" + wiViewModel.SOPNo + ">";
                                issinfo.DOCUrl = "/documentation/wi/Preview?id=" + issinfo.DOCId;
                            }
                        }
                        else
                            issinfo.DOCNo = "";

                    }
                    issuanceViewModel.IssuanceInfo = issuanceInfoViewModel;
                }
                return View(issuanceViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditIssuance")]
        public async Task<IActionResult> CreateOrEdit(int id = 0, bool refresh = false, bool addInfo = false, IssuanceViewModel ivm = null, string docStatus = "New")
        {
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

            var companiesResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
            var qaId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            int allCompId = 0;
            var items = new List<string>() { "WSCP", "SOP", "WI" };
            if (id == 0)
            {
                var issuanceViewModel = new IssuanceViewModel();
                issuanceViewModel.DOCStatus = docStatus;
                if (refresh)
                {
                    issuanceViewModel = ivm;
                }
                if (addInfo)
                {
                    issuanceViewModel = ivm;
                    var newInfo = new IssuanceInfoViewModel();
                    newInfo.DocTypes = new SelectList(items);
                    if (issuanceViewModel.IssuanceInfo == null)
                    {
                        issuanceViewModel.IssuanceInfo = new List<IssuanceInfoViewModel>();
                        newInfo.No = 1;
                    }
                    else
                        newInfo.No = issuanceViewModel.IssuanceInfo.Count + 1;
                    issuanceViewModel.IssuanceInfo.Add(newInfo);
                    issuanceViewModel.IssuanceInfo.All(c => { c.DocTypes = new SelectList(items); return true; });
                }
                else
                {
                    if (!refresh)
                    {
                        var response = await _mediator.Send(new GetAllIssuancesCachedQuery());
                        if (response.Succeeded)
                        {
                            var viewModel = _mapper.Map<List<IssuanceViewModel>>(response.Data);
                            viewModel = viewModel.Where(a => a.IsActive == true && a.DOCStatus == "New" && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2")) ? true : listDept.Contains(a.DepartmentId))).ToList();
                            issuanceViewModel.DOCNo = (viewModel.Count() + 1).ToString().PadLeft(6, '0');
                        }
                        else
                        {
                            issuanceViewModel.DOCNo = (1).ToString().PadLeft(6, '0');
                        }
                        issuanceViewModel.IssuanceInfo = new List<IssuanceInfoViewModel>();
                        var newInfo = new IssuanceInfoViewModel();
                        newInfo.No = 1;
                        newInfo.DocTypes = new SelectList(items);
                        issuanceViewModel.IssuanceInfo.Add(newInfo);
                    }
                    issuanceViewModel.IssuanceInfo.All(c => { c.DocTypes = new SelectList(items); return true; });
                }
                issuanceViewModel.RequestedBy = currentUser.FirstName + " " + currentUser.LastName;
                issuanceViewModel.RequestedByDate = DateTime.Now;
                issuanceViewModel.RequestedByPosition = currentUser.Position;
                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }

                    issuanceViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }
                if (departmentsResponse.Succeeded)
                {
                    if (rolesList.Contains("E"))
                    {
                        departmentViewModel = departmentViewModel.Where(d => d.Id != allDeptId).ToList();
                    }
                    else if (rolesList.Contains("D"))
                    {
                        if (users.Select(s => s.UserDepartmentId).Contains(allDeptId))
                        {
                            // If user's department is "All Departments", remove it from the selection
                            departmentViewModel = departmentViewModel.Where(d => d.Id != allDeptId).ToList();
                        }
                        else
                        {
                            departmentViewModel = departmentViewModel.Where(a => users.Select(s => s.UserDepartmentId).Contains(a.Id)).ToList();
                        }
                    }

                    // Add default "Select Process" option
                    var departments = departmentViewModel.ToList();

                    // Create the SelectList with the correct selected value
                    issuanceViewModel.Departments = new SelectList(
                        departments,
                        nameof(DepartmentViewModel.Id),
                        nameof(DepartmentViewModel.Name),
                        issuanceViewModel.DepartmentId == 0 ? null : issuanceViewModel.DepartmentId  // Set selected value
                    );

                    // Ensure the model's DepartmentId matches what we want to display
                    if (issuanceViewModel.DepartmentId == 0)
                    {
                        issuanceViewModel.DepartmentId = 0;  // Explicitly set to 0 for "Select Process"
                    }


                    issuanceViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }

                // First check if user has Role E (Company Admin)
                var hasRoleE = rolesList.Contains("E");

                var companyId = issuanceViewModel.CompanyId == 0 ? currentUser.UserCompanyId : issuanceViewModel.CompanyId;
                var departmentId = issuanceViewModel.DepartmentId == 0 ? currentUser.UserDepartmentId : issuanceViewModel.DepartmentId;

                var responseWSCP = await _mediator.Send(new GetProceduresByParameterQuery() { CompId = companyId, DeptId = departmentId });
                var viewModelWSCP = responseWSCP != null ? _mapper.Map<List<ProcedureViewModel>>(responseWSCP.Data) : null;
                if (viewModelWSCP != null)
                {
                    // Similar modifications should be applied to WSCP and WI filtering:
                    viewModelWSCP = viewModelWSCP.Where(a =>
                        a.IsActive == true &&
                        (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) &&
                        (
                            currentUser.UserDepartmentId == 0 ? true : // SuperAdmin check
                            hasRoleE ? true : // Company Admin can see all departments
                            currentUser.UserDepartmentId == a.DepartmentId // Department check for other roles
                        )
                    ).ToList();

                    //viewModelWSCP = viewModelWSCP.Where(a => a.IsActive == true && (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && (currentUser.UserDepartmentId == 0 ? true : currentUser.UserDepartmentId == a.DepartmentId)).ToList();


                    foreach (var item in viewModelWSCP)
                    {
                        var psStatat = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).ToList();
                        if (psStatat.Count != 0)
                        {
                            var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).OrderBy(a => a.CreatedOn)
                                .Include(a => a.DocumentStatus)
                                .Last();
                            item.ProcedureStatusView = StatusId.DocumentStatus.Name;
                        }
                    }
                    viewModelWSCP = viewModelWSCP.Where(w => w.ProcedureStatusView == "Approved").ToList();
                    var docNoList = viewModelWSCP.Select(s => s.WSCPNo).Distinct().OrderBy(o => o).ToList();
                    var pvmList = new List<ProcedureViewModel>();
                    foreach (var docNo in docNoList)
                    {
                        var vmList = viewModelWSCP.Where(w => w.WSCPNo == docNo).ToList();
                        pvmList.Add(vmList.LastOrDefault());
                    }
                    viewModelWSCP = pvmList;

                }
                var responseSOP = await _mediator.Send(new GetSOPsByParameterQuery() { CompId = companyId, DeptId = departmentId });
                var viewModelSOP = responseSOP != null ? _mapper.Map<List<SOPViewModel>>(responseSOP.Data) : null;
                if (viewModelSOP != null)
                {
                    // Modified filter logic for SOP documents
                    viewModelSOP = viewModelSOP.Where(a =>
                        a.IsActive == true &&
                        (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && // Company check
                        (
                            currentUser.UserDepartmentId == 0 ? true : // SuperAdmin check
                            hasRoleE ? true : // Company Admin can see all departments
                            currentUser.UserDepartmentId == a.DepartmentId // Department check for other roles
                        )
                    ).ToList();

                    //viewModelSOP = viewModelSOP.Where(a => a.IsActive == true && (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && (currentUser.UserDepartmentId == 0 ? true : currentUser.UserDepartmentId == a.DepartmentId)).ToList();

                    foreach (var item in viewModelSOP)
                    {
                        var psStatat = _context.SOPStatus.Where(a => a.SOPId == item.Id).ToList();
                        if (psStatat.Count != 0)
                        {
                            var StatusId = _context.SOPStatus.Where(a => a.SOPId == item.Id).OrderBy(a => a.CreatedOn)
                                .Include(a => a.DocumentStatus)
                                .Last();
                            item.SOPStatusView = StatusId.DocumentStatus.Name;
                        }
                    }
                    viewModelSOP = viewModelSOP.Where(w => w.SOPStatusView == "Approved").ToList();
                    var docNoList = viewModelSOP.Select(s => s.WSCPNo).Distinct().OrderBy(o => o).ToList();
                    var svmList = new List<SOPViewModel>();
                    foreach (var docNo in docNoList)
                    {
                        var pvmList = viewModelSOP.Where(w => w.WSCPNo == docNo).ToList();
                        var sdocNoList = pvmList.Select(s => s.SOPNo).Distinct().OrderBy(o => o).ToList();
                        foreach (var sdocNo in sdocNoList)
                        {
                            var svmList2 = viewModelSOP.Where(w => w.WSCPNo == docNo && w.SOPNo == sdocNo).ToList();
                            svmList.Add(svmList2.LastOrDefault());
                        }
                    }
                    viewModelSOP = svmList;
                }
                var responseWI = await _mediator.Send(new GetWIsByParameterQuery() { CompId = companyId, DeptId = departmentId });
                var viewModelWI = responseWI != null ? _mapper.Map<List<WIViewModel>>(responseWI.Data) : null;
                if (viewModelWI != null)
                {
                    viewModelWI = viewModelWI.Where(a =>
                        a.IsActive == true &&
                        (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) &&
                        (
                            currentUser.UserDepartmentId == 0 ? true : // SuperAdmin check
                            hasRoleE ? true : // Company Admin can see all departments
                            currentUser.UserDepartmentId == a.DepartmentId // Department check for other roles
                        )
                    ).ToList();

                    //viewModelWI = viewModelWI.Where(a => a.IsActive == true && (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && (currentUser.UserDepartmentId == 0 ? true : currentUser.UserDepartmentId == a.DepartmentId)).ToList();

                    foreach (var item in viewModelWI)
                    {
                        var psStatat = _context.WIStatus.Where(a => a.WIId == item.Id).ToList();
                        if (psStatat.Count != 0)
                        {
                            var StatusId = _context.WIStatus.Where(a => a.WIId == item.Id).OrderBy(a => a.CreatedOn)
                                .Include(a => a.DocumentStatus)
                                .Last();
                            item.WIStatusView = StatusId.DocumentStatus.Name;
                        }
                    }
                    viewModelWI = viewModelWI.Where(w => w.WIStatusView == "Approved").ToList();
                    var docNoList = viewModelWI.Select(s => s.WSCPNo).Distinct().OrderBy(o => o).ToList();
                    var wvmList = new List<WIViewModel>();
                    foreach (var docNo in docNoList)
                    {
                        var pvmList = viewModelWI.Where(w => w.WSCPNo == docNo).ToList();
                        var sdocNoList = pvmList.Select(s => s.SOPNo).Distinct().OrderBy(o => o).ToList();
                        foreach (var sdocNo in sdocNoList)
                        {
                            var svmList = viewModelWI.Where(w => w.WSCPNo == docNo && w.SOPNo == sdocNo).ToList();
                            var wdocNoList = svmList.Select(s => s.WINo).Distinct().OrderBy(o => o).ToList();
                            foreach (var wdocNo in wdocNoList)
                            {
                                var wdocNoList2 = viewModelWI.Where(w => w.WSCPNo == docNo && w.SOPNo == sdocNo && w.WINo == wdocNo).ToList();
                                wvmList.Add(wdocNoList2.LastOrDefault());
                            }
                        }
                    }
                    viewModelWI = wvmList;
                }
                foreach (var Issinfo in issuanceViewModel.IssuanceInfo)
                {
                    if (Issinfo.DocType == "WSCP" || Issinfo.DocType == null)
                    {
                        if (responseWSCP.Succeeded)
                        {
                            List<SelectListItem> lstDocNo = new List<SelectListItem>();
                            foreach (var vm in viewModelWSCP)
                            {
                                SelectListItem sli = new SelectListItem();
                                sli.Value = vm.Id.ToString();
                                sli.Text = vm.WSCPNo;
                                lstDocNo.Add(sli);
                            }
                            Issinfo.DOCNos = new SelectList(lstDocNo, "Value", "Text");
                        }
                    }
                    else if (Issinfo.DocType == "SOP")
                    {
                        if (responseSOP.Succeeded)
                        {
                            List<SelectListItem> lstDocNo = new List<SelectListItem>();
                            foreach (var vm in viewModelSOP)
                            {
                                SelectListItem sli = new SelectListItem();
                                sli.Value = vm.Id.ToString();
                                sli.Text = vm.SOPNo + " <" + vm.WSCPNo + ">";
                                lstDocNo.Add(sli);
                            }
                            Issinfo.DOCNos = new SelectList(lstDocNo, "Value", "Text");
                        }
                    }
                    else if (Issinfo.DocType == "WI")
                    {
                        if (responseWI.Succeeded)
                        {
                            List<SelectListItem> lstDocNo = new List<SelectListItem>();
                            foreach (var vm in viewModelWI)
                            {
                                SelectListItem sli = new SelectListItem();
                                sli.Value = vm.Id.ToString();
                                sli.Text = vm.WINo + " <" + vm.WSCPNo + "|" + vm.SOPNo + ">";
                                lstDocNo.Add(sli);
                            }
                            Issinfo.DOCNos = new SelectList(lstDocNo, "Value", "Text");
                        }
                    }
                }
                // Verify By
                var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC1 = (from a1 in responseC1
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred1Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                issuanceViewModel.UserListVer = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Approved By
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelAPP = (from a1 in responseAPP
                                        join a2 in _userManager.Users on a1.UserId equals a2.Id
                                        select new UserApproverViewModel
                                        {
                                            UserApproveBy = a1.UserId,
                                            FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                        }).OrderBy(a => a.Email).ToList();
                issuanceViewModel.UserListApp = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                // Concurred APP
                //var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                //var userViewModelAPP = (from a1 in responseAPP
                //                        join a2 in _userManager.Users on a1.UserId equals a2.Id
                //                        select new UserApproverViewModel
                //                        {
                //                            UserApproveBy = a1.UserId,
                //                            FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                //                        }).OrderBy(a => a.Email).ToList();
                //issuanceViewModel.UserListAck = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "DOC_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;
                return View(issuanceViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);
                    if (refresh)
                    {
                        issuanceViewModel = ivm;
                    }
                    if (addInfo)
                    {
                        issuanceViewModel = ivm;
                        var newInfo = new IssuanceInfoViewModel();
                        newInfo.DocTypes = new SelectList(items);
                        if (issuanceViewModel.IssuanceInfo == null)
                        {
                            issuanceViewModel.IssuanceInfo = new List<IssuanceInfoViewModel>();
                            newInfo.No = 1;
                        }
                        else
                            newInfo.No = issuanceViewModel.IssuanceInfo.Count + 1;
                        issuanceViewModel.IssuanceInfo.Add(newInfo);
                        issuanceViewModel.IssuanceInfo.All(c => { c.DocTypes = new SelectList(items); return true; });
                    }
                    else
                    {
                        if (!refresh)
                        {
                            if (issuanceViewModel.IssuanceInfo == null)
                            {
                                issuanceViewModel.IssuanceInfo = new List<IssuanceInfoViewModel>();
                                var newInfo = new IssuanceInfoViewModel();
                                newInfo.No = 1;
                                newInfo.DocTypes = new SelectList(items);
                                issuanceViewModel.IssuanceInfo.Add(newInfo);
                            }
                            var responseInfo = await _mediator.Send(new GetIssuanceInfoByHIdQuery() { HId = id });
                            if (responseInfo.Succeeded)
                            {
                                var issuanceInfoViewModel = _mapper.Map<List<IssuanceInfoViewModel>>(responseInfo.Data);
                                issuanceViewModel.IssuanceInfo = issuanceInfoViewModel;
                            }
                        }
                        issuanceViewModel.IssuanceInfo.All(c => { c.DocTypes = new SelectList(items); return true; });
                    }
                    //var issuanceViewModelOld = _mapper.Map<IssuanceViewModel>(response.Data);

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        if (rolesList.Contains("D"))
                        {
                            companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                        }

                        issuanceViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }
                    if (departmentsResponse.Succeeded)
                    {
                        if (rolesList.Contains("D"))
                        {
                            departmentViewModel = departmentViewModel.Where(a => users.Select(s => s.UserDepartmentId).Contains(a.Id)).ToList();
                        }

                        // Add default "Select Process" option
                        var departments = departmentViewModel.ToList();

                        // Create the SelectList with the correct selected value
                        issuanceViewModel.Departments = new SelectList(
                            departments,
                            nameof(DepartmentViewModel.Id),
                            nameof(DepartmentViewModel.Name),
                            issuanceViewModel.DepartmentId == 0 ? null : issuanceViewModel.DepartmentId  // Set selected value
                        );

                        // Ensure the model's DepartmentId matches what we want to display
                        if (issuanceViewModel.DepartmentId == 0)
                        {
                            issuanceViewModel.DepartmentId = 0;  // Explicitly set to 0 for "Select Process"
                        }

                        issuanceViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    // First check if user has Role E (Company Admin)
                    var hasRoleE = rolesList.Contains("E");

                    var companyId = issuanceViewModel.CompanyId == 0 ? currentUser.UserCompanyId : issuanceViewModel.CompanyId;
                    var departmentId = issuanceViewModel.DepartmentId == 0 ? currentUser.UserDepartmentId : issuanceViewModel.DepartmentId;

                    var responseWSCP = await _mediator.Send(new GetProceduresByParameterQuery() { CompId = companyId, DeptId = departmentId });
                    var viewModelWSCP = responseWSCP != null ? _mapper.Map<List<ProcedureViewModel>>(responseWSCP.Data) : null;
                    if (viewModelWSCP != null)
                    {
                        // Similar modifications should be applied to WSCP and WI filtering:
                        viewModelWSCP = viewModelWSCP.Where(a =>
                            a.IsActive == true &&
                            (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) &&
                            (
                                currentUser.UserDepartmentId == 0 ? true : // SuperAdmin check
                                hasRoleE ? true : // Company Admin can see all departments
                                currentUser.UserDepartmentId == a.DepartmentId // Department check for other roles
                            )
                        ).ToList();

                        //viewModelWSCP = viewModelWSCP.Where(a => a.IsActive == true && (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && (currentUser.UserDepartmentId == 0 ? true : currentUser.UserDepartmentId == a.DepartmentId)).ToList();


                        foreach (var item in viewModelWSCP)
                        {
                            var psStatat = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).ToList();
                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.ProcedureStatus.Where(a => a.ProcedureId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.ProcedureStatusView = StatusId.DocumentStatus.Name;
                            }
                        }
                        viewModelWSCP = viewModelWSCP.Where(w => w.ProcedureStatusView == "Approved").ToList();
                        var docNoList = viewModelWSCP.Select(s => s.WSCPNo).Distinct().OrderBy(o => o).ToList();
                        var pvmList = new List<ProcedureViewModel>();
                        foreach (var docNo in docNoList)
                        {
                            var vmList = viewModelWSCP.Where(w => w.WSCPNo == docNo).ToList();
                            pvmList.Add(vmList.LastOrDefault());
                        }
                        viewModelWSCP = pvmList;

                    }
                    var responseSOP = await _mediator.Send(new GetSOPsByParameterQuery() { CompId = companyId, DeptId = departmentId });
                    var viewModelSOP = responseSOP != null ? _mapper.Map<List<SOPViewModel>>(responseSOP.Data) : null;
                    if (viewModelSOP != null)
                    {
                        // Modified filter logic for SOP documents
                        viewModelSOP = viewModelSOP.Where(a =>
                            a.IsActive == true &&
                            (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && // Company check
                            (
                                currentUser.UserDepartmentId == 0 ? true : // SuperAdmin check
                                hasRoleE ? true : // Company Admin can see all departments
                                currentUser.UserDepartmentId == a.DepartmentId // Department check for other roles
                            )
                        ).ToList();


                        //viewModelSOP = viewModelSOP.Where(a => a.IsActive == true && (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && (currentUser.UserDepartmentId == 0 ? true : currentUser.UserDepartmentId == a.DepartmentId)).ToList();


                        foreach (var item in viewModelSOP)
                        {
                            var psStatat = _context.SOPStatus.Where(a => a.SOPId == item.Id).ToList();
                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.SOPStatus.Where(a => a.SOPId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.SOPStatusView = StatusId.DocumentStatus.Name;
                            }
                        }
                        viewModelSOP = viewModelSOP.Where(w => w.SOPStatusView == "Approved").ToList();
                        var docNoList = viewModelSOP.Select(s => s.WSCPNo).Distinct().OrderBy(o => o).ToList();
                        var svmList = new List<SOPViewModel>();
                        foreach (var docNo in docNoList)
                        {
                            var pvmList = viewModelSOP.Where(w => w.WSCPNo == docNo).ToList();
                            var sdocNoList = pvmList.Select(s => s.SOPNo).Distinct().OrderBy(o => o).ToList();
                            foreach (var sdocNo in sdocNoList)
                            {
                                var svmList2 = viewModelSOP.Where(w => w.WSCPNo == docNo && w.SOPNo == sdocNo).ToList();
                                svmList.Add(svmList2.LastOrDefault());
                            }
                        }
                        viewModelSOP = svmList;
                    }
                    var responseWI = await _mediator.Send(new GetWIsByParameterQuery() { CompId = companyId, DeptId = departmentId });
                    var viewModelWI = responseWI != null ? _mapper.Map<List<WIViewModel>>(responseWI.Data) : null;
                    if (viewModelWI != null)
                    {
                        viewModelWI = viewModelWI.Where(a =>
                            a.IsActive == true &&
                            (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) &&
                            (
                                currentUser.UserDepartmentId == 0 ? true : // SuperAdmin check
                                hasRoleE ? true : // Company Admin can see all departments
                                currentUser.UserDepartmentId == a.DepartmentId // Department check for other roles
                            )
                        ).ToList();


                        //viewModelWI = viewModelWI.Where(a => a.IsActive == true && (currentUser.UserCompanyId == 0 ? true : currentUser.UserCompanyId == a.CompanyId) && (currentUser.UserDepartmentId == 0 ? true : currentUser.UserDepartmentId == a.DepartmentId)).ToList();

                        foreach (var item in viewModelWI)
                        {
                            var psStatat = _context.WIStatus.Where(a => a.WIId == item.Id).ToList();
                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.WIStatus.Where(a => a.WIId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.WIStatusView = StatusId.DocumentStatus.Name;
                            }
                        }
                        viewModelWI = viewModelWI.Where(w => w.WIStatusView == "Approved").ToList();
                        var docNoList = viewModelWI.Select(s => s.WSCPNo).Distinct().OrderBy(o => o).ToList();
                        var wvmList = new List<WIViewModel>();
                        foreach (var docNo in docNoList)
                        {
                            var pvmList = viewModelWI.Where(w => w.WSCPNo == docNo).ToList();
                            var sdocNoList = pvmList.Select(s => s.SOPNo).Distinct().OrderBy(o => o).ToList();
                            foreach (var sdocNo in sdocNoList)
                            {
                                var svmList = viewModelWI.Where(w => w.WSCPNo == docNo && w.SOPNo == sdocNo).ToList();
                                var wdocNoList = svmList.Select(s => s.WINo).Distinct().OrderBy(o => o).ToList();
                                foreach (var wdocNo in wdocNoList)
                                {
                                    var wdocNoList2 = viewModelWI.Where(w => w.WSCPNo == docNo && w.SOPNo == sdocNo && w.WINo == wdocNo).ToList();
                                    wvmList.Add(wdocNoList2.LastOrDefault());
                                }
                            }
                        }
                        viewModelWI = wvmList;
                    }
                    foreach (var Issinfo in issuanceViewModel.IssuanceInfo)
                    {
                        if (Issinfo.DocType == "WSCP" || Issinfo.DocType == null)
                        {
                            if (responseWSCP.Succeeded)
                            {
                                List<SelectListItem> lstDocNo = new List<SelectListItem>();
                                foreach (var vm in viewModelWSCP)
                                {
                                    SelectListItem sli = new SelectListItem();
                                    sli.Value = vm.Id.ToString();
                                    sli.Text = vm.WSCPNo;
                                    lstDocNo.Add(sli);
                                }
                                Issinfo.DOCNos = new SelectList(lstDocNo, "Value", "Text");
                            }
                        }
                        else if (Issinfo.DocType == "SOP")
                        {
                            if (responseSOP.Succeeded)
                            {
                                List<SelectListItem> lstDocNo = new List<SelectListItem>();
                                foreach (var vm in viewModelSOP)
                                {
                                    SelectListItem sli = new SelectListItem();
                                    sli.Value = vm.Id.ToString();
                                    sli.Text = vm.SOPNo + " <" + vm.WSCPNo + ">";
                                    lstDocNo.Add(sli);
                                }
                                Issinfo.DOCNos = new SelectList(lstDocNo, "Value", "Text");
                            }
                        }
                        else if (Issinfo.DocType == "WI")
                        {
                            if (responseWI.Succeeded)
                            {
                                List<SelectListItem> lstDocNo = new List<SelectListItem>();
                                foreach (var vm in viewModelWI)
                                {
                                    SelectListItem sli = new SelectListItem();
                                    sli.Value = vm.Id.ToString();
                                    sli.Text = vm.WINo + " <" + vm.WSCPNo + "|" + vm.SOPNo + ">";
                                    lstDocNo.Add(sli);
                                }
                                Issinfo.DOCNos = new SelectList(lstDocNo, "Value", "Text");
                            }
                        }
                    }
                    // Verified
                    var responseVer = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelVer = (from a1 in responseVer
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserVerifiedId = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    issuanceViewModel.UserListVer = new SelectList(userViewModelVer, "UserVerifiedId", "FullName");

                    // Approver
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelAPP = (from a1 in responseAPP
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserApprovedId = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    issuanceViewModel.UserListApp = new SelectList(userViewModelAPP, "UserApprovedId", "FullName");

                    // Acknowledge //by companyAdmin
                    //var responseACK = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    //var userViewModelACK = (from a1 in responseACK
                    //                        join a2 in _userManager.Users on a1.UserId equals a2.Id
                    //                        select new UserApproverViewModel
                    //                        {
                    //                            UserAcknowledgedBy = a1.UserId,
                    //                            FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                    //                        }).OrderBy(a => a.Email).ToList();
                    //issuanceViewModel.UserListAck = new SelectList(userViewModelACK, "UserAcknowledgedBy", "FullName");

                    return View(issuanceViewModel);
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
            var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });

            var statusById = _context.IssuanceStatus.Where(a => a.IssuanceId == id).ToList();

            if (response.Succeeded)
            {
                var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == issuanceViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        //var QADeptId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id; //KIV
                        //if (user.UserDepartmentId == QADeptId)
                        //    rolesListDept.AddRange(roles);
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
                    var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    issuanceViewModel.IssuanceStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    issuanceViewModel.IssuanceStatusView = "New";
                }

                if (issuanceViewModel.VerifiedBy != null)
                {
                    var VerifiedUser = _userManager.Users.Where(a => a.Id == issuanceViewModel.VerifiedBy).SingleOrDefault();
                    issuanceViewModel.VerifiedName = VerifiedUser.FirstName + " " + VerifiedUser.LastName;
                    issuanceViewModel.VerifiedBy = VerifiedUser.Id;
                }

                if (issuanceViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == issuanceViewModel.ApprovedBy).SingleOrDefault();
                    issuanceViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                }

                if (issuanceViewModel.AcknowledgedBy != null)
                {
                    var AcknowledgedByUser = _userManager.Users.Where(a => a.Id == issuanceViewModel.AcknowledgedBy).SingleOrDefault();
                    issuanceViewModel.AcknowledgedBy = AcknowledgedByUser.FirstName + " " + AcknowledgedByUser.LastName;
                }

                return PartialView("_ViewIssuanceById", issuanceViewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAllNew()
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

            var response = await _mediator.Send(new GetAllIssuancesCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<IssuanceViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && a.DOCStatus == "New" && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2")) ? true : listDept.Contains(a.DepartmentId))).ToList();

                if (!rolesList.Contains("A") && !rolesList.Contains("SuperAdmin"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }

                foreach (IssuanceViewModel item in viewModel)
                {
                    var psStatat = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        if (StatusId.DocumentStatus.Name == "Concurred1")
                        {
                            item.IssuanceStatusView = "Verified";
                        }
                        else if (StatusId.DocumentStatus.Name == "Approved")
                        {
                            item.IssuanceStatusView = "Approved";
                        }
                        else if (StatusId.DocumentStatus.Name == "Acknowledged")
                        {
                            item.IssuanceStatusView = "Acknowledged";
                        }
                        else
                        {
                            item.IssuanceStatusView = StatusId.DocumentStatus.Name;
                        }
                    }
                    else
                    {
                        item.IssuanceStatusView = "New";
                    }
                }
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
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) && a.IssuanceStatusView == "Approved").ToList();
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAllAmend()
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

            var response = await _mediator.Send(new GetAllIssuancesCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<IssuanceViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && a.DOCStatus == "Amend").ToList();

                if (!rolesList.Contains("A") && !rolesList.Contains("SuperAdmin"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }

                foreach (IssuanceViewModel item in viewModel)
                {
                    var psStatat = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        if (StatusId.DocumentStatus.Name == "Concurred1")
                        {
                            item.IssuanceStatusView = "Verified";
                        }
                        else if (StatusId.DocumentStatus.Name == "Approved")
                        {
                            item.IssuanceStatusView = "Approved";
                        }
                        else if (StatusId.DocumentStatus.Name == "Acknowledged")
                        {
                            item.IssuanceStatusView = "Acknowledged";
                        }
                        else
                        {
                            item.IssuanceStatusView = StatusId.DocumentStatus.Name;
                        }
                    }
                    else
                    {
                        item.IssuanceStatusView = "New";
                    }
                }
                if (rolesList.Contains("A") || rolesList.Contains("SuperAdmin"))
                {
                    viewModel = viewModel.ToList();
                }
                else if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
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
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) && a.IssuanceStatusView == "Approved").ToList();
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadAllPrint()
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

            var responseH = await _mediator.Send(new GetAllIssuancesCachedQuery());
            var responseD = await _mediator.Send(new GetAllIssuancesInfoCachedQuery());
            var responseP = await _mediator.Send(new GetAllIssuancesInfoPrintCachedQuery());

            if (responseH.Succeeded)
            {
                var viewModelH = _mapper.Map<List<IssuanceViewModel>>(responseH.Data);
                viewModelH = viewModelH.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId))).ToList();
                foreach (var item in viewModelH)
                {
                    var psStatat = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        if (StatusId.DocumentStatus.Name == "Concurred1")
                        {
                            item.IssuanceStatusView = "Verified";
                        }
                        else if (StatusId.DocumentStatus.Name == "Approved")
                        {
                            item.IssuanceStatusView = "Approved";
                        }
                        else if (StatusId.DocumentStatus.Name == "Acknowledged")
                        {
                            item.IssuanceStatusView = "Acknowledged";
                        }
                        else
                        {
                            item.IssuanceStatusView = StatusId.DocumentStatus.Name;
                        }
                    }
                    else
                    {
                        item.IssuanceStatusView = "New";
                    }
                }
                viewModelH = viewModelH.Where(w => w.IssuanceStatusView == "Acknowledged").ToList();
                var viewModelD = _mapper.Map<List<IssuanceInfoViewModel>>(responseD.Data);
                viewModelD = viewModelD.Where(a => a.IsActive == true && viewModelH.Select(s => s.Id).Contains(a.HId)).ToList();
                var viewModelP = _mapper.Map<List<IssuanceInfoPrintViewModel>>(responseP.Data);
                viewModelP = viewModelP.Where(a => a.IsActive == true && viewModelD.Select(s => s.Id).Contains(a.IssInfoId)).ToList();
                foreach (var vmP in viewModelP)
                {
                    var vmD = viewModelD.Where(w => w.Id == vmP.IssInfoId).FirstOrDefault();
                    var vmH = viewModelH.Where(w => w.Id == vmD.HId).FirstOrDefault();
                    vmP.IssId = vmH.Id.ToString();
                    if (vmD.DocType == "WSCP")
                    {
                        vmP.DocUrl = "/documentation/procedure/Preview?id=" + vmD.DOCId;
                        var responseWSCP = await _mediator.Send(new GetProcedureByIdQuery() { Id = Convert.ToInt32(vmD.DOCId) });
                        if (responseWSCP.Succeeded)
                        {
                            var procedureViewModel = _mapper.Map<ProcedureViewModel>(responseWSCP.Data);
                            vmP.DocNo = procedureViewModel.WSCPNo;
                        }
                    }
                    else if (vmD.DocType == "SOP")
                    {
                        vmP.DocUrl = "/documentation/sop/Preview?id=" + vmD.DOCId;
                        var responseSOP = await _mediator.Send(new GetSOPByIdQuery() { Id = Convert.ToInt32(vmD.DOCId) });
                        if (responseSOP.Succeeded)
                        {
                            var sopViewModel = _mapper.Map<SOPViewModel>(responseSOP.Data);
                            vmP.DocNo = sopViewModel.SOPNo + " <" + sopViewModel.WSCPNo + ">";
                        }
                    }
                    else if (vmD.DocType == "WI")
                    {
                        vmP.DocUrl = "/documentation/wi/Preview?id=" + vmD.DOCId;
                        var responseWI = await _mediator.Send(new GetWIByIdQuery() { Id = Convert.ToInt32(vmD.DOCId) });
                        if (responseWI.Succeeded)
                        {
                            var wiViewModel = _mapper.Map<WIViewModel>(responseWI.Data);
                            vmP.DocNo = wiViewModel.WINo + " <" + wiViewModel.WSCPNo + "|" + wiViewModel.SOPNo + ">";
                        }
                    }
                }
                return PartialView("_ViewAllPrint", viewModelP);
            }
            return null;
        }
        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
        {
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            if (id == 0)
            {
                var issuanceViewModel = new IssuanceViewModel();
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", issuanceViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", issuanceViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, IssuanceViewModel issuance)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createIssuanceCommand = _mapper.Map<CreateIssuanceCommand>(issuance);
                    var result = await _mediator.Send(createIssuanceCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Issuance with ID {result.Data} Created.");
                        foreach (var info in issuance.IssuanceInfo)
                        {
                            info.HId = id;
                            if (!info.Deleted)
                            {
                                var createIssuanceInfoCommand = _mapper.Map<CreateIssuanceInfoCommand>(info);
                                var resultInfo = await _mediator.Send(createIssuanceInfoCommand);
                                var issInfoId = resultInfo.Data;
                            }
                            //if(info.RecipientName1 != "" && info.RecipientName1 != null)
                            //{
                            //    IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                            //    iiprint.IssInfoId = issInfoId;
                            //    iiprint.RecipientName = info.RecipientName1;
                            //    var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                            //    var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                            //}
                            //if (info.RecipientName2 != "" && info.RecipientName2 != null)
                            //{
                            //    IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintVieemaiwModel();
                            //    iiprint.IssInfoId = issInfoId;
                            //    iiprint.RecipientName = info.RecipientName2;
                            //    var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                            //    var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                            //}
                            //if (info.RecipientName3 != "" && info.RecipientName3 != null)
                            //{
                            //    IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                            //    iiprint.IssInfoId = issInfoId;
                            //    iiprint.RecipientName = info.RecipientName3;
                            //    var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                            //    var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                            //}
                            //if (info.RecipientName4 != "" && info.RecipientName4 != null)
                            //{
                            //    IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                            //    iiprint.IssInfoId = issInfoId;
                            //    iiprint.RecipientName = info.RecipientName4;
                            //    var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                            //    var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                            //}
                            //if (info.RecipientName5 != "" && info.RecipientName5 != null)
                            //{
                            //    IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                            //    iiprint.IssInfoId = issInfoId;
                            //    iiprint.RecipientName = info.RecipientName5;
                            //    var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                            //    var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                            //}
                            //if (info.RecipientName6 != "" && info.RecipientName6 != null)
                            //{
                            //    IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                            //    iiprint.IssInfoId = issInfoId;
                            //    iiprint.RecipientName = info.RecipientName6;
                            //    var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                            //    var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                            //}
                        }
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateIssuanceCommand = _mapper.Map<UpdateIssuanceCommand>(issuance);
                    var result = await _mediator.Send(updateIssuanceCommand);
                    if (result.Succeeded) _notify.Information($"Issuance with ID {result.Data} Updated.");
                    foreach (var info in issuance.IssuanceInfo)
                    {
                        if (info.Id == 0)
                        {
                            info.HId = id;
                            if (!info.Deleted)
                            {
                                var createIssuanceInfoCommand = _mapper.Map<CreateIssuanceInfoCommand>(info);
                                var resultInfo = await _mediator.Send(createIssuanceInfoCommand);
                            }
                        }
                        else
                        {
                            if (info.Deleted)
                                info.IsActive = false;
                            var updateIssuanceInfoCommand = _mapper.Map<UpdateIssuanceInfoCommand>(info);
                            var resultInfo = await _mediator.Send(updateIssuanceInfoCommand);
                        }
                    }
                }
                if (issuance.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetIssuanceByIdQuery() { Id = issuance.ArchiveId });
                    if (response2.Succeeded)
                    {
                        var issuanceViewModelOld = _mapper.Map<IssuanceViewModel>(response2.Data);
                        issuanceViewModelOld.IsArchive = true;
                        var archiveDt = DateTime.Now.AddDays(30);
                        issuanceViewModelOld.ArchiveDate = archiveDt;
                        var updateIssuanceCommand = _mapper.Map<UpdateIssuanceCommand>(issuanceViewModelOld);
                        var result2 = await _mediator.Send(updateIssuanceCommand);
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateIssuanceImageCommand() { Id = id, Image = image });
                }

                var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("Create", issuance);
                return View(nameof(IndexNewAsync));
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
                var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);
                    ViewBag.docStatus = issuanceViewModel.DOCStatus;
                    issuanceViewModel.IsActive = false;
                    var updateIssuanceCommand = _mapper.Map<UpdateIssuanceCommand>(issuanceViewModel);
                    var result = await _mediator.Send(updateIssuanceCommand);
                    if (result.Succeeded)
                        _notify.Information($"Issuance with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllIssuancesCachedQuery());
                    if (response2.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<IssuanceViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                        }
                        foreach (IssuanceViewModel item in viewModel)
                        {
                            var psStatat = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.IssuanceStatusView = StatusId.DocumentStatus.Name;
                            }
                            else
                            {
                                item.IssuanceStatusView = "New";
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

        //    var deleteCommand = await _mediator.Send(new DeleteIssuanceCommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Issuance with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllIssuancesCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<IssuanceViewModel>>(response.Data);

        //            if (rolesList.Contains("D"))
        //            {
        //                viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
        //            }

        //            foreach (IssuanceViewModel item in viewModel)
        //            {
        //                var psStatat = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).ToList();

        //                if (psStatat.Count != 0)
        //                {
        //                    var StatusId = _context.IssuanceStatus.Where(a => a.IssuanceId == item.Id).OrderBy(a => a.CreatedOn)
        //                        .Include(a => a.DocumentStatus)
        //                        .Last();
        //                    item.IssuanceStatusView = StatusId.DocumentStatus.Name;
        //                }
        //                else
        //                {
        //                    item.IssuanceStatusView = "New";
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
            var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Assignment", issuanceViewModel) });
            }

            return null;

        }

        public async Task<JsonResult> OnGetAssignConcurred1(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            var departmentsResponseC1 = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModelC1 = _mapper.Map<List<DepartmentViewModel>>(departmentsResponseC1.Data);
            var qaId = departmentViewModelC1.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptIdC1 = departmentViewModelC1.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            int allCompId = 0;
            var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });

            var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptIdC1)).ToList();

            if (response.Succeeded)
            {
                var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);

                var userViewModel = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                issuanceViewModel.UserList = new SelectList(userViewModel, "UserConcurred1Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred1", issuanceViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignConcurred2(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            var departmentsResponseC2 = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModelC2 = _mapper.Map<List<DepartmentViewModel>>(departmentsResponseC2.Data);
            var qaId = departmentViewModelC2.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptIdC2 = departmentViewModelC2.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            int allCompId = 0;
            var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });

            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptIdC2)).ToList();

            if (response.Succeeded)
            {
                var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);

                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();

                issuanceViewModel.UserList = new SelectList(userViewModel, "UserConcurred2Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred2", issuanceViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignApprovedBy(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
            var qaId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            int allCompId = 0;
            var response = await _mediator.Send(new GetIssuanceByIdQuery() { Id = id });
            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var issuanceViewModel = _mapper.Map<IssuanceViewModel>(response.Data);

                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();

                issuanceViewModel.UserList = new SelectList(userViewModel, "UserApproveBy", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignApprovedBy", issuanceViewModel) });
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmitAssignment(int id, IssuanceViewModel issuance)
        {
            if (id == 0)
            {
                var createIssuanceCommand = _mapper.Map<CreateIssuanceCommand>(issuance);
                var result = await _mediator.Send(createIssuanceCommand);
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Issuance with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
            }

            else
            {
                issuance.VerifiedBy = issuance.VerifiedBy;
                var updateIssuanceCommand = _mapper.Map<UpdateIssuanceCommand>(issuance);
                var result = await _mediator.Send(updateIssuanceCommand);
                if (result.Succeeded) _notify.Information($"Issuance Assignment with ID {result.Data} Updated.");
            }
            var response = await _mediator.Send(new GetAllIssuancesCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<IssuanceViewModel>>(response.Data);
                var html2 = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                return new JsonResult(new { isValid = true, html = html2 });
            }
            else
            {
                _notify.Error(response.Message);
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostPrint(int id, IssuanceInfoPrintViewModel issInfoP, bool amend = false)
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
                if (!amend)
                {
                    var response = await _mediator.Send(new GetIssuanceInfoPrintByIdQuery() { Id = id });
                    if (response.Succeeded)
                    {
                        var issuanceInfoPrintViewModel = _mapper.Map<IssuanceInfoPrintViewModel>(response.Data);

                        issuanceInfoPrintViewModel.IsPrinted = true;
                        issuanceInfoPrintViewModel.PrintedDate = null; //update when print
                        issuanceInfoPrintViewModel.PrintedBy = currentUser.Id;
                        var updateIssuanceInfoPrintCommand = _mapper.Map<UpdateIssuanceInfoPrintCommand>(issuanceInfoPrintViewModel);
                        var result = await _mediator.Send(updateIssuanceInfoPrintCommand);

                        var response0 = await _mediator.Send(new GetIssuanceInfoByIdQuery() { Id = issuanceInfoPrintViewModel.IssInfoId });
                        if (response0.Succeeded)
                        {
                            var issuanceInfoVM = _mapper.Map<IssuanceInfoViewModel>(response0.Data);
                            if (issuanceInfoVM.DocType == "WSCP")
                            {
                                issInfoP.DocUrl = "/documentation/procedure/Preview?id=" + issuanceInfoVM.DOCId;
                            }
                            else if (issuanceInfoVM.DocType == "SOP")
                            {
                                issInfoP.DocUrl = "/documentation/sop/Preview?id=" + issuanceInfoVM.DOCId;
                            }
                            else if (issuanceInfoVM.DocType == "WI")
                            {
                                issInfoP.DocUrl = "/documentation/wi/Preview?id=" + issuanceInfoVM.DOCId;
                            }
                        }
                        var url = issInfoP.DocUrl + "&IPrint=" + issuanceInfoPrintViewModel.IssInfoId;
                        return Redirect(url);
                    }
                }
                else
                {
                    var response0 = await _mediator.Send(new GetIssuanceInfoByIdQuery() { Id = id });
                    if (response0.Succeeded)
                    {
                        var issuanceInfoVM = _mapper.Map<IssuanceInfoViewModel>(response0.Data);
                        if (issuanceInfoVM.DocType == "WSCP")
                        {
                            issInfoP.DocUrl = "/documentation/procedure/CreateOrEdit?id=" + issuanceInfoVM.DOCId + "&rev=true&&ce=bd773a2a-ffc6-4ed2-9ba3-c7ea631628be";
                        }
                        else if (issuanceInfoVM.DocType == "SOP")
                        {
                            issInfoP.DocUrl = "/documentation/sop/CreateOrEdit?id=" + issuanceInfoVM.DOCId + "&rev=true&&ce=bd773a2a-ffc6-4ed2-9ba3-c7ea631628be";
                        }
                        else if (issuanceInfoVM.DocType == "WI")
                        {
                            issInfoP.DocUrl = "/documentation/wi/CreateOrEdit?id=" + issuanceInfoVM.DOCId + "&rev=true&&ce=bd773a2a-ffc6-4ed2-9ba3-c7ea631628be";
                        }
                    }
                    var url = issInfoP.DocUrl;
                    return Redirect(url);
                }
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostReturn(int id, IssuanceInfoPrintViewModel issInfoP)
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
                var response = await _mediator.Send(new GetIssuanceInfoPrintByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var issuanceInfoPrintViewModel = _mapper.Map<IssuanceInfoPrintViewModel>(response.Data);
                    var responseD = await _mediator.Send(new GetIssuanceInfoByIdQuery() { Id = issuanceInfoPrintViewModel.IssInfoId });
                    if (responseD.Succeeded)
                    {
                        //printCount is number count, check status by printed/ returned
                        //var issInfoVM = _mapper.Map<IssuanceInfoViewModel>(responseD.Data);
                        //if (issInfoVM.DocType == "WSCP")
                        //{
                        //    var responseProd = await _mediator.Send(new GetProcedureByIdQuery() { Id = Convert.ToInt32(issInfoVM.DOCId) });
                        //    if (responseProd.Succeeded)
                        //    {
                        //        var prodVM = _mapper.Map<ProcedureViewModel>(responseProd.Data);
                        //        prodVM.PrintCount = prodVM.PrintCount - 1;
                        //        var updateProcedureCommand = _mapper.Map<UpdateProcedureCommand>(prodVM);
                        //        var resultprod = await _mediator.Send(updateProcedureCommand);
                        //    }
                        //}
                        //else if (issInfoVM.DocType == "SOP")
                        //{
                        //    var responseSOP = await _mediator.Send(new GetSOPByIdQuery() { Id = Convert.ToInt32(issInfoVM.DOCId) });
                        //    if (responseSOP.Succeeded)
                        //    {                               
                        //        var sopVM = _mapper.Map<SOPViewModel>(responseSOP.Data);
                        //        sopVM.PrintCount = sopVM.PrintCount - 1;
                        //        var updateSOPCommand = _mapper.Map<UpdateSOPCommand>(sopVM);
                        //        var resultsop = await _mediator.Send(updateSOPCommand);
                        //    }
                        //}
                        //else if (issInfoVM.DocType == "WI")
                        //{
                        //    var responseWI = await _mediator.Send(new GetWIByIdQuery() { Id = Convert.ToInt32(issInfoVM.DOCId) });
                        //    if (responseWI.Succeeded)
                        //    {
                        //        var wiVM = _mapper.Map<WIViewModel>(responseWI.Data);
                        //        wiVM.PrintCount = wiVM.PrintCount - 1;
                        //        var updateWICommand = _mapper.Map<UpdateWICommand>(wiVM);
                        //        var resultwi = await _mediator.Send(updateWICommand);
                        //    }
                        //}
                    }

                    issuanceInfoPrintViewModel.IsReturned = true;
                    issuanceInfoPrintViewModel.ReturnedDate = DateTime.Now;
                    issuanceInfoPrintViewModel.ReturnedBy = currentUser.Id;
                    var updateIssuanceInfoPrintCommand = _mapper.Map<UpdateIssuanceInfoPrintCommand>(issuanceInfoPrintViewModel);
                    var result = await _mediator.Send(updateIssuanceInfoPrintCommand);
                    return RedirectToAction("IndexPrint");
                }
            }
            return null;
        }
    }
}

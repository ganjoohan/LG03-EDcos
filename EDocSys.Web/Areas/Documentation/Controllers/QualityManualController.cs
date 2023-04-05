using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManuals.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManuals.Commands.Create;
using EDocSys.Application.Features.QualityManuals.Commands.Delete;
using EDocSys.Application.Features.QualityManuals.Commands.Update;
using EDocSys.Application.Features.QualityManuals.Queries.GetById;
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
using EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllCached;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using EDocSys.Application.Features.Issuances.Queries.GetById;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class QualityManualController : BaseController<QualityManualController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public QualityManualController(ApplicationDbContext context,
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
            var model = new QualityManualViewModel();
            return View(model);
        }

        //public IActionResult Preview1()
        //{
        //    var model = new QualityManualViewModel();
        //    return View(model);
        //}

        [Authorize(Policy = "CanViewQualityManual")]
        public async Task<IActionResult> Preview(int id, bool print = false, QualityManualViewModel qlyManual = null, bool revert = false)
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
            if (revert)
            {
                var response2 = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
                var qualityManualViewModelOld = _mapper.Map<LabAccreditationManualViewModel>(response2.Data);
                qualityManualViewModelOld.IsArchive = false;
                qualityManualViewModelOld.ArchiveDate = null;
                var updateQualityManualCommandOld = _mapper.Map<UpdateQualityManualCommand>(qualityManualViewModelOld);
                var result2 = await _mediator.Send(updateQualityManualCommandOld);
            }
            var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
            var adg = _context.QualityManualStatus;
            var statusById = _context.QualityManualStatus.Where(a => a.QualityManualId == id).ToList();

            if (response.Succeeded)
            {
                var qualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);
                if (print || qlyManual.PrintCount != 0)
                {
                    if (qlyManual.PrintCount != qualityManualViewModel.PrintCount)
                        qualityManualViewModel.PrintCount = qlyManual.PrintCount;
                    else
                        qualityManualViewModel.PrintCount = qualityManualViewModel.PrintCount + 1;
                    var updateQualityManualCommand = _mapper.Map<UpdateQualityManualCommand>(qualityManualViewModel);
                    var result = await _mediator.Send(updateQualityManualCommand);
                    return RedirectToAction("Preview", new { Id = id });
                }
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == qualityManualViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        var QADeptId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
                        if (user.UserDepartmentId == QADeptId)
                            rolesListDept.AddRange(roles);
                    }
                }
                var userChk = users.Select(s => s.UserCompanyId).Contains(qualityManualViewModel.CompanyId);
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
                if (rolesListComp.Contains("E") && userChk)
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
                // QualityManualViewModel.pro String.Format("{0:y yy yyy yyyy}", dt);  // "8 08 008 2008"   year

                // QualityManualViewModel.PreparedByDate = String.Format("{0:dd/mm/yyyy}", QualityManualViewModel.PreparedByDate);

                if (statusById.Count() != 0)
                {
                    var StatusId = _context.QualityManualStatus.Where(a => a.QualityManualId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    qualityManualViewModel.QualityManualStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    qualityManualViewModel.QualityManualStatusView = "New";
                }

                if (qualityManualViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == qualityManualViewModel.Concurred1).SingleOrDefault();
                    if (concurred1User != null)
                    {
                        qualityManualViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                        qualityManualViewModel.Concurred1 = concurred1User.Id;
                        qualityManualViewModel.PositionC1 = concurred1User.Position;
                    }
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
                                    qualityManualViewModel.DateApprovedC1 = c1StatusDt;
                                }
                            }
                            else
                            {
                                qualityManualViewModel.DateApprovedC1 = c1StatusDt;
                            }
                        }
                    }
                }

                if (qualityManualViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == qualityManualViewModel.Concurred2).SingleOrDefault();
                    if (concurred2User != null)
                    {
                        qualityManualViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                        qualityManualViewModel.Concurred2 = concurred2User.Id;
                        qualityManualViewModel.PositionC2 = concurred2User.Position;
                    }
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
                                    qualityManualViewModel.DateApprovedC2 = c1StatusDt;
                                }
                            }
                            else
                            {
                                qualityManualViewModel.DateApprovedC2 = c1StatusDt;
                            }
                        }
                    }
                }

                if (qualityManualViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == qualityManualViewModel.ApprovedBy).SingleOrDefault();
                    if (ApprovedByUser != null)
                    {
                        qualityManualViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                        qualityManualViewModel.PositionApp = ApprovedByUser.Position;
                    }
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
                                    qualityManualViewModel.DateApprovedAPP = c1StatusDt;
                                }
                            }
                            else
                            {
                                qualityManualViewModel.DateApprovedAPP = c1StatusDt;
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





                return View(qualityManualViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditQualityManual")]
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
            int allCompId = 0;
            if (id == 0)
            {
                var qualityManualViewModel = new QualityManualViewModel();
                qualityManualViewModel.EstalishedDate = DateTime.Now;
                qualityManualViewModel.PreparedByDate = DateTime.Now;
                qualityManualViewModel.Category = "Documentation";
                qualityManualViewModel.SectionNo = "";
                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }

                    qualityManualViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }
                // Concurred 1
                var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC1 = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();
                qualityManualViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC2 = (from a1 in responseC2
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                qualityManualViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                var userViewModelAPP = (from a1 in responseAPP
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserApproveBy = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                qualityManualViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "DOC_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;
                return View(qualityManualViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);
                    var QualityManualViewModelOld = _mapper.Map<QualityManualViewModel>(response.Data);
                    if (rev)
                    {
                        QualityManualViewModel.Id = 0;
                        var revNo = QualityManualViewModel.RevisionNo != null ? QualityManualViewModel.RevisionNo : 0;
                        QualityManualViewModel.RevisionNo = revNo + 1;
                        QualityManualViewModel.RevisionDate = DateTime.Now;
                        QualityManualViewModel.EffectiveDate = null;
                        QualityManualViewModel.EstalishedDate = DateTime.Now;
                        QualityManualViewModel.PreparedByDate = DateTime.Now;
                        QualityManualViewModel.ArchiveId = QualityManualViewModelOld.Id;
                        QualityManualViewModel.PrintCount = 0;
                    }
                    //QualityManualViewModel.RevisionNo = QualityManualViewModel.RevisionNo == null ? 1 : (QualityManualViewModel.RevisionNo + 1);
                    //QualityManualViewModel.RevisionDate = DateTime.Now;

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        QualityManualViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }

                    // Concurred 1
                    var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC1 = (from a1 in responseC1
                                         join a2 in _userManager.Users on a1.UserId equals a2.Id
                                         select new UserApproverViewModel
                                         {
                                             UserConcurred1Id = a1.UserId,
                                             FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                         }).OrderBy(a => a.Email).ToList();
                    QualityManualViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    QualityManualViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelAPP = (from a1 in responseAPP
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserApproveBy = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    QualityManualViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");


                    return View(QualityManualViewModel);
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
          
            var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });

            var statusById = _context.QualityManualStatus.Where(a => a.QualityManualId == id).ToList();

            if (response.Succeeded)
            {
                var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == QualityManualViewModel.CompanyId)
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
                    var StatusId = _context.QualityManualStatus.Where(a => a.QualityManualId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    QualityManualViewModel.QualityManualStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    QualityManualViewModel.QualityManualStatusView = "New";
                }

                if (QualityManualViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == QualityManualViewModel.Concurred1).SingleOrDefault();
                    QualityManualViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    QualityManualViewModel.Concurred1 = concurred1User.Id;
                }

                if (QualityManualViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == QualityManualViewModel.Concurred2).SingleOrDefault();
                    QualityManualViewModel.Concurred2 = concurred2User.FirstName + " " + concurred2User.LastName;
                }

                if (QualityManualViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == QualityManualViewModel.ApprovedBy).SingleOrDefault();
                    QualityManualViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                }

                return PartialView("_ViewQualityManualById", QualityManualViewModel);
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
                ViewBag.userIds = ViewBag.userIds != "" ? (ViewBag.userIds + "," + user.Id) : user.Id;
            }
            if (rolesList.Contains("A"))
            {
                ViewBag.RoleA = true;
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

            var response = await _mediator.Send(new GetAllQualityManualsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<QualityManualViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId))).ToList();

                if (!rolesList.Contains("A") && !rolesList.Contains("SuperAdmin"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }

                foreach (QualityManualViewModel item in viewModel)
                {
                    var psStatat = _context.QualityManualStatus.Where(a => a.QualityManualId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.QualityManualStatus.Where(a => a.QualityManualId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        item.QualityManualStatusView = StatusId.DocumentStatus.Name;
                    }
                    else
                    {
                        item.QualityManualStatusView = "New";
                    }
                }
                if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }
                else if(rolesList.Contains("C"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && a.QualityManualStatusView == "Approved").ToList();
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
                var QualityManualViewModel = new QualityManualViewModel();
                QualityManualViewModel.Category = "Documentation";
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", QualityManualViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", QualityManualViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, QualityManualViewModel QualityManual)
        {
            // QualityManual.EffectiveDate.Value.ToString("g");



            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createQualityManualCommand = _mapper.Map<CreateQualityManualCommand>(QualityManual);
                    var result = await _mediator.Send(createQualityManualCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Quality Manual with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateQualityManualCommand = _mapper.Map<UpdateQualityManualCommand>(QualityManual);
                    var result = await _mediator.Send(updateQualityManualCommand);
                    if (result.Succeeded) _notify.Information($"Quality Manual with ID {result.Data} Updated.");
                }
                if (QualityManual.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetQualityManualByIdQuery() { Id = QualityManual.ArchiveId });
                    if (response2.Succeeded)
                    {
                        var qualityManualViewModelOld = _mapper.Map<QualityManualViewModel>(response2.Data);
                        qualityManualViewModelOld.IsArchive = true;
                        var archiveDt = DateTime.Now.AddDays(30);
                        qualityManualViewModelOld.ArchiveDate = archiveDt;
                        var updateQualityManualCommand = _mapper.Map<UpdateQualityManualCommand>(qualityManualViewModelOld);
                        var result2 = await _mediator.Send(updateQualityManualCommand);
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateQualityManualImageCommand() { Id = id, Image = image });
                }

                var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("Create", QualityManual);
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
                var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);
                    QualityManualViewModel.IsActive = false;
                    var updateQualityManualCommand = _mapper.Map<UpdateQualityManualCommand>(QualityManualViewModel);
                    var result = await _mediator.Send(updateQualityManualCommand);
                    if (result.Succeeded)
                        _notify.Information($"Quality Manual with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllQualityManualsCachedQuery());
                    if (response2.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<QualityManualViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                        }
                        foreach (QualityManualViewModel item in viewModel)
                        {
                            var psStatat = _context.QualityManualStatus.Where(a => a.QualityManualId == item.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.QualityManualStatus.Where(a => a.QualityManualId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.QualityManualStatusView = StatusId.DocumentStatus.Name;
                            }
                            else
                            {
                                item.QualityManualStatusView = "New";
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

        //    var deleteCommand = await _mediator.Send(new DeleteQualityManualCommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Quality Manual with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllQualityManualsCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<QualityManualViewModel>>(response.Data);

        //            if (rolesList.Contains("D"))
        //            {
        //                viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
        //            }

        //            foreach (QualityManualViewModel item in viewModel)
        //            {
        //                var psStatat = _context.QualityManualStatus.Where(a => a.QualityManualId == item.Id).ToList();

        //                if (psStatat.Count != 0)
        //                {
        //                    var StatusId = _context.QualityManualStatus.Where(a => a.QualityManualId == item.Id).OrderBy(a => a.CreatedOn)
        //                        .Include(a => a.DocumentStatus)
        //                        .Last();
        //                    item.QualityManualStatusView = StatusId.DocumentStatus.Name;
        //                }
        //                else
        //                {
        //                    item.QualityManualStatusView = "New";
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
            var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Assignment", QualityManualViewModel) });
            }

            return null;

        }

        public async Task<JsonResult> OnGetAssignConcurred1(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
            var qaId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            int allCompId = 0;
            var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });

            var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);

                var userViewModel = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                QualityManualViewModel.UserList = new SelectList(userViewModel, "UserConcurred1Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred1", QualityManualViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignConcurred2(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
            var qaId = departmentViewModel.Where(w => w.Name == "Quality Assurance").FirstOrDefault().Id;
            var allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            int allCompId = 0;
            var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });

            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);

                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();

                QualityManualViewModel.UserList = new SelectList(userViewModel, "UserConcurred2Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred2", QualityManualViewModel) });
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
            var response = await _mediator.Send(new GetQualityManualByIdQuery() { Id = id });
            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (a.DepartmentId == qaId || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var QualityManualViewModel = _mapper.Map<QualityManualViewModel>(response.Data);


                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                QualityManualViewModel.UserList = new SelectList(userViewModel, "UserApproveBy", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignApprovedBy", QualityManualViewModel) });
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmitAssignment(int id, QualityManualViewModel QualityManual)
        {
            if (id == 0)
            {
                var createQualityManualCommand = _mapper.Map<CreateQualityManualCommand>(QualityManual);
                var result = await _mediator.Send(createQualityManualCommand);
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Quality Manual with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
            }

            else
            {
                QualityManual.Concurred1 = QualityManual.Concurred1;
                var updateQualityManualCommand = _mapper.Map<UpdateQualityManualCommand>(QualityManual);
                var result = await _mediator.Send(updateQualityManualCommand);
                if (result.Succeeded) _notify.Information($"Quality Manual Assignment with ID {result.Data} Updated.");
            }
            var response = await _mediator.Send(new GetAllQualityManualsCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<QualityManualViewModel>>(response.Data);
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

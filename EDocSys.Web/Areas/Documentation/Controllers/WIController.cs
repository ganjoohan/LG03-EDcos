using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.WIs.Queries.GetAllCached;
using EDocSys.Application.Features.WIs.Commands.Create;
using EDocSys.Application.Features.WIs.Commands.Delete;
using EDocSys.Application.Features.WIs.Commands.Update;
using EDocSys.Application.Features.WIs.Queries.GetById;
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
using EDocSys.Application.Features.SOPs.Commands.Update;
using EDocSys.Application.Features.Procedures.Commands.Update;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;


namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class WIController : BaseController<WIController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public WIController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string sopno, int wscpId = 0, int sopId = 0)
        {
            var model = new WIViewModel();

            ViewBag.SOPNo = sopno;
            ViewBag.WSCPId = wscpId;
            ViewBag.SOPId = sopId;

            return View(model);
        }


        [Authorize(Policy = "CanViewWI")]
        public async Task<IActionResult> Preview(int id, bool print = false)
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
          
            var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });

            var statusById = _context.WIStatus.Where(a => a.WIId == id).ToList();

            if (response.Succeeded)
            {
                var wiViewModel = _mapper.Map<WIViewModel>(response.Data);
                if (print)
                {
                    wiViewModel.PrintCount = wiViewModel.PrintCount + 1;
                    var updateWICommand = _mapper.Map<UpdateWICommand>(wiViewModel);
                    var result = await _mediator.Send(updateWICommand);
                    return RedirectToAction("Preview", new { Id = id });
                }
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == wiViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                        if (user.UserDepartmentId == wiViewModel.DepartmentId)
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
                    var StatusId = _context.WIStatus.Where(a => a.WIId == id).OrderBy(a => a.CreatedOn)
                        .Include(a => a.DocumentStatus)
                        .Last();

                    wiViewModel.WIStatusView = StatusId.DocumentStatus.Name;
                }
                else
                {
                    wiViewModel.WIStatusView = "New";
                }

                if (wiViewModel.Concurred1 != null)
                {
                    var concurred1User = _userManager.Users.Where(a => a.Id == wiViewModel.Concurred1).SingleOrDefault();
                    wiViewModel.Concurred1Name = concurred1User.FirstName + " " + concurred1User.LastName;
                    wiViewModel.Concurred1 = concurred1User.Id;
                    wiViewModel.PositionC1 = concurred1User.Position;

                    if (statusById.Count() != 0)
                    {
                        var StatusId = _context.WIStatus.Where(a => a.WIId == id).OrderBy(a => a.CreatedOn).Select(a => a.DocumentStatusId).Last();
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
                                    wiViewModel.DateApprovedC1 = c1StatusDt;
                                }
                            }
                            else
                            {
                                wiViewModel.DateApprovedC1 = c1StatusDt;
                            }
                        }
                    }
                }

                if (wiViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == wiViewModel.Concurred2).SingleOrDefault();
                    wiViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                    wiViewModel.Concurred2 = concurred2User.Id;
                    wiViewModel.PositionC2 = concurred2User.Position;

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
                                    wiViewModel.DateApprovedC2 = c1StatusDt;
                                }
                            }
                            else
                            {
                                wiViewModel.DateApprovedC2 = c1StatusDt;
                            }
                        }
                    }

                }

                if (wiViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == wiViewModel.ApprovedBy).SingleOrDefault();
                    wiViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    wiViewModel.PositionApp = ApprovedByUser.Position;

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
                                    wiViewModel.DateApprovedAPP = c1StatusDt;
                                }
                            }
                            else
                            {
                                wiViewModel.DateApprovedAPP = c1StatusDt;
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



                return View(wiViewModel);
            }
            return null;
        }
        [Authorize(Policy = "CanCreateEditWI")]
        public async Task<IActionResult> CreateOrEdit(int id = 0, string wscpno = "", int wscpid = 0, string sopno = "", int sopid = 0, int departmentId = 0, int procedureId = 0, bool WIrev = false)
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
                var selectedDepartmentId = await _mediator.Send(new GetDepartmentByIdQuery() { Id = departmentId });

                var wiViewModel = new WIViewModel();
                if (departmentsResponse.Succeeded)
                {
                    wiViewModel.DepartmentId = selectedDepartmentId.Data.Id;
                    wiViewModel.ProcessName = selectedDepartmentId.Data.Name;

                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
                    wiViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);

                    wiViewModel.ProcedureId = procedureId;
                    wiViewModel.WSCPNo = wscpno;
                    wiViewModel.WSCPId = wscpid;
                    wiViewModel.SOPNo = sopno;
                    wiViewModel.SOPId = sopid;


                }

                //if (companiesResponse.Succeeded)
                //{
                //     var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                //     wiViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                // }
                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }

                    wiViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
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
                wiViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelC2 = (from a1 in responseC2
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                wiViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                var userViewModelAPP = (from a1 in responseAPP
                                        join a2 in _userManager.Users on a1.UserId equals a2.Id
                                        select new UserApproverViewModel
                                        {
                                            UserApproveBy = a1.UserId,
                                            FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                        }).OrderBy(a => a.Email).ToList();
                wiViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "WI_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;

                return View(wiViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var wiViewModel = _mapper.Map<WIViewModel>(response.Data);
                    var wiViewModelOld = _mapper.Map<WIViewModel>(response.Data);
                    if (WIrev)
                    {
                        wiViewModel.Id = 0;
                        var revNo = wiViewModel.RevisionNo != null ? wiViewModel.RevisionNo : 0;
                        wiViewModel.RevisionNo = revNo + 1;
                        var defDate = DateTime.Now;
                        wiViewModel.RevisionDate = defDate;
                        wiViewModel.EffectiveDate = null;
                        wiViewModel.EstalishedDate = defDate;
                        wiViewModel.PreparedByDate = defDate;
                        wiViewModel.ArchiveId = wiViewModelOld.Id;
                    }
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
                        wiViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        wiViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
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
                    wiViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    wiViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();
                    var userViewModelAPP = (from a1 in responseAPP
                                            join a2 in _userManager.Users on a1.UserId equals a2.Id
                                            select new UserApproverViewModel
                                            {
                                                UserApproveBy = a1.UserId,
                                                FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                            }).OrderBy(a => a.Email).ToList();
                    wiViewModel.UserListAPP = new SelectList(userViewModelAPP, "UserApproveBy", "FullName");

                    return View(wiViewModel);
                }
                return null;
            }

        }

        //public async Task<IActionResult> LoadAll()
        //{
        //    var response = await _mediator.Send(new GetAllWIsCachedQuery());

        //    if (response.Succeeded)
        //    {
        //        var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);

        //        return PartialView("_ViewAll", viewModel);
        //    }
        //    return null;
        //}

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

            var response = await _mediator.Send(new GetAllWIsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2")) ? true : listDept.Contains(a.DepartmentId))).ToList();
                // Access Category = D  
                // SOP Department Admin (Full Access by Department)
                if (rolesList.Contains("D"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                }

                // Access Category = E  
                // QMR / Lead Auditor / SOP Company Admin (Full Access by Company)


                foreach (WIViewModel item in viewModel)
                {
                    var psStatat = _context.WIStatus.Where(a => a.WIId == item.Id).ToList();

                    if (psStatat.Count != 0)
                    {
                        var StatusId = _context.WIStatus.Where(a => a.WIId == item.Id).OrderBy(a => a.CreatedOn)
                            .Include(a => a.DocumentStatus)
                            .Last();
                        item.WIStatusView = StatusId.DocumentStatus.Name;
                    }
                    else
                    {
                        item.WIStatusView = "New";
                    }
                }
                if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }
                else if (rolesList.Contains("C"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) && a.WIStatusView == "Approved").ToList();
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadBySOP(string sopno, int wscpid = 0, int sopid = 0)
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
            var response = await _mediator.Send(new GetAllWIsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);

                var viewModelbySOPNo = viewModel.Where(a => a.SOPNo == sopno && a.WSCPId == wscpid && a.SOPId == sopid && a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId)) && ((listDept.Contains(0) || rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2")) ? true : listDept.Contains(a.DepartmentId))).ToList();
                return PartialView("_ViewAll", viewModelbySOPNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
        {
            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());

            if (id == 0)
            {
                var wiViewModel = new WIViewModel();
                wiViewModel.EstalishedDate = DateTime.Now;
                wiViewModel.PreparedByDate = DateTime.Now;
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    wiViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", wiViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var wiViewModel = _mapper.Map<WIViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        wiViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", wiViewModel) });
                }
                return null;
            }
        }

 

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, WIViewModel wi)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createWICommand = _mapper.Map<CreateWICommand>(wi);
                    var result = await _mediator.Send(createWICommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"WI with ID {result.Data} Created.");

                        // Update Procedure
                        {
                            ProcedureViewModel procedure = new ProcedureViewModel();
                            procedure.Id = wi.ProcedureId;
                            procedure.WINo = wi.WINo;

                            var updateProcedureCommand = _mapper.Map<UpdateProcedureCommand>(procedure);
                            var resultProcedure = await _mediator.Send(updateProcedureCommand);
                            if (resultProcedure.Succeeded) _notify.Information($"Procedure with ID {result.Data} Updated.");
                        }

                        // Upload SOP
                        { 

                            SOPViewModel sop = new SOPViewModel();
                            sop.Id = wi.ProcedureId;
                            sop.WSCPNo = wi.WSCPNo;
                            sop.SOPNo = wi.SOPNo;
                            sop.WINo = wi.WINo;
                            sop.hasWI = true;

                            var updateSOPCommand = _mapper.Map<UpdateSOPCommand>(sop);
                            var resultSOP = await _mediator.Send(updateSOPCommand);
                            if (resultSOP.Succeeded) _notify.Information($"SOP with ID {result.Data} Updated.");
                        }
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateWICommand = _mapper.Map<UpdateWICommand>(wi);
                    var result = await _mediator.Send(updateWICommand);
                    if (result.Succeeded) _notify.Information($"WI with ID {result.Data} Updated.");
                }
                if (wi.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetWIByIdQuery() { Id = wi.ArchiveId });
                    if (response2.Succeeded)
                    {
                        var WIViewModelOld = _mapper.Map<WIViewModel>(response2.Data);
                        WIViewModelOld.IsArchive = true;
                        var archiveDt = DateTime.Now.AddDays(30);
                        WIViewModelOld.ArchiveDate = archiveDt;
                        var updateWICommandOld = _mapper.Map<UpdateSOPCommand>(WIViewModelOld);
                        var result2 = await _mediator.Send(updateWICommandOld);
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateWIImageCommand() { Id = id, Image = image });
                }
                var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var wiViewModel = _mapper.Map<WIViewModel>(response.Data);
                    //return View(sopViewModel);
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
                var html = await _viewRenderer.RenderViewToStringAsync("Create", wi);
                //return new JsonResult(new { isValid = false, html = html });
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
                var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var wiViewModel = _mapper.Map<WIViewModel>(response.Data);
                    wiViewModel.IsActive = false;
                    var updateWICommand = _mapper.Map<UpdateWICommand>(wiViewModel);
                    var result = await _mediator.Send(updateWICommand);
                    if (result.Succeeded)
                        _notify.Information($"WI with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllWIsCachedQuery());
                    if (response2.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<WIViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId) && users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId)).ToList();
                        }
                        foreach (WIViewModel item in viewModel)
                        {
                            var psStatat = _context.WIStatus.Where(a => a.WIId == item.Id).ToList();

                            if (psStatat.Count != 0)
                            {
                                var StatusId = _context.WIStatus.Where(a => a.WIId == item.Id).OrderBy(a => a.CreatedOn)
                                    .Include(a => a.DocumentStatus)
                                    .Last();
                                item.WIStatusView = StatusId.DocumentStatus.Name;
                            }
                            else
                            {
                                item.WIStatusView = "New";
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
        //    var deleteCommand = await _mediator.Send(new DeleteWICommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"WI with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllWIsCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);
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

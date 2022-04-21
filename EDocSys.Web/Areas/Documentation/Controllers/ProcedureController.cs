using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Queries.GetAllCached;
using EDocSys.Application.Features.Procedures.Commands.Create;
using EDocSys.Application.Features.Procedures.Commands.Delete;
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

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class ProcedureController : BaseController<ProcedureController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProcedureController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var model = new ProcedureViewModel();
            return View(model);
        }

        //public IActionResult Preview1()
        //{
        //    var model = new ProcedureViewModel();
        //    return View(model);
        //}

        [Authorize(Policy = "CanViewProcedure")]
        public async Task<IActionResult> Preview(int id)
        {
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var statusById = _context.ProcedureStatus.Where(a => a.ProcedureId == id).ToList();

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);

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
                }

                if (procedureViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == procedureViewModel.Concurred2).SingleOrDefault();
                    procedureViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                    procedureViewModel.Concurred2 = concurred2User.Id;
                    procedureViewModel.PositionC2 = concurred2User.Position;
                }

                if (procedureViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == procedureViewModel.ApprovedBy).SingleOrDefault();
                    procedureViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    procedureViewModel.PositionApp = ApprovedByUser.Position;
                }

                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                var currentyUserId = currentUser.Id;
                var concurred1 = response.Data.Concurred1;
                var concurred2 = response.Data.Concurred2;
                var app = response.Data.ApprovedBy;

                if (currentyUserId == concurred1) 
                {
                    ViewBag.IsConcurred1 = true;
                }

                if (currentyUserId == concurred2)
                {
                    ViewBag.IsConcurred2 = true;
                }

                if (currentyUserId == app)
                {
                    ViewBag.IsApp = true;
                }



                return View(procedureViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditProcedure")]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var companiesResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());

            if (id == 0)
            {
                var procedureViewModel = new ProcedureViewModel();
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        departmentViewModel = departmentViewModel.Where(a => a.Id == user.UserDepartmentId).ToList();
                    }

                    procedureViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }

                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => a.Id == user.UserCompanyId).ToList();
                    }

                    procedureViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }


                // Concurred 1
                var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                var userViewModelC1 = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();
                procedureViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                var userViewModelC2 = (from a1 in responseC1
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                procedureViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
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
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        procedureViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        procedureViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }

                    // Concurred 1
                    var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                    var userViewModelC1 = (from a1 in responseC1
                                         join a2 in _userManager.Users on a1.UserId equals a2.Id
                                         select new UserApproverViewModel
                                         {
                                             UserConcurred1Id = a1.UserId,
                                             FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                         }).OrderBy(a => a.Email).ToList();
                    procedureViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    procedureViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
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
            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var statusById = _context.ProcedureStatus.Where(a => a.ProcedureId == id).ToList();

            if (response.Succeeded)
            {
                var procedureViewModel = _mapper.Map<ProcedureViewModel>(response.Data);

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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var response = await _mediator.Send(new GetAllProceduresCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<ProcedureViewModel>>(response.Data);


                // Access Categiry = D  
                // SOP Department Admin (Full Access by Department)
                if (rolesList.Contains("D"))
                {
                    viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
                }

                // Access Categiry = E  
                // QMR / Lead Auditor / SOP Company Admin (Full Access by Company)
                

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
                if (rolesList.Contains("E") || rolesList.Contains("B1"))
                {
                    viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.ProcedureStatusView == "Approved").ToList();
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
                return View(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var deleteCommand = await _mediator.Send(new DeleteProcedureCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Procedure with Id {id} Deleted.");
                var response = await _mediator.Send(new GetAllProceduresCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<ProcedureViewModel>>(response.Data);

                    if (rolesList.Contains("D"))
                    {
                        viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
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

                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
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
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();

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
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var response = await _mediator.Send(new GetProcedureByIdQuery() { Id = id });

            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();

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

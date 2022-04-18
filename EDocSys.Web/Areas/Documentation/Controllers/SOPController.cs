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

        public IActionResult Index(string wscpno)
        {
            var model = new SOPViewModel();

            ViewBag.WSCPNo = wscpno;

            return View(model);
        }

        public async Task<IActionResult> Preview(int id)
        {
            var response = await _mediator.Send(new GetSOPByIdQuery() { Id = id });

            var statusById = _context.SOPStatus.Where(a => a.SOPId == id).ToList();

            if (response.Succeeded)
            {
                var sopViewModel = _mapper.Map<SOPViewModel>(response.Data);

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
                }

                if (sopViewModel.Concurred2 != null)
                {
                    var concurred2User = _userManager.Users.Where(a => a.Id == sopViewModel.Concurred2).SingleOrDefault();
                    sopViewModel.Concurred2Name = concurred2User.FirstName + " " + concurred2User.LastName;
                    sopViewModel.Concurred2 = concurred2User.Id;
                    sopViewModel.PositionC2 = concurred2User.Position;
                }

                if (sopViewModel.ApprovedBy != null)
                {
                    var ApprovedByUser = _userManager.Users.Where(a => a.Id == sopViewModel.ApprovedBy).SingleOrDefault();
                    sopViewModel.ApprovedBy = ApprovedByUser.FirstName + " " + ApprovedByUser.LastName;
                    sopViewModel.PositionApp = ApprovedByUser.Position;
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



                return View(sopViewModel);
            }
            return null;
        }

        public async Task<IActionResult> CreateOrEdit(int id = 0, string wscpno = "", int departmentId = 0, int procedureId = 0)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var companiesResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());
            

            if (id == 0)
            {
                var selectedDepartmentId = await _mediator.Send(new GetDepartmentByIdQuery() { Id = departmentId });

                var sopViewModel = new SOPViewModel();
                if (departmentsResponse.Succeeded)
                {
                    sopViewModel.DepartmentId = selectedDepartmentId.Data.Id;
                    sopViewModel.ProcessName = selectedDepartmentId.Data.Name;

                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    sopViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);

                    sopViewModel.ProcedureId = procedureId;
                    sopViewModel.WSCPNo = wscpno;
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
                        companyViewModel = companyViewModel.Where(a => a.Id == user.UserCompanyId).ToList();
                    }

                    sopViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
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
                sopViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                var userViewModelC2 = (from a1 in responseC1
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                sopViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
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
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        sopViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        sopViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
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
                    sopViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    sopViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
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

        //public async Task<IActionResult> LoadAll()
        //{
        //    var response = await _mediator.Send(new GetAllSOPsCachedQuery());

        //    if (response.Succeeded)
        //    {
        //        var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

        //        return PartialView("_ViewAll", viewModel);
        //    }
        //    return null;
        //}

        public async Task<IActionResult> LoadAll()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var response = await _mediator.Send(new GetAllSOPsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

                // Access Categiry = D  
                // SOP Department Admin (Full Access by Department)
                if (rolesList.Contains("D"))
                {
                    viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
                }

                // Access Categiry = E  
                // QMR / Lead Auditor / SOP Company Admin (Full Access by Company)


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
                if (rolesList.Contains("E") || rolesList.Contains("B1"))
                {
                    viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.SOPStatusView == "Approved").ToList();
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadByWSCP(string wscpno)
        {
            var response = await _mediator.Send(new GetAllSOPsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

                var viewModelbyWSCPNo = viewModel.Where(a => a.WSCPNo == wscpno).ToList();

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
        public async Task<JsonResult> OnPostDelete(int id)
        {
            var deleteCommand = await _mediator.Send(new DeleteSOPCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"SOP with Id {id} Deleted.");
                var response = await _mediator.Send(new GetAllSOPsCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);
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

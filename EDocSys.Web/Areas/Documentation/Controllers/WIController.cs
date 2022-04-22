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

        public IActionResult Index(string sopno)
        {
            var model = new WIViewModel();

            ViewBag.SOPNo = sopno;

            return View(model);
        }


        [Authorize(Policy = "CanViewWI")]
        public async Task<IActionResult> Preview(int id)
        {
            var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var wiViewModel = _mapper.Map<WIViewModel>(response.Data);
                return View(wiViewModel);
            }
            return null;
        }
        [Authorize(Policy = "CanCreateEditWI")]
        public async Task<IActionResult> CreateOrEdit(int id = 0, string wscpno = "", string sopno = "", int departmentId = 0, int procedureId = 0)
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

                var wiViewModel = new WIViewModel();
                if (departmentsResponse.Succeeded)
                {
                    wiViewModel.DepartmentId = selectedDepartmentId.Data.Id;
                    wiViewModel.ProcessName = selectedDepartmentId.Data.Name;

                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    wiViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);

                    wiViewModel.ProcedureId = procedureId;
                    wiViewModel.WSCPNo = wscpno;
                    wiViewModel.SOPNo = sopno;


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
                        companyViewModel = companyViewModel.Where(a => a.Id == user.UserCompanyId).ToList();
                    }

                    wiViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
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
                wiViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                // Concurred 2
                var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                var userViewModelC2 = (from a1 in responseC1
                                       join a2 in _userManager.Users on a1.UserId equals a2.Id
                                       select new UserApproverViewModel
                                       {
                                           UserConcurred2Id = a1.UserId,
                                           FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                       }).OrderBy(a => a.Email).ToList();
                wiViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                // Concurred APP
                var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
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
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                        wiViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }

                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        wiViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
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
                    wiViewModel.UserListC1 = new SelectList(userViewModelC1, "UserConcurred1Id", "FullName");

                    // Concurred 2
                    var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
                    var userViewModelC2 = (from a1 in responseC2
                                           join a2 in _userManager.Users on a1.UserId equals a2.Id
                                           select new UserApproverViewModel
                                           {
                                               UserConcurred2Id = a1.UserId,
                                               FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                           }).OrderBy(a => a.Email).ToList();
                    wiViewModel.UserListC2 = new SelectList(userViewModelC2, "UserConcurred2Id", "FullName");

                    // Concurred APP
                    var responseAPP = _context.UserApprovers.Where(a => a.ApprovalType == "APP" && (a.DepartmentId == user.UserDepartmentId || a.DepartmentId == 4)).ToList();
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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(currentUser.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            var response = await _mediator.Send(new GetAllWIsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);

                // Access Category = D  
                // SOP Department Admin (Full Access by Department)
                if (rolesList.Contains("D"))
                {
                    viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
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
                if (rolesList.Contains("E") || rolesList.Contains("B1"))
                {
                    viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.WIStatusView == "Approved").ToList();
                }

                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> LoadBySOP(string sopno)
        {
            var response = await _mediator.Send(new GetAllWIsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);

                var viewModelbySOPNo = viewModel.Where(a => a.SOPNo == sopno).ToList();

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
        public async Task<JsonResult> OnPostDelete(int id)
        {
            var deleteCommand = await _mediator.Send(new DeleteWICommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"WI with Id {id} Deleted.");
                var response = await _mediator.Send(new GetAllWIsCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);
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

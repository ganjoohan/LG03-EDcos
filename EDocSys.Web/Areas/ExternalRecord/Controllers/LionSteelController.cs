using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetAllCached;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Commands.Create;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Commands.Delete;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Commands.Update;
using EDocSys.Application.Features.ExternalFeatures.LionSteels.Queries.GetById;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.ExternalRecord.Models;
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
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using EDocSys.Web.Areas.Documentation.Models;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Commands.Create;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetById;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Commands.Update;
using EDocSys.Application.Features.ExternalFeatures.Attachments.Queries.GetByDocId;
using Hangfire;
using Hangfire.Common;
using EDocSys.Application.DTOs.Mail;
using EDocSys.Application.Interfaces.Shared;
using System.Text.Encodings.Web;
using Hangfire.Storage;

namespace EDocSys.Web.Areas.ExternalRecord.Controllers
{
    [Area("ExternalRecord")]
    public class LionSteelController : BaseController<LionSteelController>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMailService _mailService;

        public LionSteelController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env,
            IWebHostEnvironment hostEnvironment,
            IMailService mailService)
        {
            _mailService = mailService;
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
            var model = new LionSteelViewModel();           
            return View(model);
        }


        [Authorize(Policy = "CanViewExternalLionSteel")]
        public async Task<IActionResult> Preview(int id, bool print = false, LionSteelViewModel lionSteel = null)
        {
            ViewBag.RoleA = false;
            ViewBag.RoleE = false;
            ViewBag.RoleD = false;
            ViewBag.RoleSA = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            List<string> rolesListComp = new List<string>();

            var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);
                if (print || lionSteel.PrintCount != 0)
                {
                    if (lionSteel.PrintCount != lionSteelViewModel.PrintCount)
                        lionSteelViewModel.PrintCount = lionSteel.PrintCount;
                    else
                        lionSteelViewModel.PrintCount = lionSteelViewModel.PrintCount + 1;
                    var updateLionSteellCommand = _mapper.Map<UpdateLionSteelCommand>(lionSteelViewModel);
                    var result = await _mediator.Send(updateLionSteellCommand);
                    return RedirectToAction("Preview", new { Id = id });
                }
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == lionSteelViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                    }
                }
                if (rolesList.Contains("A"))
                {
                    ViewBag.RoleA = true;
                }
                if (rolesListComp.Contains("E"))
                {
                    ViewBag.RoleE = true;
                }
                if (rolesListComp.Contains("D"))
                {
                    ViewBag.RoleD = true;
                }
                if (rolesList.Contains("SuperAdmin"))
                {
                    ViewBag.RoleSA = true;
                }
                var response2 = _mediator.Send(new GetAttachmentByDocIdQuery() { DocId = id });
                List<AttachmentViewModel> attachmentViewModel = _mapper.Map<List<GetAttachmentByDocIdResponse>, List<AttachmentViewModel>>(response2.Result.Data);
                lionSteelViewModel.MyAttachments = attachmentViewModel.Where(w => w.IsActive == true && w.DocName == "LionSteel").ToList();

                return View(lionSteelViewModel);
            }
            return null;
        }

        [Authorize(Policy = "CanCreateEditExternalLionSteel")]
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
            var allDeptId = departmentViewModel.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            if (id == 0)
            {
                var lionSteelViewModel = new LionSteelViewModel();
                if (departmentsResponse.Succeeded)
                {
                    if (rolesList.Contains("D"))
                    {
                        departmentViewModel = departmentViewModel.Where(a => users.Select(s => s.UserDepartmentId).Contains(a.Id)).ToList();
                    }

                    lionSteelViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                }
                if (companiesResponse.Succeeded)
                {
                    var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                    if (rolesList.Contains("D"))
                    {
                        companyViewModel = companyViewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.Id)).ToList();
                    }

                    lionSteelViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                }
                lionSteelViewModel.MyAttachments = new List<AttachmentViewModel>();


                ViewBag.CreateEditFlag = "Create";
                string serverMapPath = Path.Combine(_env.WebRootPath, "html_template", "DOC_Template.html");
                string text = System.IO.File.ReadAllText(serverMapPath);

                HtmlString s = new HtmlString(text);
                ViewData["DetailTemplate"] = s;
                return View(lionSteelViewModel);
            }
            else
            {
                var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);
                    var lionSteelViewModelOld = _mapper.Map<LionSteelViewModel>(response.Data);
                    if (rev)
                    {
                        lionSteelViewModel.Id = 0;
                        var revNo = lionSteelViewModel.RevisionNo != null ? lionSteelViewModel.RevisionNo : 0;
                        lionSteelViewModel.RevisionNo = revNo + 1;
                        lionSteelViewModel.RevisionDate = DateTime.Now;
                        lionSteelViewModel.ArchiveId = lionSteelViewModelOld.Id;
                        lionSteelViewModel.PrintCount = 0;
                    }
                    if (departmentsResponse.Succeeded)
                    {
                        lionSteelViewModel.Departments = new SelectList(departmentViewModel, nameof(DepartmentViewModel.Id), nameof(DepartmentViewModel.Name), null, null);
                    }
                    if (companiesResponse.Succeeded)
                    {
                        var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                        lionSteelViewModel.Companies = new SelectList(companyViewModel, nameof(CompanyViewModel.Id), nameof(CompanyViewModel.Name), null, null);
                    }
                    var response2 = _mediator.Send(new GetAttachmentByDocIdQuery() { DocId = id });
                    List<AttachmentViewModel> attachmentViewModel = _mapper.Map<List<GetAttachmentByDocIdResponse>, List<AttachmentViewModel>>(response2.Result.Data);
                    lionSteelViewModel.MyAttachments = attachmentViewModel.Where(w => w.IsActive == true && w.DocName == "LionSteel").ToList();

                    return View(lionSteelViewModel);
                }
                return null;
            }
        }

        public async Task<IActionResult> LoadAll1(int id)
        {
            ViewBag.RoleA = false;
            ViewBag.RoleE = false;
            ViewBag.RoleD = false;
            ViewBag.RoleSA = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            List<string> rolesListComp = new List<string>();

            var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });

            //var statusById = _context.LionSteelStatus.Where(a => a.LionSteelId == id).ToList();

            if (response.Succeeded)
            {
                var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    rolesList.AddRange(roles);
                    if (user.UserCompanyId == lionSteelViewModel.CompanyId)
                    {
                        rolesListComp.AddRange(roles);
                    }
                }
                if (rolesList.Contains("A"))
                {
                    ViewBag.RoleA = true;
                }
                if (rolesListComp.Contains("E"))
                {
                    ViewBag.RoleE = true;
                }
                if (rolesListComp.Contains("D"))
                {
                    ViewBag.RoleD = true;
                }
                if (rolesList.Contains("SuperAdmin"))
                {
                    ViewBag.RoleSA = true;
                }
                return PartialView("_ViewLionSteelById", lionSteelViewModel);
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

            var response = await _mediator.Send(new GetAllLionSteelsCachedQuery());

            if (response.Succeeded)
            {
                var companiesResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());
                var departmentsResponse = await _mediator.Send(new GetAllDepartmentsCachedQuery());
                var companyViewModel = _mapper.Map<List<CompanyViewModel>>(companiesResponse.Data);
                var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                var viewModel = _mapper.Map<List<LionSteelViewModel>>(response.Data);
                viewModel = viewModel.Where(a => a.IsActive == true && (listComp.Contains(0) ? true : listComp.Contains(a.CompanyId))).ToList();
                foreach (LionSteelViewModel item in viewModel)
                {
                    item.CompanyName = companyViewModel.Where(w => w.Id == item.CompanyId).Select(s => s.Name).FirstOrDefault();
                    item.ProcessName = departmentViewModel.Where(w => w.Id == item.DepartmentId).Select(s => s.Name).FirstOrDefault();
                }
                if (viewModel.Count == 1)
                {
                    RecurringJob.RemoveIfExists("expiredExternalLS");
                    var manager = new RecurringJobManager();
                    manager.AddOrUpdate("expiredExternalLS", Job.FromExpression(() => sendMailAsync()), Cron.Daily());
                    var viewModel0 = viewModel.Where(w => w.ExpiryDate <= DateTime.Now && w.IsActive == true && w.IsArchive == false).ToList();
                    if (viewModel0.Count > 0)
                    {
                        RecurringJob.TriggerJob("expiredExternalLS");
                        BackgroundJob.Enqueue(() => sendMailAsync());
                    }
                }
                else
                {
                    List<Hangfire.Storage.RecurringJobDto> recurringJobs = new List<Hangfire.Storage.RecurringJobDto>();
                    recurringJobs = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs().ToList();
                    var newRJ = recurringJobs.Where(w => w.NextExecution.Value.ToString("dd/MM/yyyy") == DateTime.Now.AddDays(1).ToString("dd/MM/yyyy")).FirstOrDefault();
                    if (newRJ == null)
                    {
                        var viewModel0 = viewModel.Where(w => w.ExpiryDate <= DateTime.Now && w.IsActive == true && w.IsArchive == false).ToList();
                        if (viewModel0.Count > 0)
                        {
                            RecurringJob.RemoveIfExists("expiredExternalLS");
                            var manager = new RecurringJobManager();
                            manager.AddOrUpdate("expiredExternalLS", Job.FromExpression(() => sendMailAsync()), Cron.Daily());
                            RecurringJob.TriggerJob("expiredExternalLS");
                            BackgroundJob.Enqueue(() => sendMailAsync());
                        }
                    }
                    else
                    {
                        var viewModel0 = viewModel.Where(w => w.ExpiryDate <= DateTime.Now && w.IsActive == true && w.IsArchive == false).ToList();
                        if (viewModel0.Count > 0)
                        {
                            //RecurringJob.RemoveIfExists("expiredExternalLS");
                            //var manager = new RecurringJobManager();
                            //manager.AddOrUpdate("expiredExternalLS", Job.FromExpression(() => sendMailAsync()), Cron.Daily());
                            //BackgroundJob.Enqueue(() => sendMailAsync());
                        }
                    }
                }
                if (!rolesList.Contains("A") && !rolesList.Contains("SuperAdmin"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }


                if (rolesList.Contains("E") || rolesList.Contains("B1") || rolesList.Contains("B2"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                }
                else if (rolesList.Contains("C"))
                {
                    viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
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
                var lionSteelViewModel = new LionSteelViewModel();
                //lionSteelViewModel.Category = "Documentation";
                if (departmentsResponse.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", lionSteelViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);
                    if (departmentsResponse.Succeeded)
                    {
                        var departmentViewModel = _mapper.Map<List<DepartmentViewModel>>(departmentsResponse.Data);
                    }
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", lionSteelViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(int id, LionSteelViewModel lionSteel)
        {
            // lionSteel.EffectiveDate.Value.ToString("g");



            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createLionSteelCommand = _mapper.Map<CreateLionSteelCommand>(lionSteel);
                    var result = await _mediator.Send(createLionSteelCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"LionSteel with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateLionSteelCommand = _mapper.Map<UpdateLionSteelCommand>(lionSteel);
                    var result = await _mediator.Send(updateLionSteelCommand);
                    if (result.Succeeded) _notify.Information($"LionSteel with ID {result.Data} Updated.");
                }
                if (lionSteel.ArchiveId != 0)
                {
                    var response2 = await _mediator.Send(new GetLionSteelByIdQuery() { Id = lionSteel.ArchiveId });
                    if (response2.Succeeded)
                    {
                        var lionSteelViewModelOld = _mapper.Map<LionSteelViewModel>(response2.Data);
                        lionSteelViewModelOld.IsArchive = true;
                        var archiveDt = DateTime.Now.AddDays(30);
                        lionSteelViewModelOld.ArchiveDate = archiveDt;
                        var updateLionSteelCommand = _mapper.Map<UpdateLionSteelCommand>(lionSteelViewModelOld);
                        var result2 = await _mediator.Send(updateLionSteelCommand);
                    }
                }
                if (lionSteel.MyFiles != null ? lionSteel.MyFiles.Count > 0 : false)
                {
                    string prefixFn = "EDOCS" + "_LionSteel" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "_";
                    string filePath = "C:\\iis\\sites\\edocs\\file\\ExternalRecord\\Uploads\\LionSteel";
                    foreach (var myFile in lionSteel.MyFiles)
                    {
                        //Save file details into table "Attachment"
                        string fileName = Path.GetFileName(myFile.FileName);
                        string fileNameBatch = prefixFn + fileName;
                        AttachmentViewModel attachment = new AttachmentViewModel();
                        attachment.FileName = fileName;
                        attachment.FileNameBatch = fileNameBatch;
                        attachment.FileSize = Convert.ToInt32(myFile.Length);
                        attachment.FileType = myFile.ContentType;
                        attachment.FileLoc = filePath + "\\" + fileNameBatch;
                        attachment.DocId = id;
                        attachment.DocName = "LionSteel";
                        attachment.IsActive = true;
                        var createAttachmentCommand = _mapper.Map<CreateAttachmentCommand>(attachment);
                        var result = await _mediator.Send(createAttachmentCommand);
                        if (result.Succeeded)
                        {
                            //Save file into server C://EDOCS/LGPG/Uploads
                            OnPostUpload(fileNameBatch, myFile);
                        }
                    }
                }
                if (lionSteel.MyAttachments != null ? lionSteel.MyAttachments.Count > 0 : false)
                {
                    var tobedel = lionSteel.MyAttachments.Where(w => w.Deleted == true).ToList();
                    if (tobedel != null ? tobedel.Count > 0 : false)
                    {
                        foreach (var tbdel in tobedel)
                        {
                            var response3 = _mediator.Send(new GetAttachmentByIdQuery() { Id = tbdel.Id });
                            AttachmentViewModel file = _mapper.Map<AttachmentViewModel>(response3.Result.Data);
                            file.IsActive = false;
                            var updateAttachmentCommand = _mapper.Map<UpdateAttachmentCommand>(file);
                            var result = await _mediator.Send(updateAttachmentCommand);
                        }
                    }
                }
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    var image = file.OptimizeImageSize(700, 700);
                    await _mediator.Send(new UpdateLionSteelImageCommand() { Id = id, Image = image });
                }

                var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("Create", lionSteel);
                return View(nameof(IndexAsync));
            }
        }

        public void OnPostUpload(string fileNameBatch, IFormFile myFile)
        {
            //string wwwPath = this.webHostEnvironment.WebRootPath;
            //string contentPath = this.webHostEnvironment.ContentRootPath;

            string path = Path.Combine("C:\\iis\\sites\\edocs\\file\\ExternalRecord\\Uploads", "LionSteel");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();
            //string fileName = Path.GetFileName(myFile.FileName);
            using (FileStream stream = new FileStream(Path.Combine(path, fileNameBatch), FileMode.Create))
            {
                myFile.CopyTo(stream);
                //uploadedFiles.Add(fileName);
                //this.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
            }
        }
        public FileResult OnGetDownloadFile(int fileId)
        {
            var response = _mediator.Send(new GetAttachmentByIdQuery() { Id = fileId });
            AttachmentViewModel file = _mapper.Map<AttachmentViewModel>(response.Result.Data);
            //Build the File Path.
            string path = file.FileLoc.Contains("//") ? file.FileLoc : file.FileLoc.Replace("/", "//");
            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            //Send the File to Download.
            return File(bytes, "application/octet-stream", file.FileName);
        }
        public async void OnDeleteFile(int fileId)
        {
            var response = _mediator.Send(new GetAttachmentByIdQuery() { Id = fileId });
            AttachmentViewModel file = _mapper.Map<AttachmentViewModel>(response.Result.Data);
            file.IsActive = false;
            var updateAttachmentCommand = _mapper.Map<UpdateAttachmentCommand>(file);
            var result = await _mediator.Send(updateAttachmentCommand);
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
                var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);
                    lionSteelViewModel.IsActive = false;
                    var updateLionSteelCommand = _mapper.Map<UpdateLionSteelCommand>(lionSteelViewModel);
                    var result = await _mediator.Send(updateLionSteelCommand);
                    if (result.Succeeded)
                        _notify.Information($"LionSteel with ID {result.Data} Deleted.");
                    var response2 = await _mediator.Send(new GetAllLionSteelsCachedQuery());
                    if (response2.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<LionSteelViewModel>>(response2.Data);
                        viewModel = viewModel.Where(a => a.IsActive == true).ToList();
                        if (rolesList.Contains("D"))
                        {
                            viewModel = viewModel.Where(a => users.Select(s => s.UserCompanyId).Contains(a.CompanyId)).ToList();
                        }
                        //foreach (LionSteelViewModel item in viewModel)
                        //{
                        //    var psStatat = _context.LionSteelStatus.Where(a => a.LionSteelId == item.Id).ToList();

                        //    if (psStatat.Count != 0)
                        //    {
                        //        var StatusId = _context.LionSteelStatus.Where(a => a.LionSteelId == item.Id).OrderBy(a => a.CreatedOn)
                        //            .Include(a => a.DocumentStatus)
                        //            .Last();
                        //        item.LionSteelStatusView = StatusId.DocumentStatus.Name;
                        //    }
                        //    else
                        //    {
                        //        item.LionSteelStatusView = "New";
                        //    }
                        //}
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

        //    var deleteCommand = await _mediator.Send(new DeleteLionSteelCommand { Id = id });
        //    if (deleteCommand.Succeeded)
        //    {
        //        _notify.Information($"Document Manual with Id {id} Deleted.");
        //        var response = await _mediator.Send(new GetAllLionSteelsCachedQuery());
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<LionSteelViewModel>>(response.Data);

        //            if (rolesList.Contains("D"))
        //            {
        //                viewModel = viewModel.Where(a => a.CompanyId == user.UserCompanyId && a.DepartmentId == user.UserDepartmentId).ToList();
        //            }

        //            foreach (LionSteelViewModel item in viewModel)
        //            {
        //                var psStatat = _context.LionSteelStatus.Where(a => a.LionSteelId == item.Id).ToList();

        //                if (psStatat.Count != 0)
        //                {
        //                    var StatusId = _context.LionSteelStatus.Where(a => a.LionSteelId == item.Id).OrderBy(a => a.CreatedOn)
        //                        .Include(a => a.DocumentStatus)
        //                        .Last();
        //                    item.LionSteelStatusView = StatusId.DocumentStatus.Name;
        //                }
        //                else
        //                {
        //                    item.LionSteelStatusView = "New";
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
            var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });
            if (response.Succeeded)
            {
                var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Assignment", lionSteelViewModel) });
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
            var departmentsResponseC1 = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModelC1 = _mapper.Map<List<DepartmentViewModel>>(departmentsResponseC1.Data);
            int allCompId = 0;
            var allDeptId = departmentViewModelC1.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });

            var responseC1 = _context.UserApprovers.Where(a => a.ApprovalType == "C1" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);

                var userViewModel = (from a1 in responseC1
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred1Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                //lionSteelViewModel.UserList = new SelectList(userViewModel, "UserConcurred1Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred1", lionSteelViewModel) });
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
            var departmentsResponseC2 = await _mediator.Send(new GetAllDepartmentsCachedQuery());
            var departmentViewModelC2 = _mapper.Map<List<DepartmentViewModel>>(departmentsResponseC2.Data);
            int allCompId = 0;
            var allDeptId = departmentViewModelC2.Where(w => w.Name == "All Departments").FirstOrDefault().Id;
            var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });

            var responseC2 = _context.UserApprovers.Where(a => a.ApprovalType == "C2" && (users.Select(s => s.UserCompanyId).Contains(a.CompanyId) || a.CompanyId == allCompId) && (users.Select(s => s.UserDepartmentId).Contains(a.DepartmentId) || a.DepartmentId == allDeptId)).ToList();

            if (response.Succeeded)
            {
                var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);

                var userViewModel = (from a1 in responseC2
                                     join a2 in _userManager.Users on a1.UserId equals a2.Id
                                     select new UserApproverViewModel
                                     {
                                         UserConcurred2Id = a1.UserId,
                                         FullName = a2.LastName + " " + a2.FirstName + " (" + a2.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();

                //lionSteelViewModel.UserList = new SelectList(userViewModel, "UserConcurred2Id", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignConcurred2", lionSteelViewModel) });
            }
            return null;
        }

        public async Task<JsonResult> OnGetAssignApprovedBy(int id)
        {
            var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });

            if (response.Succeeded)
            {
                var lionSteelViewModel = _mapper.Map<LionSteelViewModel>(response.Data);


                var userViewModel = (from a1 in await _userManager.Users.ToListAsync()
                                     select new UserViewModel
                                     {
                                         UserApproveBy = a1.Id,
                                         FullName = a1.LastName + " " + a1.FirstName + " (" + a1.Email + ")"
                                     }).OrderBy(a => a.Email).ToList();


                //lionSteelViewModel.UserList = new SelectList(userViewModel, "UserApproveBy", "FullName");

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignApprovedBy", lionSteelViewModel) });
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmitAssignment(int id, LionSteelViewModel lionSteel)
        {
            if (id == 0)
            {
                var createLionSteelCommand = _mapper.Map<CreateLionSteelCommand>(lionSteel);
                var result = await _mediator.Send(createLionSteelCommand);
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"LionSteel with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
            }

            else
            {
                //lionSteel.Concurred1 = lionSteel.Concurred1;
                var updateLionSteelCommand = _mapper.Map<UpdateLionSteelCommand>(lionSteel);
                var result = await _mediator.Send(updateLionSteelCommand);
                if (result.Succeeded) _notify.Information($"LionSteel Assignment with ID {result.Data} Updated.");
            }
            var response = await _mediator.Send(new GetAllLionSteelsCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<LionSteelViewModel>>(response.Data);
                var html2 = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                return new JsonResult(new { isValid = true, html = html2 });
            }
            else
            {
                _notify.Error(response.Message);
                return null;
            }
        }
        public async Task sendMailAsync()
        {
            var response = await _mediator.Send(new GetAllLionSteelsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<LionSteelViewModel>>(response.Data);
                var viewModel0 = viewModel.Where(w=> w.ExpiryDate <= DateTime.Now && w.IsActive == true && w.IsArchive == false).ToList();
                if (viewModel0.Count > 0)
                {
                    List<string> informedLists = viewModel0.SelectMany(s => s.InformedList.Split(",").ToList()).Distinct().ToList();
                    foreach (var informedList in informedLists)
                    {
                        var viewModel1 = viewModel.Where(w => w.InformedList.Contains(informedList)).ToList();
                        string s_body = $"<table><tr>Documents are expired: </tr>";
                        foreach (var vM1 in viewModel1)
                        {
                            s_body += $"<tr>" + vM1.FormNo + " by ";
                            s_body += $"<a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/externalrecord/lionsteel/preview?id=" + vM1.Id)}'>clicking here</a> to open the document.</tr>";
                        }
                        s_body += $"</table>";
                        MailRequest mail = new MailRequest()
                        {
                            //To = userModel.Email,
                            // To = "lgcompadmin@lion.com.my",
                            To = informedList,
                            Subject = "Lion Steel External Record Expired.",
                            // 

                            Body = s_body
                        };

                        try
                        {
                            await _mailService.SendAsync(mail);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
    }
}

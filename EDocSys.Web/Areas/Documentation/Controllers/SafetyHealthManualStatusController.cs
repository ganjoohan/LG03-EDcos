using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetAllCached;
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
using EDocSys.Application.Features.SafetyHealthManualStatuses.Commands.Create;
using EDocSys.Application.DTOs.Mail;
using System.Net.Mail;
using EDocSys.Application.Interfaces.Shared;
using System.Text.Encodings.Web;
using EDocSys.Application.Features.SafetyHealthManuals.Queries.GetById;
using Microsoft.AspNetCore.Identity;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Application.Features.SafetyHealthManuals.Commands.Update;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class SafetyHealthManualStatusController : BaseController<SafetyHealthManualStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string c1 { get; private set; }
        public string c2 { get; private set; }
        public string app { get; private set; }
        public string emailTo { get; private set; }
        

        public SafetyHealthManualStatusController(IMailService mailService, UserManager<ApplicationUser> userManager, IdentityContext identityContext, RoleManager<IdentityRole> roleManager)
        {
            _mailService = mailService;
            _userManager = userManager;
            _identityContext = identityContext;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = new SafetyHealthManualStatusViewModel();

            var response = await _mediator.Send(new GetAllSafetyHealthManualsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SafetyHealthManualViewModel>>(response.Data);

                var viewModelbyDOCNo = viewModel.Where(a => a.Id == Id).SingleOrDefault();

                ViewBag.DOCNo = viewModelbyDOCNo.DOCNo;
                ViewBag.SafetyHealthManualId = viewModelbyDOCNo.Id;
                return View(model);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll(int SafetyHealthManualId)
        {
            var response = await _mediator.Send(new GetAllSafetyHealthManualStatusCachedQuery());
            ViewBag.RoleE = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }
            if (rolesList.Contains("E"))
            {
                ViewBag.RoleE = true;
            }
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SafetyHealthManualStatusViewModel>>(response.Data);
                var viewModelbyDOCNo = viewModel.Where(a => a.SafetyHealthManualId == SafetyHealthManualId).ToList();

                return PartialView("_ViewAll", viewModelbyDOCNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0, int SafetyHealthManualId = 0)
        {
            var safetyHealthManualStatusResponse = await _mediator.Send(new GetAllSafetyHealthManualStatusCachedQuery());

            if (id == 0)
            {
                var safetyHealthManualstatusViewModel = new SafetyHealthManualStatusViewModel();
                safetyHealthManualstatusViewModel.DocumentStatusId = 7;
                safetyHealthManualstatusViewModel.SafetyHealthManualId = SafetyHealthManualId;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", safetyHealthManualstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        public async Task<JsonResult> OnGetSubmit(int id = 0, int SafetyHealthManualId = 0, int status = 0)
        {
            var safetyHealthManualStatusResponse = await _mediator.Send(new GetAllSafetyHealthManualStatusCachedQuery());

            if (id == 0)
            {
                var safetyHealthManualstatusViewModel = new SafetyHealthManualStatusViewModel();
                safetyHealthManualstatusViewModel.DocumentStatusId = status;
                safetyHealthManualstatusViewModel.SafetyHealthManualId = SafetyHealthManualId;

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", safetyHealthManualstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmit(int id, SafetyHealthManualStatusViewModel SafetyHealthManualStatus)
        {




            if (ModelState.IsValid)
            {
                var responseGetSafetyHealthManualById = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = SafetyHealthManualStatus.SafetyHealthManualId });
                var safetyHealthManualIdx = responseGetSafetyHealthManualById.Data.CompanyId;



                if (responseGetSafetyHealthManualById.Succeeded)
                {
                    c1 = responseGetSafetyHealthManualById.Data.Concurred1;
                    c2 = responseGetSafetyHealthManualById.Data.Concurred2;
                    app = responseGetSafetyHealthManualById.Data.ApprovedBy;
                    //emailTo = _userManager.Users.Where(a => a.Id == lTo).Select(a => a.Email).SingleOrDefault();
                }

                var createSafetyHealthManualStatusCommand = _mapper.Map<CreateSafetyHealthManualStatusCommand>(SafetyHealthManualStatus);
                    var result = await _mediator.Send(createSafetyHealthManualStatusCommand);

                if (SafetyHealthManualStatus.DocumentStatusId == 5) // REJECTED: send email to company admin
                {
                    // locate company admin email and send to [TO] sender


                    var allUsersByCompany = _userManager.Users.Where(a => a.UserCompanyId == responseGetSafetyHealthManualById.Data.CompanyId).ToList();

                    var companyAdmin = (from a1 in allUsersByCompany
                                        join a2 in _identityContext.UserRoles on a1.Id equals a2.UserId
                                        join a3 in _roleManager.Roles on a2.RoleId equals a3.Id
                                        select new UserViewModel
                                        {
                                            Email = a1.Email,
                                            RoleName = a3.Name
                                        }).ToList();
                    string companyAdminEmail = companyAdmin.Where(a => a.RoleName == "E").Select(a => a.Email).FirstOrDefault();




                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        // To = "lgcompadmin@lion.com.my",
                        To = companyAdminEmail,
                        Subject = "Safety and Health Manual " + responseGetSafetyHealthManualById.Data.DOCNo + " need rejected.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/SafetyHealthManual/preview?id=" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need rejected. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/SafetyHealthManual/preview?id=" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (SafetyHealthManualStatus.DocumentStatusId == 1) // Submitted: check C1, C2 and APP is available
                {
                    if (c1 != null)
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == c1).Select(a => a.Email).SingleOrDefault();
                    }
                    else if (c2 != null)
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == c2).Select(a => a.Email).SingleOrDefault();
                    }
                    else
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();
                    }

                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "Safety and Health Manual " + responseGetSafetyHealthManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/SafetyHealthManual/preview?id=" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/SafetyHealthManual/preview?id=" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."


                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }

                else if (SafetyHealthManualStatus.DocumentStatusId == 2) // CONCURRED 1: check C2 and APP is available
                {
                    if (c2 != null)
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == c2).Select(a => a.Email).SingleOrDefault();
                    }
                    else
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();
                    }

                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "Safety and Health Manual " + responseGetSafetyHealthManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/SafetyHealthManual/preview/" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/SafetyHealthManual/preview?id=" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (SafetyHealthManualStatus.DocumentStatusId == 3) // CONCURRED 2: check C2 and APP is available
                {

                    emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();


                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "Thank you for registering",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/SafetyHealthManual/preview/" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/SafetyHealthManual/preview?id=" + SafetyHealthManualStatus.SafetyHealthManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (SafetyHealthManualStatus.DocumentStatusId == 4) //APPROVED
                {
                    responseGetSafetyHealthManualById.Data.EffectiveDate = DateTime.Now;
                    var updateQualityManualCommand = _mapper.Map<UpdateSafetyHealthManualCommand>(responseGetSafetyHealthManualById.Data);
                    var result1 = await _mediator.Send(updateQualityManualCommand);
                }
                if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Safety and Health Manual with ID {result.Data} Submitted. ");
                    }
                    else _notify.Error(result.Message);
                //}

                //else
                //{

                //}
                try
                {
                    var response = await _mediator.Send(new GetAllSafetyHealthManualStatusCachedQuery());
                    if (response.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<SafetyHealthManualStatusViewModel>>(response.Data);
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        var viewModel = new List<SafetyHealthManualStatusViewModel>();
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                }
                catch
                {
                    var viewModel = new List<SafetyHealthManualStatusViewModel>();
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Submit", SafetyHealthManualStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, SafetyHealthManualStatusViewModel SafetyHealthManualStatus)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createSafetyHealthManualStatusCommand = _mapper.Map<CreateSafetyHealthManualStatusCommand>(SafetyHealthManualStatus);
                    var result = await _mediator.Send(createSafetyHealthManualStatusCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Safety and Health Manual with ID {result.Data} Submitted.");
                    }
                    else _notify.Error(result.Message);
                }

                else
                {
                    //var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
                    //var result = await _mediator.Send(updateBrandCommand);
                    //if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllSafetyHealthManualStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<SafetyHealthManualStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", SafetyHealthManualStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
    }
}

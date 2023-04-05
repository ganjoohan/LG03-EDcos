using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.QualityManuals.Queries.GetAllCached;
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
using EDocSys.Application.Features.QualityManualStatuses.Commands.Create;
using EDocSys.Application.DTOs.Mail;
using System.Net.Mail;
using EDocSys.Application.Interfaces.Shared;
using System.Text.Encodings.Web;
using EDocSys.Application.Features.QualityManuals.Queries.GetById;
using Microsoft.AspNetCore.Identity;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Application.Features.QualityManuals.Commands.Update;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class QualityManualStatusController : BaseController<QualityManualStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string c1 { get; private set; }
        public string c2 { get; private set; }
        public string app { get; private set; }
        public string emailTo { get; private set; }
        

        public QualityManualStatusController(IMailService mailService, UserManager<ApplicationUser> userManager, IdentityContext identityContext, RoleManager<IdentityRole> roleManager)
        {
            _mailService = mailService;
            _userManager = userManager;
            _identityContext = identityContext;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = new QualityManualStatusViewModel();

            var response = await _mediator.Send(new GetAllQualityManualsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<QualityManualViewModel>>(response.Data);

                var viewModelbyDOCNo = viewModel.Where(a => a.Id == Id).SingleOrDefault();

                ViewBag.DOCNo = viewModelbyDOCNo.DOCNo;
                ViewBag.QualityManualId = viewModelbyDOCNo.Id;
                return View(model);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll(int QualityManualId)
        {
            var response = await _mediator.Send(new GetAllQualityManualStatusCachedQuery());
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
                var viewModel = _mapper.Map<List<QualityManualStatusViewModel>>(response.Data);
                var viewModelbyDOCNo = viewModel.Where(a => a.QualityManualId == QualityManualId).ToList();

                return PartialView("_ViewAll", viewModelbyDOCNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0, int QualityManualId = 0)
        {
            var QualityManualStatusResponse = await _mediator.Send(new GetAllQualityManualStatusCachedQuery());

            if (id == 0)
            {
                var QualityManualstatusViewModel = new QualityManualStatusViewModel();
                QualityManualstatusViewModel.DocumentStatusId = 7;
                QualityManualstatusViewModel.QualityManualId = QualityManualId;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", QualityManualstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        public async Task<JsonResult> OnGetSubmit(int id = 0, int QualityManualId = 0, int status = 0)
        {
            var QualityManualStatusResponse = await _mediator.Send(new GetAllQualityManualStatusCachedQuery());

            if (id == 0)
            {
                var QualityManualstatusViewModel = new QualityManualStatusViewModel();
                QualityManualstatusViewModel.DocumentStatusId = status;
                QualityManualstatusViewModel.QualityManualId = QualityManualId;

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", QualityManualstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmit(int id, QualityManualStatusViewModel QualityManualStatus)
        {




            if (ModelState.IsValid)
            {
                var responseGetQualityManualById = await _mediator.Send(new GetQualityManualByIdQuery() { Id = QualityManualStatus.QualityManualId });
                var QualityManualIdx = responseGetQualityManualById.Data.CompanyId;



                if (responseGetQualityManualById.Succeeded)
                {
                    c1 = responseGetQualityManualById.Data.Concurred1;
                    c2 = responseGetQualityManualById.Data.Concurred2;
                    app = responseGetQualityManualById.Data.ApprovedBy;
                    //emailTo = _userManager.Users.Where(a => a.Id == lTo).Select(a => a.Email).SingleOrDefault();
                }

                var createQualityManualStatusCommand = _mapper.Map<CreateQualityManualStatusCommand>(QualityManualStatus);
                    var result = await _mediator.Send(createQualityManualStatusCommand);

                if (QualityManualStatus.DocumentStatusId == 5) // REJECTED: send email to company admin
                {
                    // locate company admin email and send to [TO] sender


                    var allUsersByCompany = _userManager.Users.Where(a => a.UserCompanyId == responseGetQualityManualById.Data.CompanyId && a.IsActive == true).ToList();

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
                        Subject = "Quality Manual " + responseGetQualityManualById.Data.DOCNo + " need rejected.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/QualityManual/preview?id=" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need rejected. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/QualityManual/preview?id=" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (QualityManualStatus.DocumentStatusId == 1) // Submitted: check C1, C2 and APP is available
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
                        Subject = "Quality Manual " + responseGetQualityManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/QualityManual/preview?id=" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/QualityManual/preview?id=" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."


                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }

                else if (QualityManualStatus.DocumentStatusId == 2) // CONCURRED 1: check C2 and APP is available
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
                        Subject = "Quality Manual " + responseGetQualityManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/QualityManual/preview/" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/QualityManual/preview?id=" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (QualityManualStatus.DocumentStatusId == 3) // CONCURRED 2: check C2 and APP is available
                {

                    emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();


                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "Quality Manual " + responseGetQualityManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/QualityManual/preview/" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/QualityManual/preview?id=" + QualityManualStatus.QualityManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (QualityManualStatus.DocumentStatusId == 4) //APPROVED
                {
                    var qualityManualViewModel = _mapper.Map<QualityManualViewModel>(responseGetQualityManualById.Data);
                    qualityManualViewModel.EffectiveDate = DateTime.Now;
                    var updateQualityManualCommand = _mapper.Map<UpdateQualityManualCommand>(qualityManualViewModel);
                    var result1 = await _mediator.Send(updateQualityManualCommand);
                    if (qualityManualViewModel.ArchiveId != 0)
                    {
                        var responseGetQualityManualByIdOld = await _mediator.Send(new GetQualityManualByIdQuery() { Id = qualityManualViewModel.ArchiveId });
                        var qualityManualViewModelOld = _mapper.Map<QualityManualViewModel>(responseGetQualityManualByIdOld.Data);
                        qualityManualViewModelOld.ArchiveDate = DateTime.Now;
                        var updateQualityManualCommandOld = _mapper.Map<UpdateQualityManualCommand>(qualityManualViewModelOld);
                        var result1Old = await _mediator.Send(updateQualityManualCommandOld);
                    }
                }
                if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Quality Manual with ID {result.Data} Submitted. ");
                    }
                    else _notify.Error(result.Message);
                //}

                //else
                //{

                //}
                try
                {
                    var response = await _mediator.Send(new GetAllQualityManualStatusCachedQuery());
                    if (response.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<QualityManualStatusViewModel>>(response.Data);
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        var viewModel = new List<QualityManualStatusViewModel>();
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                }
                catch
                {
                    var viewModel = new List<QualityManualStatusViewModel>();
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Submit", QualityManualStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, QualityManualStatusViewModel QualityManualStatus)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createQualityManualStatusCommand = _mapper.Map<CreateQualityManualStatusCommand>(QualityManualStatus);
                    var result = await _mediator.Send(createQualityManualStatusCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Quality Manual with ID {result.Data} Submitted.");
                    }
                    else _notify.Error(result.Message);
                }

                else
                {
                    //var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
                    //var result = await _mediator.Send(updateBrandCommand);
                    //if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllQualityManualStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<QualityManualStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", QualityManualStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
    }
}

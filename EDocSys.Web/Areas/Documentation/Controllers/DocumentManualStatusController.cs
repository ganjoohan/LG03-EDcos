using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.DocumentManualStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.DocumentManuals.Queries.GetAllCached;
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
using EDocSys.Application.Features.DocumentManualStatuses.Commands.Create;
using EDocSys.Application.DTOs.Mail;
using System.Net.Mail;
using EDocSys.Application.Interfaces.Shared;
using System.Text.Encodings.Web;
using EDocSys.Application.Features.DocumentManuals.Queries.GetById;
using Microsoft.AspNetCore.Identity;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Application.Features.DocumentManuals.Commands.Update;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class DocumentManualStatusController : BaseController<DocumentManualStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string c1 { get; private set; }
        public string c2 { get; private set; }
        public string app { get; private set; }
        public string emailTo { get; private set; }
        

        public DocumentManualStatusController(IMailService mailService, UserManager<ApplicationUser> userManager, IdentityContext identityContext, RoleManager<IdentityRole> roleManager)
        {
            _mailService = mailService;
            _userManager = userManager;
            _identityContext = identityContext;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = new DocumentManualStatusViewModel();

            var response = await _mediator.Send(new GetAllDocumentManualsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<DocumentManualViewModel>>(response.Data);

                var viewModelbyDOCNo = viewModel.Where(a => a.Id == Id).SingleOrDefault();

                ViewBag.DOCNo = viewModelbyDOCNo.DOCNo;
                ViewBag.DocumentManualId = viewModelbyDOCNo.Id;
                return View(model);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll(int documentManualId)
        {
            var response = await _mediator.Send(new GetAllDocumentManualStatusCachedQuery());
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
                var viewModel = _mapper.Map<List<DocumentManualStatusViewModel>>(response.Data);
                var viewModelbyDOCNo = viewModel.Where(a => a.DocumentManualId == documentManualId).ToList();

                return PartialView("_ViewAll", viewModelbyDOCNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0, int documentManualId = 0)
        {
            var documentManualStatusResponse = await _mediator.Send(new GetAllDocumentManualStatusCachedQuery());

            if (id == 0)
            {
                var documentmanualstatusViewModel = new DocumentManualStatusViewModel();
                documentmanualstatusViewModel.DocumentStatusId = 7;
                documentmanualstatusViewModel.DocumentManualId = documentManualId;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", documentmanualstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        public async Task<JsonResult> OnGetSubmit(int id = 0, int documentManualId = 0, int status = 0)
        {
            var documentManualStatusResponse = await _mediator.Send(new GetAllDocumentManualStatusCachedQuery());

            if (id == 0)
            {
                var documentmanualstatusViewModel = new DocumentManualStatusViewModel();
                documentmanualstatusViewModel.DocumentStatusId = status;
                documentmanualstatusViewModel.DocumentManualId = documentManualId;

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", documentmanualstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmit(int id, DocumentManualStatusViewModel documentManualStatus)
        {




            if (ModelState.IsValid)
            {
                var responseGetDocumentManualById = await _mediator.Send(new GetDocumentManualByIdQuery() { Id = documentManualStatus.DocumentManualId });
                var documentManualIdx = responseGetDocumentManualById.Data.CompanyId;



                if (responseGetDocumentManualById.Succeeded)
                {
                    c1 = responseGetDocumentManualById.Data.Concurred1;
                    c2 = responseGetDocumentManualById.Data.Concurred2;
                    app = responseGetDocumentManualById.Data.ApprovedBy;
                    //emailTo = _userManager.Users.Where(a => a.Id == lTo).Select(a => a.Email).SingleOrDefault();
                }

                var createDocumentManualStatusCommand = _mapper.Map<CreateDocumentManualStatusCommand>(documentManualStatus);
                    var result = await _mediator.Send(createDocumentManualStatusCommand);

                if (documentManualStatus.DocumentStatusId == 5) // REJECTED: send email to company admin
                {
                    // locate company admin email and send to [TO] sender


                    var allUsersByCompany = _userManager.Users.Where(a => a.UserCompanyId == responseGetDocumentManualById.Data.CompanyId).ToList();

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
                        Subject = "Document Manual " + responseGetDocumentManualById.Data.DOCNo + " need rejected.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/documentmanual/preview?id=" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need rejected. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/documentmanual/preview?id=" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (documentManualStatus.DocumentStatusId == 1) // Submitted: check C1, C2 and APP is available
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
                        Subject = "Document Manual " + responseGetDocumentManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/documentmanual/preview?id=" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/documentmanual/preview?id=" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."


                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }

                else if (documentManualStatus.DocumentStatusId == 2) // CONCURRED 1: check C2 and APP is available
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
                        Subject = "Document Manual " + responseGetDocumentManualById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/documentmanual/preview/" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/documentmanual/preview?id=" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (documentManualStatus.DocumentStatusId == 3) // CONCURRED 2: check C2 and APP is available
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
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/documentmanual/preview/" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/documentmanual/preview?id=" + documentManualStatus.DocumentManualId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (documentManualStatus.DocumentStatusId == 4) //APPROVED
                {
                    responseGetDocumentManualById.Data.EffectiveDate = DateTime.Now;
                    var updateDocManualCommand = _mapper.Map<UpdateDocumentManualCommand>(responseGetDocumentManualById.Data);
                    var result1 = await _mediator.Send(updateDocManualCommand);
                }
                if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Document Manual with ID {result.Data} Submitted. ");
                    }
                    else _notify.Error(result.Message);
                //}

                //else
                //{

                //}
                try
                {
                    var response = await _mediator.Send(new GetAllDocumentManualStatusCachedQuery());
                    if (response.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<DocumentManualStatusViewModel>>(response.Data);
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        var viewModel = new List<DocumentManualStatusViewModel>();
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                }
                catch
                {
                    var viewModel = new List<DocumentManualStatusViewModel>();
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Submit", documentManualStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, DocumentManualStatusViewModel documentManualStatus)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createDocumentManualStatusCommand = _mapper.Map<CreateDocumentManualStatusCommand>(documentManualStatus);
                    var result = await _mediator.Send(createDocumentManualStatusCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Document Manual with ID {result.Data} Submitted.");
                    }
                    else _notify.Error(result.Message);
                }

                else
                {
                    //var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
                    //var result = await _mediator.Send(updateBrandCommand);
                    //if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllDocumentManualStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<DocumentManualStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", documentManualStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
    }
}

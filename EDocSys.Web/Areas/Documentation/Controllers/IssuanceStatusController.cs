using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.IssuanceStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.Issuances.Queries.GetAllCached;
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
using EDocSys.Application.Features.IssuanceStatuses.Commands.Create;
using EDocSys.Application.DTOs.Mail;
using System.Net.Mail;
using EDocSys.Application.Interfaces.Shared;
using System.Text.Encodings.Web;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using Microsoft.AspNetCore.Identity;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Application.Features.Issuances.Commands.Update;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class IssuanceStatusController : BaseController<IssuanceStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string ver { get; private set; }       
        public string app { get; private set; }
        public string ack { get; private set; }
        public string emailTo { get; private set; }
        

        public IssuanceStatusController(IMailService mailService, UserManager<ApplicationUser> userManager, IdentityContext identityContext, RoleManager<IdentityRole> roleManager)
        {
            _mailService = mailService;
            _userManager = userManager;
            _identityContext = identityContext;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = new IssuanceStatusViewModel();

            var response = await _mediator.Send(new GetAllIssuancesCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<IssuanceViewModel>>(response.Data);

                var viewModelbyDOCNo = viewModel.Where(a => a.Id == Id).SingleOrDefault();

                ViewBag.DOCNo = viewModelbyDOCNo.DOCNo;
                ViewBag.IssuanceId = viewModelbyDOCNo.Id;
                return View(model);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll(int issuanceId)
        {
            var response = await _mediator.Send(new GetAllIssuanceStatusCachedQuery());
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
                var viewModel = _mapper.Map<List<IssuanceStatusViewModel>>(response.Data);
                var viewModelbyDOCNo = viewModel.Where(a => a.IssuanceId == issuanceId).ToList();

                return PartialView("_ViewAll", viewModelbyDOCNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0, int issuanceId = 0)
        {
            var issuanceStatusResponse = await _mediator.Send(new GetAllIssuanceStatusCachedQuery());

            if (id == 0)
            {
                var issuanceStatusViewModel = new IssuanceStatusViewModel();
                issuanceStatusViewModel.DocumentStatusId = 7;
                issuanceStatusViewModel.IssuanceId = issuanceId;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", issuanceStatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        public async Task<JsonResult> OnGetSubmit(int id = 0, int issuanceId = 0, int status = 0)
        {
            var issuanceStatusResponse = await _mediator.Send(new GetAllIssuanceStatusCachedQuery());

            if (id == 0)
            {
                var issuanceStatusViewModel = new IssuanceStatusViewModel();
                issuanceStatusViewModel.DocumentStatusId = status;
                issuanceStatusViewModel.IssuanceId = issuanceId;

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", issuanceStatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmit(int id, IssuanceStatusViewModel issuanceStatus)
        {




            if (ModelState.IsValid)
            {
                var responseGetIssuanceById = await _mediator.Send(new GetIssuanceByIdQuery() { Id = issuanceStatus.IssuanceId });
                var issuanceIdx = responseGetIssuanceById.Data.CompanyId;



                if (responseGetIssuanceById.Succeeded)
                {
                    ver = responseGetIssuanceById.Data.VerifiedBy;
                    app = responseGetIssuanceById.Data.ApprovedBy;
                    ack = responseGetIssuanceById.Data.AcknowledgedBy;
                    //emailTo = _userManager.Users.Where(a => a.Id == lTo).Select(a => a.Email).SingleOrDefault();
                }

                var createIssuanceStatusCommand = _mapper.Map<CreateIssuanceStatusCommand>(issuanceStatus);
                var result = await _mediator.Send(createIssuanceStatusCommand);

                if (issuanceStatus.DocumentStatusId == 5) // REJECTED: send email to company admin
                {
                    // locate company admin email and send to [TO] sender


                    var allUsersByCompany = _userManager.Users.Where(a => a.UserCompanyId == responseGetIssuanceById.Data.CompanyId).ToList();

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
                        Subject = "Issuance " + responseGetIssuanceById.Data.DOCNo + " need rejected.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/issuance/preview?id=" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                        Body = $"Document need rejected. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/issuance/preview?id=" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (issuanceStatus.DocumentStatusId == 1) // Submitted: check C1, C2 and APP is available
                {
                    if (ver != null)
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == ver).Select(a => a.Email).SingleOrDefault();
                    }
                    else if (app != null)
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();
                    }
                    else
                    {
                        emailTo = _userManager.Users.Where(a => a.Id == ack).Select(a => a.Email).SingleOrDefault();
                    }

                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "Issuance " + responseGetIssuanceById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/issuance/preview?id=" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/issuance/preview?id=" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."


                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }

                else if (issuanceStatus.DocumentStatusId == 2) // CONCURRED 1: check C2 and APP is available
                {

                    emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();

                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "Issuance " + responseGetIssuanceById.Data.DOCNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/issuance/preview/" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/issuance/preview?id=" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (issuanceStatus.DocumentStatusId == 3) // CONCURRED 2: check C2 and APP is available
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
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/issuance/preview/" + isssuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/issuance/preview?id=" + issuanceStatus.IssuanceId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (issuanceStatus.DocumentStatusId == 4) //APPROVED
                {
                    responseGetIssuanceById.Data.EffectiveDate = DateTime.Now;
                    var updateIssuancelCommand = _mapper.Map<UpdateIssuanceCommand>(responseGetIssuanceById.Data);
                    var result1 = await _mediator.Send(updateIssuancelCommand);
                }
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Issuance with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
                //}

                //else
                //{

                //}
                try
                {
                    var response = await _mediator.Send(new GetAllIssuanceStatusCachedQuery());
                    if (response.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<IssuanceStatusViewModel>>(response.Data);
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        var viewModel = new List<IssuanceStatusViewModel>();
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                }
                catch
                {
                    var viewModel = new List<IssuanceStatusViewModel>();
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Submit", issuanceStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, IssuanceStatusViewModel issuanceStatus)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createIssuanceStatusCommand = _mapper.Map<CreateIssuanceStatusCommand>(issuanceStatus);
                    var result = await _mediator.Send(createIssuanceStatusCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Issuance with ID {result.Data} Submitted.");
                    }
                    else _notify.Error(result.Message);
                }

                else
                {
                    //var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
                    //var result = await _mediator.Send(updateBrandCommand);
                    //if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllIssuanceStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<IssuanceStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", issuanceStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
    }
}

﻿using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.WIStatuses.Queries.GetAllCached;
// using EDocSys.Application.Features.WIStatuses.Commands.Create;
using EDocSys.Application.Features.WIs.Queries.GetAllCached;
//using EDocSys.Application.Features.WIs.Commands.Delete;
//using EDocSys.Application.Features.WIs.Commands.Update;
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
using EDocSys.Application.Features.WIStatuses.Commands.Create;
using EDocSys.Application.Interfaces.Shared;
using Microsoft.AspNetCore.Identity;
using EDocSys.Application.DTOs.Mail;
using System.Text.Encodings.Web;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Application.Features.WIs.Queries.GetById;
using EDocSys.Application.Features.WIs.Commands.Update;
using EDocSys.Application.Features.Issuances.Queries.GetByDOCPNo;
using EDocSys.Application.Features.Issuances.Commands.Update;
using EDocSys.Application.Features.Issuances.Queries.GetById;
using EDocSys.Application.Features.Issuances.Commands.Create;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]

    public class WIStatusController : BaseController<WIStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string c1 { get; private set; }
        public string c2 { get; private set; }
        public string app { get; private set; }
        public string emailTo { get; private set; }

        public WIStatusController(IMailService mailService, 
                                   UserManager<ApplicationUser> userManager,
                                   IdentityContext identityContext,
                                   RoleManager<IdentityRole> roleManager)
        {
            _mailService = mailService;
            _userManager = userManager;
            _identityContext = identityContext;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = new WIStatusViewModel();

            var response = await _mediator.Send(new GetAllWIsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WIViewModel>>(response.Data);

                var viewModelbyWSCPNo = viewModel.Where(a => a.Id == Id).SingleOrDefault();

                ViewBag.WSCPNo = viewModelbyWSCPNo.WSCPNo;
                ViewBag.WIId = viewModelbyWSCPNo.Id;
                return View(model);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll(int wiId)
        {
            var response = await _mediator.Send(new GetAllWIStatusCachedQuery());
            ViewBag.RoleD = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.Where(w => w.Email == currentUser.Email).ToList();
            List<string> rolesList = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesList.AddRange(roles);
            }
            if (rolesList.Contains("D"))
            {
                ViewBag.RoleD = true;
            }
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WIStatusViewModel>>(response.Data);
                var viewModelbyWSCPNo = viewModel.Where(a => a.WIId == wiId).ToList();

                return PartialView("_ViewAll", viewModelbyWSCPNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0, int wiId = 0)
        {
            var wiStatusResponse = await _mediator.Send(new GetAllWIStatusCachedQuery());

            if (id == 0)
            {
                var wistatusViewModel = new WIStatusViewModel();
                wistatusViewModel.DocumentStatusId = 7;
                wistatusViewModel.WIId = wiId;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", wistatusViewModel) });
            }
            else
            {
                return null;
            }
        }
        public async Task<JsonResult> OnGetSubmit(int id = 0, int wiId = 0, int status = 0)
        {
            var wiStatusResponse = await _mediator.Send(new GetAllWIStatusCachedQuery());

            if (id == 0)
            {
                var wistatusViewModel = new WIStatusViewModel();
                wistatusViewModel.DocumentStatusId = status;
                wistatusViewModel.WIId = wiId;

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", wistatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmit(int id, WIStatusViewModel wiStatus)
        {
            if (ModelState.IsValid)
            {
                var responseGetWIById = await _mediator.Send(new GetWIByIdQuery() { Id = wiStatus.WIId });
                if (responseGetWIById.Succeeded)
                {
                    c1 = responseGetWIById.Data.Concurred1;
                    c2 = responseGetWIById.Data.Concurred2;
                    app = responseGetWIById.Data.ApprovedBy;
                }

                var createWIStatusCommand = _mapper.Map<CreateWIStatusCommand>(wiStatus);
                var result = await _mediator.Send(createWIStatusCommand);

                if (wiStatus.DocumentStatusId == 1) // SUBMITTED: send email to company admin
                {
                    // locate company admin email and send to [TO] sender                    
                    var allUsersByCompany = _userManager.Users.Where(a => a.UserCompanyId == responseGetWIById.Data.CompanyId && a.IsActive == true).ToList();

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
                        Subject = "WI " + responseGetWIById.Data.WINo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/wi/preview?id=" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (wiStatus.DocumentStatusId == 5) // REJECTED: send email to department admin
                {
                    // locate company admin email and send to [TO] sender


                    var allUsersByCompany = _userManager.Users.Where(a => a.UserCompanyId == responseGetWIById.Data.CompanyId
                                                                       && a.UserDepartmentId == responseGetWIById.Data.DepartmentId
                                                                        && a.IsActive == true
                                                                       ).ToList();

                    var deptAdmin = (from a1 in allUsersByCompany
                                     join a2 in _identityContext.UserRoles on a1.Id equals a2.UserId
                                     join a3 in _roleManager.Roles on a2.RoleId equals a3.Id
                                     select new UserViewModel
                                     {
                                         Email = a1.Email,
                                         RoleName = a3.Name
                                     }).ToList();
                    string deptAdminEmail = deptAdmin.Where(a => a.RoleName == "D").Select(a => a.Email).FirstOrDefault();




                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        // To = "lgcompadmin@lion.com.my",
                        To = deptAdminEmail,
                        Subject = "WI " + responseGetWIById.Data.WINo + " has been rejected.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/procedure/preview?id=" + procedureStatus.ProcedureId)}'>clicking here</a> to open the document."
                        Body = $"Document has been rejected. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/wi/preview?id=" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (wiStatus.DocumentStatusId == 6) // FORMAT CHECKED: check C1, C2 and APP is available
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
                        Subject = "WI " + responseGetWIById.Data.WINo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/wi/preview?id=" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }

                else if (wiStatus.DocumentStatusId == 2) // CONCURRED 1: check C2 and APP is available
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
                        Subject = "WI " + responseGetWIById.Data.WINo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/wi/preview?id=" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (wiStatus.DocumentStatusId == 3) // CONCURRED 2: check C2 and APP is available
                {

                    emailTo = _userManager.Users.Where(a => a.Id == app).Select(a => a.Email).SingleOrDefault();


                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        //To = "lgcompadmin@lion.com.my",
                        To = emailTo,
                        Subject = "WI " + responseGetWIById.Data.WINo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        // Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://edocs.lion.com.my/documentation/wi/preview?id=" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (wiStatus.DocumentStatusId == 4) //APPROVED
                {
                    var wiViewModel = _mapper.Map<WIViewModel>(responseGetWIById.Data);
                    wiViewModel.EffectiveDate = DateTime.Now;
                    var updateWiCommand = _mapper.Map<UpdateWICommand>(wiViewModel);
                    var result1 = await _mediator.Send(updateWiCommand);
                    if (wiViewModel.ArchiveId != 0)
                    {
                        var responseGetWIByIdOld = await _mediator.Send(new GetWIByIdQuery() { Id = wiViewModel.ArchiveId });
                        var wiViewModelOld = _mapper.Map<SOPViewModel>(responseGetWIByIdOld.Data);
                        wiViewModelOld.ArchiveDate = DateTime.Now;
                        var updateWICommandOld = _mapper.Map<UpdateWICommand>(wiViewModelOld);
                        var result1Old = await _mediator.Send(updateWICommandOld);
                        var responseGetIssuanceInfoByDocNo = await _mediator.Send(new GetIssuanceInfoByDOCNoQuery() { docNo = wiViewModel.Id.ToString(), docType = "WI" });
                        var issInfoViewModelOld = _mapper.Map<List<IssuanceInfoViewModel>>(responseGetIssuanceInfoByDocNo.Data);
                        issInfoViewModelOld = issInfoViewModelOld.Where(w => w.DOCId == wiViewModelOld.Id.ToString()).ToList();
                        var listHID = issInfoViewModelOld.Select(s => s.HId).Distinct().ToList();
                        foreach (var Hid in listHID)
                        {
                            var response2 = await _mediator.Send(new GetIssuanceByIdQuery() { Id = Hid });
                            if (response2.Succeeded)
                            {
                                var issuanceVM = _mapper.Map<IssuanceViewModel>(response2.Data);
                                if (issuanceVM.DOCStatus == "Amend")
                                {
                                    var issInfoVM = issInfoViewModelOld.Where(w => w.HId == Hid).ToList();
                                    foreach (var info in issInfoVM)
                                    {
                                        info.DOCId = wiViewModel.Id.ToString();
                                        info.IsAmend = true;
                                        var updateIivmCommandOld = _mapper.Map<UpdateIssuanceInfoCommand>(info);
                                        var resultIssInfo = await _mediator.Send(updateIivmCommandOld);

                                        if (info.RecipientName1 != "" && info.RecipientName1 != null)
                                        {
                                            IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                                            iiprint.IssInfoId = info.Id;
                                            iiprint.RecipientName = info.RecipientName1;
                                            iiprint.IsActive = true;
                                            var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                                            var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                                        }
                                        if (info.RecipientName2 != "" && info.RecipientName2 != null)
                                        {
                                            IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                                            iiprint.IssInfoId = info.Id;
                                            iiprint.RecipientName = info.RecipientName2;
                                            iiprint.IsActive = true;
                                            var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                                            var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                                        }
                                        if (info.RecipientName3 != "" && info.RecipientName3 != null)
                                        {
                                            IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                                            iiprint.IssInfoId = info.Id;
                                            iiprint.RecipientName = info.RecipientName3;
                                            iiprint.IsActive = true;
                                            var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                                            var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                                        }
                                        if (info.RecipientName4 != "" && info.RecipientName4 != null)
                                        {
                                            IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                                            iiprint.IssInfoId = info.Id;
                                            iiprint.RecipientName = info.RecipientName4;
                                            iiprint.IsActive = true;
                                            var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                                            var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                                        }
                                        if (info.RecipientName5 != "" && info.RecipientName5 != null)
                                        {
                                            IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                                            iiprint.IssInfoId = info.Id;
                                            iiprint.RecipientName = info.RecipientName5;
                                            iiprint.IsActive = true;
                                            var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                                            var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                                        }
                                        if (info.RecipientName6 != "" && info.RecipientName6 != null)
                                        {
                                            IssuanceInfoPrintViewModel iiprint = new IssuanceInfoPrintViewModel();
                                            iiprint.IssInfoId = info.Id;
                                            iiprint.RecipientName = info.RecipientName6;
                                            iiprint.IsActive = true;
                                            var createIssuanceInfoPrintCommand = _mapper.Map<CreateIssuanceInfoPrintCommand>(iiprint);
                                            var resultInfoPrint = await _mediator.Send(createIssuanceInfoPrintCommand);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"WI with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
                //}

                //else
                //{

                //}
                try
                {
                    var response = await _mediator.Send(new GetAllWIStatusCachedQuery());
                    if (response.Succeeded)
                    {
                        var viewModel = _mapper.Map<List<WIStatusViewModel>>(response.Data);
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        var viewModel = new List<WIStatusViewModel>();
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                }
                catch
                {
                    var viewModel = new List<WIStatusViewModel>();
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Submit", wiStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        /* public async Task<JsonResult> OnGetSubmit(int id = 0, int wiId = 0, int status = 0)
         {
             var wiStatusResponse = await _mediator.Send(new GetAllWIStatusCachedQuery());

             if (id == 0)
             {
                 var wistatusViewModel = new WIStatusViewModel();
                 wistatusViewModel.DocumentStatusId = status;
                 wistatusViewModel.WIId = wiId;
                 return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", wistatusViewModel) });
             }
             else
             {
                 return null;
             }
         }*/
        /*
         [HttpPost]
         public async Task<JsonResult> OnPostSubmit(int id, WIStatusViewModel wiStatus)
         {
             if (ModelState.IsValid)
             {
                 if (id == 0)
                 {
                     var createWIStatusCommand = _mapper.Map<CreateWIStatusCommand>(wiStatus);
                     var result = await _mediator.Send(createWIStatusCommand);
                     if (result.Succeeded)
                     {
                         id = result.Data;
                         _notify.Success($"WI with ID {result.Data} Submitted. ");
                     }
                     else _notify.Error(result.Message);
                 }
                 else
                 {

                 }
                 var response = await _mediator.Send(new GetAllWIStatusCachedQuery());
                 if (response.Succeeded)
                 {
                     var viewModel = _mapper.Map<List<WIStatusViewModel>>(response.Data);
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
                 var html = await _viewRenderer.RenderViewToStringAsync("_Submit", wiStatus);
                 return new JsonResult(new { isValid = false, html = html });
             }
         }
        */
        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, WIStatusViewModel wiStatus)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createWIStatusCommand = _mapper.Map<CreateWIStatusCommand>(wiStatus);
                    var result = await _mediator.Send(createWIStatusCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"WI with ID {result.Data} Submitted.");
                    }
                    else _notify.Error(result.Message);
                }

                else
                {
                    //var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
                    //var result = await _mediator.Send(updateBrandCommand);
                    //if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllWIStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<WIStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", wiStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
    }
}

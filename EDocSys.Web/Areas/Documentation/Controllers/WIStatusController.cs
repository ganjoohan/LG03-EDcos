using EDocSys.Application.Constants;
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

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]

    public class WIStatusController : BaseController<WIStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public string c1 { get; private set; }
        public string c2 { get; private set; }
        public string app { get; private set; }
        public string emailTo { get; private set; }

        public WIStatusController(IMailService mailService, UserManager<ApplicationUser> userManager)
        {
            _mailService = mailService;
            _userManager = userManager;
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
                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        To = "lgcompadmin@lion.com.my",
                        Subject = "WI " + responseGetWIById.Data.WSCPNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
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
                        Subject = "WI " + responseGetWIById.Data.WSCPNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
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
                        Subject = "WI " + responseGetWIById.Data.WSCPNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
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
                        Subject = "Thank you for registering",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/wi/preview/" + wiStatus.WIId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
                    {

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

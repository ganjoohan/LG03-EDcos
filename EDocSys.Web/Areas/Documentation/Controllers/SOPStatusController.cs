using EDocSys.Application.Constants;
using EDocSys.Application.Features.Departments.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
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

using EDocSys.Application.DTOs.Mail;
using System.Net.Mail;
using EDocSys.Application.Interfaces.Shared;
using System.Text.Encodings.Web;
using EDocSys.Application.Features.Procedures.Queries.GetById;
using Microsoft.AspNetCore.Identity;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Application.Features.SOPs.Queries.GetAllCached;
using EDocSys.Application.Features.SOPStatuses.Queries.GetAllCached;
using EDocSys.Application.Features.SOPs.Queries.GetById;
using EDocSys.Application.Features.ProcedureStatuses.Commands.Create;
using EDocSys.Application.Features.SOPStatuses.Commands.Create;

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class SOPStatusController : BaseController<SOPStatusController>
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public string c1 { get; private set; }
        public string c2 { get; private set; }
        public string app { get; private set; }
        public string emailTo { get; private set; }


        public SOPStatusController(IMailService mailService, UserManager<ApplicationUser> userManager)
        {
            _mailService = mailService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = new SOPStatusViewModel();

            var response = await _mediator.Send(new GetAllSOPsCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPViewModel>>(response.Data);

                var viewModelbyWSCPNo = viewModel.Where(a => a.Id == Id).SingleOrDefault();

                ViewBag.WSCPNo = viewModelbyWSCPNo.WSCPNo;
                ViewBag.SOPId = viewModelbyWSCPNo.Id;
                return View(model);
            }
            return null;
        }

        public async Task<IActionResult> LoadAll(int sopId)
        {
            var response = await _mediator.Send(new GetAllSOPStatusCachedQuery());

            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<SOPStatusViewModel>>(response.Data);
                var viewModelbyWSCPNo = viewModel.Where(a => a.SOPId == sopId).ToList();

                return PartialView("_ViewAll", viewModelbyWSCPNo);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0, int procedureId = 0)
        {
            var procedureStatusResponse = await _mediator.Send(new GetAllSOPStatusCachedQuery());

            if (id == 0)
            {
                var procedurestatusViewModel = new ProcedureStatusViewModel();
                procedurestatusViewModel.DocumentStatusId = 7;
                procedurestatusViewModel.ProcedureId = procedureId;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", procedurestatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        public async Task<JsonResult> OnGetSubmit(int id = 0, int sopId = 0, int status = 0)
        {
            var sopStatusResponse = await _mediator.Send(new GetAllSOPStatusCachedQuery());

            if (id == 0)
            {
                var sopstatusViewModel = new SOPStatusViewModel();
                sopstatusViewModel.DocumentStatusId = status;
                sopstatusViewModel.SOPId = sopId;

                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Submit", sopstatusViewModel) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostSubmit(int id, SOPStatusViewModel sopStatus)
        {
            if (ModelState.IsValid)
            {
                var responseGetSOPById = await _mediator.Send(new GetSOPByIdQuery() { Id = sopStatus.SOPId });
                if (responseGetSOPById.Succeeded)
                {
                    c1 = responseGetSOPById.Data.Concurred1;
                    c2 = responseGetSOPById.Data.Concurred2;
                    app = responseGetSOPById.Data.ApprovedBy;
                }

                var createSOPStatusCommand = _mapper.Map<CreateSOPStatusCommand>(sopStatus);
                var result = await _mediator.Send(createSOPStatusCommand);

                if (sopStatus.DocumentStatusId == 1) // SUBMITTED: send email to company admin
                {
                    MailRequest mail = new MailRequest()
                    {
                        //To = userModel.Email,
                        To = "lgcompadmin@lion.com.my",
                        Subject = "SOP " + responseGetSOPById.Data.WSCPNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/sop/preview/" + sopStatus.SOPId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (sopStatus.DocumentStatusId == 6) // FORMAT CHECKED: check C1, C2 and APP is available
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
                        Subject = "SOP " + responseGetSOPById.Data.WSCPNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/procedure/preview/" + sopStatus.SOPId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                else if (sopStatus.DocumentStatusId == 2) // CONCURRED 1: check C2 and APP is available
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
                        Subject = "SOP " + responseGetSOPById.Data.WSCPNo + " need approval.",
                        // 
                        //Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("www.liongroup.com.my")}'>clicking here</a> to open the document."
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/sop/preview/" + sopStatus.SOPId)}'>clicking here</a> to open the document."
                    };

                    try
                    {
                        await _mailService.SendAsync(mail);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (sopStatus.DocumentStatusId == 3) // CONCURRED 2: check C2 and APP is available
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
                        Body = $"Document need approval. <a href='{HtmlEncoder.Default.Encode("https://localhost:5001/documentation/sop/preview/" + sopStatus.SOPId)}'>clicking here</a> to open the document."
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
                    _notify.Success($"SOP with ID {result.Data} Submitted. ");
                }
                else _notify.Error(result.Message);
                //}

                //else
                //{

                //}

                var response = await _mediator.Send(new GetAllSOPStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<SOPStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_Submit", sopStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }


        /*
        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, ProcedureStatusViewModel procedureStatus)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createProcedureStatusCommand = _mapper.Map<CreateSOPStatusCommand>(procedureStatus);
                    var result = await _mediator.Send(createProcedureStatusCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Procedure with ID {result.Data} Submitted.");
                    }
                    else _notify.Error(result.Message);
                }

                else
                {
                    //var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
                    //var result = await _mediator.Send(updateBrandCommand);
                    //if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllSOPStatusCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<ProcedureStatusViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", procedureStatus);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
        */
    }
}

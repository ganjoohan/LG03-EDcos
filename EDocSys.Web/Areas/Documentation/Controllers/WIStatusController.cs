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

namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class WIStatusController : BaseController<WIStatusController>
    {
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

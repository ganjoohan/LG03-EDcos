using EDocSys.Application.Features.Companies.Commands.Create;
using EDocSys.Application.Features.Companies.Commands.Delete;
using EDocSys.Application.Features.Companies.Commands.Update;
using EDocSys.Application.Features.Companies.Queries.GetAllCached;
using EDocSys.Application.Features.Companies.Queries.GetById;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Areas.Documentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace EDocSys.Web.Areas.Documentation.Controllers
{
    [Area("Documentation")]
    public class CompanyController : BaseController<CompanyController>
    {
        public IActionResult Index()
        {
            var model = new CompanyViewModel();
            return View(model);
        }

        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetAllCompaniesCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<CompanyViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
        {
            var departmentsResponse = await _mediator.Send(new GetAllCompaniesCachedQuery());

            if (id == 0)
            {
                var departmentViewModel = new CompanyViewModel();
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", departmentViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetCompanyByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var departmentViewModel = _mapper.Map<CompanyViewModel>(response.Data);
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", departmentViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, CompanyViewModel department)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createCompanyCommand = _mapper.Map<CreateCompanyCommand>(department);
                    var result = await _mediator.Send(createCompanyCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Company with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateCompanyCommand = _mapper.Map<UpdateCompanyCommand>(department);
                    var result = await _mediator.Send(updateCompanyCommand);
                    if (result.Succeeded) _notify.Information($"Company with ID {result.Data} Updated.");
                }
                var response = await _mediator.Send(new GetAllCompaniesCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<CompanyViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", department);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(int id)
        {
            var deleteCommand = await _mediator.Send(new DeleteCompanyCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Company with Id {id} Deleted.");
                var response = await _mediator.Send(new GetAllCompaniesCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<CompanyViewModel>>(response.Data);
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
    }
}
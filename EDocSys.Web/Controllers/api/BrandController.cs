using EDocSys.Web.Controllers;
using EDocSys.Application.Features.Brands.Commands.Create;
using EDocSys.Application.Features.Brands.Commands.Delete;
using EDocSys.Application.Features.Brands.Commands.Update;
using EDocSys.Application.Features.Brands.Queries.GetAllCached;
using EDocSys.Application.Features.Brands.Queries.GetById;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EDocSys.Web.Controllers.api
{
    public class BrandController : BaseWebApiController<BrandController>
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllBrandsCachedQuery());
            return Ok(brands);
        }
    }
}

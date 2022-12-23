using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.ViewComponents
{
    public class IssuanceDetails : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public IssuanceDetails(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getIssuance = db.Issuances.Where(a => a.Id == Id).SingleOrDefault();
            if (getIssuance != null)
            {
                var m = await db.Issuances.FindAsync(getIssuance.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getIssuance = db.Issuances.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getIssuance != null)
        //    {
        //        var m = await db.Issuances.FindAsync(getIssuance.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
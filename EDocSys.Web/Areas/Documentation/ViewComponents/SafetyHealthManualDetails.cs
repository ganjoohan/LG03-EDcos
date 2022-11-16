using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.ViewComponents
{
    public class SafetyHealthManualDetails : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public SafetyHealthManualDetails(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getSafetyHealthManualId = db.SafetyHealthManuals.Where(a => a.Id == Id).SingleOrDefault();
            if (getSafetyHealthManualId != null)
            {
                var m = await db.SafetyHealthManuals.FindAsync(getSafetyHealthManualId.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getSafetyHealthManualId = db.SafetyHealthManuals.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getSafetyHealthManualId != null)
        //    {
        //        var m = await db.SafetyHealthManuals.FindAsync(getSafetyHealthManualId.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
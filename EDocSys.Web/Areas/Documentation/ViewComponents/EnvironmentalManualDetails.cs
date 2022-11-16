using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.ViewComponents
{
    public class EnvironmentalManualDetails : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public EnvironmentalManualDetails(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getEnvironmentalManualId = db.EnvironmentalManuals.Where(a => a.Id == Id).SingleOrDefault();
            if (getEnvironmentalManualId != null)
            {
                var m = await db.EnvironmentalManuals.FindAsync(getEnvironmentalManualId.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getEnvironmentalManualId = db.EnvironmentalManuals.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getEnvironmentalManualId != null)
        //    {
        //        var m = await db.EnvironmentalManuals.FindAsync(getEnvironmentalManualId.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
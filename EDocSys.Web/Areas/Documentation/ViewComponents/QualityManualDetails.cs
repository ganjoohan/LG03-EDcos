using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.ViewComponents
{
    public class QualityManualDetails : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public QualityManualDetails(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getQualityManualId = db.QualityManuals.Where(a => a.Id == Id).SingleOrDefault();
            if (getQualityManualId != null)
            {
                var m = await db.QualityManuals.FindAsync(getQualityManualId.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getQualityManualId = db.QualityManuals.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getQualityManualId != null)
        //    {
        //        var m = await db.QualityManuals.FindAsync(getQualityManualId.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
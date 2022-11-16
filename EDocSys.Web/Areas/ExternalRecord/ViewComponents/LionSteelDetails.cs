using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.ExternalRecord.ViewComponents
{
    public class LionSteelDetails : ViewComponent
    {
        private readonly ApplicationExternalDbContext db;
        public LionSteelDetails(ApplicationExternalDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getLionSteelId = db.LionSteels.Where(a => a.Id == Id).SingleOrDefault();
            if (getLionSteelId != null)
            {
                var m = await db.LionSteels.FindAsync(getLionSteelId.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getDocumentManualId = db.DocumentManuals.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getDocumentManualId != null)
        //    {
        //        var m = await db.DocumentManuals.FindAsync(getDocumentManualId.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
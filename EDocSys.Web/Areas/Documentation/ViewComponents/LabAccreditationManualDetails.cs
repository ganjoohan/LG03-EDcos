using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.ViewComponents
{
    public class LabAccreditationManualDetails : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public LabAccreditationManualDetails(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getLabAccreditationManualId = db.LabAccreditationManuals.Where(a => a.Id == Id).SingleOrDefault();
            if (getLabAccreditationManualId != null)
            {
                var m = await db.LabAccreditationManuals.FindAsync(getLabAccreditationManualId.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getLabAccreditationManualId = db.LabAccreditationManuals.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getLabAccreditationManualId != null)
        //    {
        //        var m = await db.LabAccreditationManuals.FindAsync(getLabAccreditationManualId.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
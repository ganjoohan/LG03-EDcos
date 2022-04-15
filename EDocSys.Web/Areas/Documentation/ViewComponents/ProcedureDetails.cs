using EDocSys.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Documentation.ViewComponents
{
    public class ProcedureDetails : ViewComponent
    {
        private readonly ApplicationDbContext db;
        public ProcedureDetails(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var getProcId = db.Procedures.Where(a => a.Id == Id).SingleOrDefault();
            if (getProcId != null)
            {
                var m = await db.Procedures.FindAsync(getProcId.Id);
                return View(m);
            }
            return null;

        }

        //public async Task<IViewComponentResult> InvokeAsync(string wscpno)
        //{
        //    var getProcId = db.Procedures.Where(a => a.WSCPNo == wscpno).SingleOrDefault();
        //    if (getProcId != null)
        //    {
        //        var m = await db.Procedures.FindAsync(getProcId.Id);
        //        return View(m);
        //    }
        //    return null;

        //}
    }
}
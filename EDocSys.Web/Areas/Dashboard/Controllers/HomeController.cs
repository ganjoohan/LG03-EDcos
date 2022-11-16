using EDocSys.Web.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EDocSys.Web.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : BaseController<HomeController>
    {
        public IActionResult Index()
        {
            _notify.Information("Hi There!");
            //string newLine = " \\n ";
            //string s_header = "STEEL DIVISION";
            //string s_reminder = "REMINDER : NON-DISCLOSURE OF CONFIDENTIAL INFORMATION";
            //string s_line1 = "Note: Please be reminded that by accessing this eDOC System, you are legally bound to follow the rules and regulations, conditions and/or terms of reference governing this system.";
            //string s_line2 = "I agree at all times to hold in strictest confidence, and not to use, except for the benefit of the Company, or to disclose to any person, firm, or corporation without authorization from authorized approver, any confidential information stored in the Steel Division eDOC System.";
            //string s_line3 = "I acknowledge that all information stored in this database is the property of The Lion Group and I also understand that this information is to be used solely for job-related purposes.  I agree not to reproduce and circulate any stored information unless authorized and also understand that by passing my user ID and password to any staff, I will be held responsible for any information leaked out under my user ID.";
            //string s_line4 = "I also acknowledge that the contents in this agreement can similarly be found or covered in the Company Human Resource Policies and/ or employment letter/contract concerning Non-Disclosure of Confidential Information and I fully understand that I can be disciplined, up to and including having legal action being taken against me and/or termination of employment if found guilty of any violation.";
            //string s_alert = s_header + newLine + s_reminder + newLine + newLine;
            //s_alert += s_line1 + newLine + newLine + s_line2 + newLine + s_line3 + newLine + s_line4;
            //ViewBag.Message = s_alert;
            return View();
        }
    }
}
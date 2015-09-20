using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTAT.WebClient.Controllers
{
    public class WebClientController : Controller
    {

        // GET: /WebClient/

        public ActionResult Administration()
        {
            return View();
        }
        public ActionResult Contacts()
        {
            return View();
        }
        public ActionResult Copyright()
        {
            return View();
        }
        public ActionResult DashboardsManagement()
        {
            return View();
        }
        public ActionResult DashboardsManagement2()
        {
            return View();
        }
        public ActionResult Help()
        {
            return View();
        }
        public ActionResult Index(int? QueryId, string UserCode)
        {
            if (QueryId.HasValue && !string.IsNullOrEmpty(UserCode))
                ViewBag.query = new QueryController().GetSigleQuery(QueryId.Value, UserCode);
            
            return View();
        }
        public ActionResult Layout()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Manual()
        {
            return View();
        }
        public ActionResult TemplatesManagement()
        {
            return View();
        }
        public ActionResult Templates()
        {
            return View();
        }
        public ActionResult Sitemap()
        {
            return View();
        }
        public ActionResult UseLinks()
        {
            return View();
        }
        public ActionResult W3Ccheck()
        {
            return View();
        }
        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult Profiles()
        {
            return View();
        }
        public ActionResult Queries()
        {
            return View();
        }
        public ActionResult RegUsers()
        {
            return View();
        }
        public ActionResult Statistics()
        {
            return View();
        }

    }
}

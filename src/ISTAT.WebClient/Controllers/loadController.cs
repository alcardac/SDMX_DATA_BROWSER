using ISTAT.WebClient.Engine.Manager;
using ISTAT.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTAT.WebClient.Controllers
{
    public class loadController : Controller
    {
        private SessionObject sessionObject = new SessionObject();
        public ResultsRequests down = new ResultsRequests();

        public ActionResult Refresh()
        {
            down.Refresh(new DownloadSupport(), sessionObject.GetSessionQuery());
            return null;
        }
        public ActionResult UpdateLayout()
        {
            down.UpdateLayout(new DownloadSupport(), sessionObject.GetSessionQuery());
            return null;
        }
        public ActionResult UpdateSliceKey()
        {
            down.UpdateSliceKey(new DownloadSupport(), sessionObject.GetSessionQuery());
            return null;
        }
    }
}

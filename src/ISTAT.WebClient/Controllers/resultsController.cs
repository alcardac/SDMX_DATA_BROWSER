using ISTAT.WebClient.Engine.Manager;
using ISTAT.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTAT.WebClient.Controllers
{
    public class resultsController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();
        private SessionObject sessionObject = new SessionObject();
        public MainRequests JR = new MainRequests();


        public ActionResult ResetDisplayMode()
        {
            return CS.ReturnForJQuery(JR.ResetDisplayMode(sessionObject.GetSessionQuery()));
        }

        public ActionResult ToggleKeyDisplayMode()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.ToggleKeyDisplayMode(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.key));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult ToggleValueDisplayMode()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.ToggleValueDisplayMode(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.key, (string)PostDataArrived.value));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult UpdateLayout()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.UpdateLayout(sessionObject.GetSessionQuery(),
                    (string[])PostDataArrived.sliceAxis.ToObject<string[]>(), (string[])PostDataArrived.horizontalAxis.ToObject<string[]>(), (string[])PostDataArrived.verticalAxis.ToObject<string[]>()));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult UpdateSliceKey()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.UpdateSliceKey(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.key, (string)PostDataArrived.value));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult ViewResults()
        {
            return CS.ReturnForJQuery(JR.ViewResults(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient()));
        }

    }
}

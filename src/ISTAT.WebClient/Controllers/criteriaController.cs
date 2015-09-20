using ISTAT.WebClient.Complements.Model.App_GlobalResources;
using ISTAT.WebClient.Complements.Model.Exceptions;
using ISTAT.WebClient.Complements.Model.Properties;
using ISTAT.WebClient.Engine.Manager;
using ISTAT.WebClient.Engine.Model;
using ISTAT.WebClient.Engine.Model.GlobalSession;
using ISTAT.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTAT.WebClient.Controllers
{
    public class criteriaController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();
        private SessionObject sessionObject = new SessionObject();
        public MainRequests JR = new MainRequests();


        public ActionResult getComponents()
        {
            return CS.ReturnForJQuery(JR.GetComponents(sessionObject.GetSessionQuery()));

        }


        public ActionResult clear()
        {
            return CS.ReturnForJQuery(JR.ClearCriteria(sessionObject.GetSessionQuery()));
        }


        public ActionResult ComponentEditForm()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.ComponentEditForm(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient(),
                    (string)PostDataArrived.concept));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult ComponentSave()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.ComponentSave(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient(),
                    (string)PostDataArrived.concept, (string[])PostDataArrived.ids.ToObject<string[]>()));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult GetChildrenCodes()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.GetChildrenCodes(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.parentCode));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult SimpleComponentSave()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.SimpleComponentSave(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.concept, (string)PostDataArrived.value));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult TimeComponentSave()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(JR.TimeComponentSave(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.startDate, (string)PostDataArrived.endDate));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

    }
}

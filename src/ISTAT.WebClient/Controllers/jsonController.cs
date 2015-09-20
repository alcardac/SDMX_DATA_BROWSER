using ISTAT.WebClient.Complements.Model.App_GlobalResources;
using ISTAT.WebClient.Complements.Model.Exceptions;
using ISTAT.WebClient.Complements.Model.Properties;
using ISTAT.WebClient.Engine.Builder.Tree;
using ISTAT.WebClient.Engine.Manager;
using ISTAT.WebClient.Engine.Model;
using ISTAT.WebClient.Engine.Model.GlobalSession;
using ISTAT.WebClient.Models;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ISTAT.WebClient.Complements.Model.CallWS;
using ISTAT.WebClient.Complements.Model;
using ISTAT.WebClient.Complements.Model.Settings;

namespace ISTAT.WebClient.Controllers
{
    public class jsonController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();
        private SessionObject sessionObject = new SessionObject();
        public MainRequests JR = new MainRequests();

        public ActionResult getDataflows()
        {

            
            if (!SessionQueryManager.SessionQueryExistsAndIsValid(HttpContext.ApplicationInstance.Context))
            {
                SessionQuery query = new SessionQuery();
                SessionQueryManager.SaveSessionQuery(HttpContext.ApplicationInstance.Context.Session, query);
            }

            EndPointStructure ep = ISTATSettings.CurrentEndPoint;
            if (NSIClientSettings.Instance.EndPointCenterEnable)
                SetEndPoint(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient(), ISTATSettings.CentralEndPoint);

            var res= CS.ReturnForJQuery(JR.GetTree(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient()));

            if (NSIClientSettings.Instance.EndPointCenterEnable)
                SetEndPoint(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient(), ep);

            return res;

        }
       
        public ActionResult reload()
        {
            return CS.ReturnForJQuery(JR.ReloadMainPage(sessionObject.GetSessionQuery()));
        }
       
        public ActionResult SetLocale()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
          
            try
            {
                return CS.ReturnForJQuery(JR.SetLocale(sessionObject.GetSessionQuery(),
                    (string)PostDataArrived.locale));

                LocaleResolver.RemoveCookie(HttpContext.ApplicationInstance.Context);
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult ping()
        {
            return CS.ReturnForJQuery(JR.Ping(sessionObject.GetSessionQuery()));
        }

        public ActionResult getMessages()
        {
            var ser = new JavaScriptSerializer();
            var messages = new Dictionary<string, string>();
            foreach (DictionaryEntry a in Messages.GetResourceSet(Thread.CurrentThread.CurrentUICulture))
                messages.Add(a.Key.ToString(), a.Value.ToString());
            string json = ser.Serialize(messages);
            return CS.ReturnForJQuery(json);
        }

        public ActionResult dataflowChange()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                SessionQuery query = sessionObject.GetSessionQuery();
                                               
                if (NSIClientSettings.Instance.EndPointCenterEnable)
                {
                    EndPointStructure ep = new EndPointStructure()
                    {
                        ID = "WS of " + (string)PostDataArrived.agency + "+" + (string)PostDataArrived.id + "+" + (string)PostDataArrived.version,
                        DisplayName = "WS of " + (string)PostDataArrived.agency + "+" + (string)PostDataArrived.id + "+" + (string)PostDataArrived.version,
                        EndPoint = (string.IsNullOrEmpty((string)PostDataArrived.urlV21)) ? ISTATSettings.ListEndPoint.First().EndPoint : (string)PostDataArrived.urlV21,
                        EndPointV20 = (string.IsNullOrEmpty((string)PostDataArrived.urlV20)) ? ISTATSettings.ListEndPoint.First().EndPointV20 : (string)PostDataArrived.urlV20,
                        EndPointType = "V21",
                    };
                    SetEndPoint(query, sessionObject.GetNSIClient(), ep, true);
                }


                ActionResult res = CS.ReturnForJQuery(
                    JR.DataflowChange(
                    query, 
                    sessionObject.GetNSIClient(),
                    (string)PostDataArrived.agency,
                    (string)PostDataArrived.id, 
                    (string)PostDataArrived.version,
                    Server.MapPath("~/query/")));

                SessionQueryManager.SaveSessionQuery(HttpContext.ApplicationInstance.Context.Session, query);
                return res;
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }
       
        public ActionResult CheckNumberOfObservations()
        {
            return CS.ReturnForJQuery(JR.CheckNumberOfObservations(sessionObject.GetSessionQuery(), sessionObject.GetNSIClient()));
        }

        public ActionResult ChangeEndPoint()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);
            try
            {
                return CS.ReturnForJQuery(ChangeEndPoint(
                    sessionObject.GetSessionQuery(),
                    sessionObject.GetNSIClient(),
                   (string)PostDataArrived.id));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        private void SetEndPoint(SessionQuery query, INsiClient nsiClient, EndPointStructure ep, bool clearQuery = false)
        {

            if (ep == null) return;

            //Session[Estat.Nsi.Client.Web.ISTATSettings.Session_key_endpoint] = ep;

            ISTATSettings.CurrentEndPoint = ep;

            NSIClientSettings.Instance.SetEndPoint(ISTATSettings.CurrentEndPoint);

            // NSIClientSettings settings = NSIClientSettings.GetSection(System.Configuration.ConfigurationUserLevel.None);
            NSIClientSettings settings = NSIClientSettings.Instance;

            SessionObject.InitializeNSIClient();


            // clear criteria only if was set a data end point 
            if (clearQuery)
            {

                if (query != null)
                {
                    query.Clear();
                    query.Reset();
                }
            }

        }


        private string ChangeEndPoint(SessionQuery query, INsiClient nsiClient, string id)
        {
            IEnumerable<EndPointStructure> _ep =
                    (from ep in ISTATSettings.ListEndPoint
                     where ep.ID.Equals(id)
                     select ep).OfType<EndPointStructure>();

            SetEndPoint(query, nsiClient, _ep.First());


            return "{\"change\" : true }";
        }

    }
}

using ISTAT.WebClient.Complements.Model;
using ISTAT.WebClient.Complements.Model.App_GlobalResources;
using ISTAT.WebClient.Complements.Model.Settings;
using ISTAT.WebClient.Engine.Model.GlobalSession;
using ISTAT.WebClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MvcApplicationTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult HomePage()
        {
            ViewBag.Message = "Modify this TT to jump-start your ASP.NET MVC application.";
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this TT to jump-start your ASP.NET MVC application.";
            ViewBag.CurrentWorkingPage = this.CurrentWorkingPage;
            ViewBag.DataflowId = this.DataflowId;
            ViewBag.IsEmpty = this.IsEmpty;
            ViewBag.MessagesToJson = this.MessagesToJson;
            ViewBag.AvailableLocale = this.AvailableLocale;
            ViewBag.CentralEndPoint = ISTATSettings.CentralEndPoint;
            ViewBag.CurrentEndPoint = ISTATSettings.CurrentEndPoint;
            ViewBag.ListEndPoint = ISTATSettings.ListEndPoint;
            ViewBag.AvailableLocale = this.AvailableLocale;
            ViewBag.SupportedPageSizes = ISTAT.WebClient.Engine.Model.DataRender.PdfRenderer.SupportedPageSizes;
            Istat_OnLoadHook();
            return View();
        }

        public ActionResult BrowserSupport()
        {
            ViewBag.Message = "Modify this TT to jump-start your ASP.NET MVC application.";
            ViewBag.CurrentWorkingPage = this.CurrentWorkingPage;
            ViewBag.DataflowId = this.DataflowId;
            ViewBag.IsEmpty = this.IsEmpty;
            ViewBag.MessagesToJson = this.MessagesToJson;
            ViewBag.AvailableLocale = this.AvailableLocale;
            ViewBag.CentralEndPoint = ISTATSettings.CentralEndPoint;
            ViewBag.CurrentEndPoint = ISTATSettings.CurrentEndPoint;
            ViewBag.ListEndPoint = ISTATSettings.ListEndPoint;
            ViewBag.AvailableLocale = this.AvailableLocale;
            ViewBag.SupportedPageSizes = ISTAT.WebClient.Engine.Model.DataRender.PdfRenderer.SupportedPageSizes;
            Istat_OnLoadHook();
            return View();
        }

        /// <summary>
        /// Gets the current working page from the session
        /// </summary>
        /// <value>
        ///   0 - For criteria tab, 1 - for resutls tab
        /// </value>
        public int CurrentWorkingPage
        {
            get
            {
                int workingPage = 0;
                if (SessionQueryManager.SessionQueryExistsAndIsValid(HttpContext.ApplicationInstance.Context))
                {
                    SessionQuery query = SessionQueryManager.GetSessionQuery(HttpContext.ApplicationInstance.Context.Session);
                    workingPage = query.WorkPageIdx;
                }

                return workingPage;
            }
        }

        /// <summary>
        /// Gets the currenty selected Dataflow id in Agency+Id+Version format
        /// </summary>
        /// <value>
        ///   A String with the Dataflow id in Agency+Id+Version format
        /// </value>
        public string DataflowId
        {
            get
            {
                if (SessionQueryManager.SessionQueryExistsAndIsValid(HttpContext.ApplicationInstance.Context))
                {
                    SessionQuery query = SessionQueryManager.GetSessionQuery(HttpContext.ApplicationInstance.Context.Session);

                    return query.GetDataflowId();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the criteria stored in the session are empty
        /// </summary>
        /// <value>
        ///   True if there are no criteria in the session else false
        /// </value>
        public bool IsEmpty
        {
            get
            {
                bool ret = true;
                if (SessionQueryManager.SessionQueryExistsAndIsValid(HttpContext.ApplicationInstance.Context))
                {
                    SessionQuery query = SessionQueryManager.GetSessionQuery(HttpContext.ApplicationInstance.Context.Session);
                    ret = query.IsEmpty();
                }

                return ret;
            }
        }


        /// <summary>
        /// Gets all <see cref="Resources.Messages"/>
        /// </summary>
        /// <value>
        ///   The &lt;see cref=&quot;Resources.Messages&quot;/&gt; in json string format
        /// </value>
        public string MessagesToJson
        {
            get
            {
                return GetMessages(Thread.CurrentThread.CurrentUICulture);
            }
        }

        /// <summary>
        /// Get all <see cref="Resources.Messages"/>
        /// </summary>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="Resources.Messages"/> in json string format
        /// </returns>
        public string GetMessages(CultureInfo culture)
        {
            var ser = new JavaScriptSerializer();

            // ser.RecursionLimit = 1;
            var messages = new Dictionary<string, string>();
            List<DictionaryEntry> rst = Messages.GetResourceSet(culture);
            foreach (DictionaryEntry a in rst)
            {
                messages.Add(a.Key.ToString(), a.Value.ToString());
            }

            string json = ser.Serialize(messages);
            return json;
        }
       
        /// <summary>
        /// Gets the Available Locale.
        /// </summary>
        public ICollection<CultureInfo> AvailableLocale
        {
            get
            {
                return Messages.AvailableLocales();
            }
        }


        #region ISTAT Extend

        private void Istat_OnLoadHook()
        {
            NSIClientSettings settings = NSIClientSettings.Instance;

            try
            {
                if (settings.EndPointCenterEnable)
                {
                    if (ISTATSettings.CentralEndPoint == null)
                    {
                        ISTATSettings.CentralEndPoint = new EndPointStructure()
                        {
                            ID = "WS_C",
                            DisplayName = "Web Serivce Centrale",
                            EndPoint = settings.EndPoint,
                            EndPointV20 = settings.EndPointV20,
                            EndPointType = settings.EndPointType,
                            logSDMX = settings.LogSDMX,
                        };
                    }
                    // ISTATSettings.ListEndPoint.Add(ISTATSettings.CentralEndPoint);
                }
                else
                {
                    ISTATSettings.ListEndPoint = new List<EndPointStructure>();

                    var pathFile = System.Configuration.ConfigurationManager.AppSettings["EndPointListFile"].ToString();
                    pathFile = Server.MapPath(pathFile);
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.Load(pathFile);
                    foreach (System.Xml.XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        ISTATSettings.ListEndPoint.Add(new
                            EndPointStructure()
                        {
                            ID = node.Attributes["ID"].InnerText.Trim(),
                            DisplayName = node.Attributes["DisplayName"].InnerText.Trim(),
                            EndPoint = node.Attributes["EndPoint"].InnerText.Trim(),
                            EndPointV20 = node.Attributes["EndPointV20"].InnerText.Trim(),
                            EndPointType = node.Attributes["EndPointType"].InnerText.Trim(),
                            logSDMX = (node.Attributes["logSDMX"].InnerText.Trim().ToLower() == "true") ? true : false,
                        });
                    }
                    //setting EndpointType
                    settings.SetListEndPoint(ISTATSettings.ListEndPoint);
                    settings.SetEndPoint(ISTATSettings.ListEndPoint[0]);
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        #endregion
    }
}

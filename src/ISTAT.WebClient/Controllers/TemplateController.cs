using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISTAT.WebClient.Models;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;

namespace ISTAT.WebClient.Controllers
{
    public class TemplateController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();

        public ActionResult GetTemplateList()
        {

            try
            {

                GetTemplateObject PostDataArrived = CS.GetPostData<GetTemplateObject>(this.Request);

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];

                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))

                    throw new Exception("ConnectionString not set");



                TemplateWidget qw = new TemplateWidget(connectionStringSetting.ConnectionString);

                return CS.ReturnForJQuery(qw.Get(PostDataArrived));

            }

            catch (Exception ex)
            {

                return CS.ReturnForJQuery(ex.Message);

            }

        }

        public ActionResult Get()
        {
            try
            {
                GetTemplateObject PostDataArrived = CS.GetPostData<GetTemplateObject>(this.Request);
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                TemplateWidget qw = new TemplateWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Get(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }
        public ActionResult Add()
        {
            try
            {
                // Lettura parametri tramite cast di tipi
                GetTemplateObject PostDataArrived = CS.GetPostData<GetTemplateObject>(this.Request);
                // Lettura connection string
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                // Check sulla connection string
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                // Inizializzazione Service dei template
                TemplateWidget qw = new TemplateWidget(connectionStringSetting.ConnectionString);
                // Ritorna Json al client con il risultato della chiamata TemplateWidget.Add(GetTemplateObject arg)
                return CS.ReturnForJQuery(qw.Add(PostDataArrived));
            }
            catch (Exception ex)
            {
                // Ritorna Json al client con messaggio di errore
                return CS.ReturnForJQuery(ex.Message);
            }
        }
        public ActionResult Del()
        {
            try
            {
                GetTemplateObject PostDataArrived = CS.GetPostData<GetTemplateObject>(this.Request);
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                TemplateWidget qw = new TemplateWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Delete(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

    }
}

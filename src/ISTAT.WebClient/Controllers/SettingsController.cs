using ISTAT.WebClient.Models;
using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using System.Configuration;

namespace ISTAT.WebClient.Controllers
{
    public class SettingsController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();

        private ConnectionStringSettings connectionStringSetting=ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
        
          

        public ActionResult SetLocale()
        {
            dynamic PostDataArrived = CS.GetPostData(this.Request);

            try
            {
                //LocaleResolver.RemoveCookie(HttpContext.ApplicationInstance.Context);

                CultureInfo c = Messages.SetLocale((string)PostDataArrived.locale);

                Thread.CurrentThread.CurrentUICulture = c;
                Thread.CurrentThread.CurrentCulture = c;

                LocaleResolver.SendCookie(c,HttpContext.ApplicationInstance.Context);
                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                query.CurrentCulture = c;
                
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(string.Empty));
            }
            catch (Exception)
            {
                return CS.ReturnForJQuery(ControllerSupport.ErrorOccured);
            }
        }

        public ActionResult GetMessages()
        {
            try
            {
                var ser = new JavaScriptSerializer();
                var messages = new Dictionary<string, string>();

                foreach (DictionaryEntry a in Messages.GetResourceSet(Thread.CurrentThread.CurrentCulture))
                    messages.Add(a.Key.ToString(), a.Value.ToString());

                return CS.ReturnForJQuery(messages);
            }
            catch (Exception ex)
            {

                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        

        public ActionResult GetSettings()
        {
            try
            {

                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                SettingsWidget qw = new SettingsWidget(connectionStringSetting.ConnectionString);

               return CS.ReturnForJQuery(qw.Load());
            }
            catch (Exception ex)
            {

                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }

        public ActionResult SetSettings()
        {
            try
            {
                GetEndpointSettings PostDataArrived = CS.GetPostData<GetEndpointSettings>(this.Request);

                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                SettingsWidget qw = new SettingsWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Save(PostDataArrived) ? JSONConst.Success : JSONConst.Error);
            }
            catch (Exception ex)
            {

                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }

        public ActionResult GetUserSettings()
        {
            try
            {

                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                GetUserSettingObject PostDataArrived = CS.GetPostData<GetUserSettingObject>(this.Request);
                SettingsWidget qw = new SettingsWidget(connectionStringSetting.ConnectionString);
                
                return CS.ReturnForJQuery(qw.LoadUserSetting(PostDataArrived.userCode));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }
  
        public ActionResult SetUserSettings()
        {
            try
            {
                GetUserSettingObject PostDataArrived = CS.GetPostData<GetUserSettingObject>(this.Request);

                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                SettingsWidget qw = new SettingsWidget(connectionStringSetting.ConnectionString);
                
                return CS.ReturnForJQuery(qw.SaveUserSetting(PostDataArrived) ? JSONConst.Success : JSONConst.Error);


            }
            catch (Exception ex)
            {

                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }




    }
}

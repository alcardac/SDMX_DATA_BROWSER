using ISTAT.WebClient.Models;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTAT.WebClient.Controllers
{
    public class QueryController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();

        public ActionResult Get()
        {
            try
            {
                GetQueryObject PostDataArrived = CS.GetPostData<GetQueryObject>(this.Request);
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                QueryWidget qw = new QueryWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Get(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

        public QueryObject GetSigleQuery(int queryId, string userCode)
        {
            try
            {
                GetQueryObject PostDataArrived = new GetQueryObject() 
                {
                    QueryId = queryId.ToString(),
                    UserCode = userCode
                };
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                QueryWidget qw = new QueryWidget(connectionStringSetting.ConnectionString);
                List<QueryObject> res = qw.Get(PostDataArrived);
                if (res != null && res.Count == 1)
                    return res[0];
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult Add()
        {
            try
            {
                GetQueryObject PostDataArrived = CS.GetPostData<GetQueryObject>(this.Request);
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                QueryWidget qw = new QueryWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Add(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }
        public ActionResult Del()
        {
            try
            {
                GetQueryObject PostDataArrived = CS.GetPostData<GetQueryObject>(this.Request);
                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                QueryWidget qw = new QueryWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Delete(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

    }
}

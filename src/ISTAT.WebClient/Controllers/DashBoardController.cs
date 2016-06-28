using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ISTAT.WebClient.Models;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;

namespace ISTAT.WebClient.Controllers
{
    public class DashBoardController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();

        private string ConnectionString
        {
            get
            {

                ConnectionStringSettings connectionStringSetting = new ConnectionStringSettings();

                if (ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"].ConnectionString.ToLower() != "file")
                {
                    connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];

                    if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                        throw new Exception("ConnectionString not set");
                }
                else
                { connectionStringSetting.ConnectionString = null; }

                return connectionStringSetting.ConnectionString;
            }
        }

        public ActionResult GetDashboards()
        {
            try
            {

                DashboardWidget qw = new DashboardWidget(ConnectionString);
                if (ConnectionString.ToLower() != "file")
                    return CS.ReturnForJQuery(qw.Load());
                else
                    return null;
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        public ActionResult GetDashboardsActive()
        {
            try
            {

                DashboardWidget qw = new DashboardWidget(ConnectionString);

                GetDashboardObject PostDataArrived = CS.GetPostData<GetDashboardObject>(this.Request);
                return CS.ReturnForJQuery(qw.Load(-1, true));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }
        public ActionResult PreviewDashboard()
        {
            try
            {
                GetDashboardObject PostDataArrived = CS.GetPostData<GetDashboardObject>(this.Request);
                List<GetDashboardObject> DashBoardObject;

                DashboardWidget qw = new DashboardWidget(ConnectionString);

                DashBoardObject = qw.Load(PostDataArrived.id);
                Session["DashBoard"] = DashBoardObject[0];

                return CS.ReturnForJQuery(DashBoardObject);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        public ActionResult CreateDashboard()
        {
            try
            {
                GetDashboardObject DashBoardObject = CS.GetPostData<GetDashboardObject>(this.Request);

                DashboardWidget dBWidget = new DashboardWidget(ConnectionString);

                DashBoardObject.id = dBWidget.CreateDashBoard(DashBoardObject);

                if (DashBoardObject.id == -1)
                    return CS.ReturnForJQuery(JSONConst.Error);

                Session["DashBoard"] = DashBoardObject;

                AddRow();

                return CS.ReturnForJQuery(DashBoardObject);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }

        }
        public ActionResult DeleteDashboard()
        {
            try
            {
                GetDashboardObject DashBoardObject = CS.GetPostData<GetDashboardObject>(this.Request);

                DashboardWidget dBWidget = new DashboardWidget(ConnectionString);

                //DashBoardObject.id = dBWidget.CreateDashBoard(DashBoardObject);

                if (DashBoardObject.id == -1)
                    return CS.ReturnForJQuery(JSONConst.Error);

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                DashboardWidget qw = new DashboardWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery((qw.Delete(DashBoardObject.id)) ? JSONConst.Success : JSONConst.Error);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }

        }
        public ActionResult CloseDashboard()
        {
            try
            {
                GetDashboardObject DashBoardObject = CS.GetPostData<GetDashboardObject>(this.Request);

                DashboardWidget dBWidget = new DashboardWidget(ConnectionString);

                //DashBoardObject.id = dBWidget.CreateDashBoard(DashBoardObject);

                if (DashBoardObject.id == -1)
                    return CS.ReturnForJQuery(JSONConst.Error);

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                DashboardWidget qw = new DashboardWidget(connectionStringSetting.ConnectionString);

                foreach (var dashRow in DashBoardObject.rows) {
                    if (dashRow.widgets.Count == 0) {
                        qw.DeleteRow(dashRow.id);
                    }
                }

                return CS.ReturnForJQuery(JSONConst.Success);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }

        }
         
        public ActionResult ActiveDashboard()
        {
            try
            {
                GetDashboardObject PostDataArrived = CS.GetPostData<GetDashboardObject>(this.Request);

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                DashboardWidget qw = new DashboardWidget(connectionStringSetting.ConnectionString);
                return CS.ReturnForJQuery(qw.Active(PostDataArrived.id, PostDataArrived.active));
            }
            catch (Exception ex)
            {

                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }

        public ActionResult AddRow()
        {
            DashboardRow DBRow;

            try
            {
                DashboardWidget qw = new DashboardWidget(ConnectionString);

                GetDashboardObject DashBoardObject = (GetDashboardObject)Session["DashBoard"];

                DBRow = qw.AddRow(DashBoardObject.id, false, -1);

                if (DashBoardObject.rows == null)
                    DashBoardObject.rows = new List<DashboardRow>();

                DashBoardObject.rows.Add(DBRow);

                Session["DashBoard"] = DashBoardObject;

                return CS.ReturnForJQuery(DBRow);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        public ActionResult UpdateRow()
        {
            try
            {
                DashboardWidget qw = new DashboardWidget(ConnectionString);
                DashboardRow UpdatedDashBoardRow;

                DashboardRow PostDataArrived = CS.GetPostData<DashboardRow>(this.Request);

                GetDashboardObject DashBoardObject = (GetDashboardObject)Session["DashBoard"];

                UpdatedDashBoardRow = qw.UpdateRow(PostDataArrived);

                var row_widget = DashBoardObject.rows.Find(c => c.id == PostDataArrived.id);

                UpdatedDashBoardRow.widgets = row_widget.widgets;

                DashBoardObject.rows.Remove(row_widget);
                DashBoardObject.rows.Add(UpdatedDashBoardRow);

                Session["DashBoard"] = DashBoardObject;

                return CS.ReturnForJQuery(UpdatedDashBoardRow);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        public ActionResult DeleteRow()
        {
            DashboardRow DBRow = CS.GetPostData<DashboardRow>(this.Request);

            try
            {

                DashboardWidget qw = new DashboardWidget(ConnectionString);

                GetDashboardObject DashBoardObject = (GetDashboardObject)Session["DashBoard"];

                qw.DeleteRow(DBRow.id);

                DashBoardObject.rows.Remove(DashBoardObject.rows.Find(c => c.id == DBRow.id));

                Session["DashBoard"] = DashBoardObject;

                return CS.ReturnForJQuery(DBRow);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }

        public ActionResult AddWidget()
        {
            WidgetObject woRet;

            try
            {
                DashboardWidget qw = new DashboardWidget(ConnectionString);

                GetDashboardObject DashBoardObject = (GetDashboardObject)Session["DashBoard"];
                WidgetObject PostDataArrived = CS.GetPostData<WidgetObject>(this.Request);

                woRet = qw.AddWidget(PostDataArrived);

                DashboardRow dbRow = DashBoardObject.rows.Find(r => r.id == woRet.rowID);

                if(dbRow.widgets == null)
                    dbRow.widgets = new List<WidgetObject>();

                dbRow.widgets.Add(woRet);

                Session["DashBoard"] = DashBoardObject;

                return CS.ReturnForJQuery(woRet);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        public ActionResult UpdateWidget()
        {
            WidgetObject ret;

            try
            {
                DashboardWidget qw = new DashboardWidget(ConnectionString);

                GetDashboardObject DashBoardObject = (GetDashboardObject)Session["DashBoard"];
                WidgetObject PostDataArrived = CS.GetPostData<WidgetObject>(this.Request);

                ret = qw.UpdateWidget(PostDataArrived);

                DashboardRow dRow = DashBoardObject.rows.Find(r => r.id == PostDataArrived.rowID);

                dRow.widgets.Remove(dRow.widgets.Find(w => w.id == PostDataArrived.id));
                dRow.widgets.Add(PostDataArrived);

                Session["DashBoard"] = DashBoardObject;

                return CS.ReturnForJQuery(ret);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }
        public ActionResult DeleteWidget()
        {
            bool ret = true;

            try
            {
                DashboardWidget qw = new DashboardWidget(ConnectionString);

                GetDashboardObject DashBoardObject = (GetDashboardObject)Session["DashBoard"];
                WidgetObject PostDataArrived = CS.GetPostData<WidgetObject>(this.Request);

                ret = qw.DeleteWidget(PostDataArrived.id);

                DashboardRow dRow = DashBoardObject.rows.Find(r => r.widgets.Find(w => w.id == PostDataArrived.id) != null);

                dRow.widgets.Remove(dRow.widgets.Find(w => w.id == PostDataArrived.id));

                Session["DashBoard"] = DashBoardObject;

                return CS.ReturnForJQuery(ret);

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(new ISTAT.WebClient.Models.ControllerSupport.StringResult() { Msg = ex.Message }));
            }
        }

        public ActionResult AddWidgetText(int wdgID, TextLocalised textLocalised)
        {
            throw new NotImplementedException();
        }

    }
}

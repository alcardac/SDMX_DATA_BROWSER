using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using log4net;

namespace ISTAT.WebClient.Caching
{
    internal class CachingManager
    {
        #region Props

        private Timer TimerThread_Dashboard { get; set; }

        private Timer TimerThread_Tree { get; set; }
        private SqlConnection Sqlconn { get; set; }
        private bool LockTreeRefresh = false;
        private bool LockWidgetRefresh = false;
        private string ConnectionString = Properties.Settings.Default.ConnectionString;

        private ILog Logger = LogManager.GetLogger("ISTAT.WebClientCaching");

        bool enableLog = Properties.Settings.Default.EnableCacheWidgetLog;

        #endregion

        #region Threads Methods

        public void Start()
        {

            if (enableLog)
            {
                Logger.Info("Application Start");
            }

            if (Properties.Settings.Default.EnableCacheWidget)
            {
                int pollingDashboard = Properties.Settings.Default.PollingCacheWidget;
                TimerThread_Dashboard = new Timer(new TimerCallback(RefreshWidget), null, 0, pollingDashboard * 1000);
            }

        }

        internal void Stop()
        {
            if (TimerThread_Tree != null) TimerThread_Tree.Dispose();
            if (TimerThread_Dashboard != null) TimerThread_Dashboard.Dispose();
        }

        #endregion

        #region Widget Methods

        private void RefreshWidget(object tState)
        {
            if (LockWidgetRefresh)
                return;
            try
            {
                int refreshEach = Properties.Settings.Default.RefreshCacheWidget;
                List<WidgetObject> lUpdateableWidgets;
                Utils.App_Data_Path = Path.GetTempPath();

                LockWidgetRefresh = true;

                string logMsgUpdated;

                CacheWidget widgetModel = new CacheWidget(ConnectionString);

                // Elimino i Widget scaduti
                widgetModel.DeleteExpiredWidget(refreshEach);
                if (enableLog)
                    Console.WriteLine("Deleted Expired Widget");

                // Prendo la lista dei Widget da inserire in cache
                lUpdateableWidgets = widgetModel.GetUpdateableWidgets();
                if (enableLog)
                    Console.WriteLine("Get The Updateable Widget: " + lUpdateableWidgets.Count.ToString());
                if (lUpdateableWidgets.Count <= 0)
                    return;

                foreach (WidgetObject widget in lUpdateableWidgets)
                {
                    foreach (TextLocalised locale in widget.text)
                    {
                        if (widget.type == "chart")
                        {
                            GetChartObject chartObject = new GetChartObject();

                            chartObject.Configuration = new EndpointSettings();
                            chartObject.Dataflow = new MaintenableObj();

                            chartObject.Configuration.Locale = locale.locale;
                            chartObject.Configuration.EndPoint = widget.endPoint;
                            chartObject.Configuration.EndPointV20 = widget.endPointV20;
                            chartObject.Configuration.EndPointType = widget.endPointType;
                            chartObject.Configuration.EndPointSource = widget.endPointSource;
                            chartObject.Configuration.DecimalSeparator = widget.endPointDecimalSeparator; 
                            chartObject.ObsValue = new List<string>();
                            if (widget.v) chartObject.ObsValue.Add("v");
                            if (widget.vc) chartObject.ObsValue.Add("vc");
                            if (widget.vt) chartObject.ObsValue.Add("vt");
                            chartObject.ChartType = widget.chartype;
                            chartObject.Dataflow.id = widget.dataflow_id;
                            chartObject.Dataflow.agency = widget.dataflow_agency_id;
                            chartObject.Dataflow.version = widget.dataflow_version;
                            chartObject.Criteria = (Dictionary<string, List<string>>)new JavaScriptSerializer().Deserialize(widget.criteria, typeof(Dictionary<string, List<string>>));

                            try
                            {

                                System.Globalization.CultureInfo cFrom=
                                    new System.Globalization.CultureInfo(
                                        (chartObject.Configuration.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");
                                
                                System.Globalization.CultureInfo cTo 
                                    = new System.Globalization.CultureInfo(
                                        (Properties.Settings.Default.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");

                                ChartWidget dataWidget = new ChartWidget(chartObject, null, cFrom, cTo);
                                SessionImplObject ret = dataWidget.GetDataChart();
                                widgetModel.InsertWidget(widget.id, ret.SavedChart, locale.locale);

                                if (enableLog)
                                {
                                    logMsgUpdated = string.Format("Caching Widget Info: Updated cache of Chart Widget with ID:{0} ", widget.id.ToString());
                                    Logger.Info(logMsgUpdated);
                                    Console.WriteLine(logMsgUpdated);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (ex.Message == "No results found")
                                {
                                    string message = "Caching Widget Error: No results found for widget with id: " + widget.id.ToString() + ". Check the configuration of the widget.";
                                    Logger.Error(message);
                                    Console.WriteLine(message);
                                }
                                else
                                {
                                    Logger.Error("Caching Widget Generic Error for widget with id: " + widget.id.ToString() + " -- " + ex.Message);
                                    Console.WriteLine("Caching Widget Generic Error for widget with id: " + widget.id.ToString() + " -- " + ex.Message);
                                }
                            }
                        }
                        else if (widget.type == "table")
                        {
                            GetDataObject dataObject = new GetDataObject();
                            dataObject.Configuration = new EndpointSettings();
                            dataObject.Dataflow = new MaintenableObj();

                            dataObject.Configuration.Locale = locale.locale;
                            dataObject.Configuration.EndPoint = widget.endPoint;
                            dataObject.Configuration.EndPointV20 = widget.endPointV20;
                            dataObject.Configuration.EndPointType = widget.endPointType;
                            dataObject.Configuration.EndPointSource = widget.endPointSource;
                            dataObject.Configuration.DecimalSeparator = widget.endPointDecimalSeparator; 
                            dataObject.Dataflow.id = widget.dataflow_id;
                            dataObject.Dataflow.agency = widget.dataflow_agency_id;
                            dataObject.Dataflow.version = widget.dataflow_version;
                            dataObject.Criteria = (Dictionary<string, List<string>>)new JavaScriptSerializer().Deserialize(widget.criteria, typeof(Dictionary<string, List<string>>));
                            dataObject.Layout = (LayoutObj)new JavaScriptSerializer().Deserialize(widget.layout, typeof(LayoutObj));

                            object DataStream = null;
                            try
                            {
                                DataWidget dWidget = new DataWidget(dataObject, null, Properties.Settings.Default.ParseSDMXAttributes);

                                SessionImplObject ret = dWidget.GetData(out DataStream);

                                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                                System.IO.TextWriter textWriter = new System.IO.StreamWriter(memoryStream);

                                         
                                System.Globalization.CultureInfo cFrom=
                                    new System.Globalization.CultureInfo(
                                        (dataObject.Configuration.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");
                                System.Globalization.CultureInfo cTo = 
                                    new System.Globalization.CultureInfo(
                                        (Properties.Settings.Default.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");

                                DataWidget.StreamDataTable(
                                    DataStream, 
                                    textWriter, 
                                    Properties.Settings.Default.ParseSDMXAttributes,
                                    cFrom,
                                    cTo);

                                textWriter.Flush();
                                byte[] bytesInStream = memoryStream.ToArray();
                                memoryStream.Close();

                                var htmlOutput = System.Text.Encoding.Default.GetString(bytesInStream);

                                widgetModel.InsertWidget(widget.id, htmlOutput, locale.locale);

                                if (enableLog)
                                {
                                    logMsgUpdated = string.Format("Caching Widget Info: Updated cache of Table Widget with ID:{0} ", widget.id.ToString());
                                    Logger.Info(logMsgUpdated);
                                    Console.WriteLine(logMsgUpdated);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (ex.Message == "No results found")
                                {
                                    string message = "Caching Widget Error: No results found for widget with id: " + widget.id.ToString() + ". Check the configuration of the widget.";
                                    Logger.Error(message);
                                    Console.WriteLine(message);
                                }
                                else
                                {
                                    Logger.Error("Caching Widget Generic Error for widget with id: " + widget.id.ToString() + " -- " + ex.Message);
                                    Console.WriteLine("Caching Widget Generic Error for widget with id: " + widget.id.ToString() + " -- " + ex.Message);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception exm)
            {
                Logger.Error("Master Error: "+ exm.Message);
                Console.WriteLine("Master Error: " + exm.Message + "STACKTRACE:"+ exm.StackTrace);
            }
            finally
            {
                LockWidgetRefresh = false;
            }
        }

        #endregion

        #region Tree Methods

        //private void RefreshTree(object tState)
        //{
        //    if (LockTreeRefresh)
        //        return;
        //    try
        //    {
        //        LockTreeRefresh = true;
        //        DateTime dtdel = DateTime.Now.AddHours(WebClientCachingSettings.Instance.DeleteCacheTree * -1);
        //        string DelSql = string.Format(@"DELETE from SavedTree WHERE (CAST(SUBSTRING ( LastRequest ,0 , 9 ) as int)<{0}) OR (CAST(SUBSTRING ( LastRequest ,0 , 9 ) as int) = {0} AND CAST(SUBSTRING ( LastRequest ,10 , 4 ) as int) <{1})", dtdel.ToString("yyyyMMdd"), dtdel.ToString("HHmm"));

        //        Sqlconn.Open();
        //        try
        //        {
        //            using (SqlCommand comm = new SqlCommand(DelSql, Sqlconn))
        //            {
        //                comm.ExecuteNonQuery();
        //            }
        //            string SqlTree = "SELECT * FROM SavedTree";
        //            DataTable dtres = new DataTable();
        //            using (SqlCommand commTree = new SqlCommand(SqlTree, Sqlconn))
        //            {
        //                using (SqlDataAdapter da = new SqlDataAdapter(commTree))
        //                {
        //                    da.Fill(dtres);
        //                }
        //            }
        //            List<string> upd = new List<string>();
        //            foreach (DataRow riga in dtres.Rows)
        //            {
        //                string idtree = riga["TreeId"].ToString();
        //                string Newtree = CallNewTree(riga["Configuration"].ToString());
        //                if (string.IsNullOrEmpty(Newtree)) continue;
        //                upd.Add(string.Format(@" UPDATE SavedTree SET SavedTreeJson='{0}', LastUpdate='{1}' WHERE TreeId={2}", Newtree, DateTime.Now.ToString("yyyyMMdd HHmm"), idtree));
        //            }
        //            foreach (var sqlUpd in upd)
        //            {
        //                using (SqlCommand commupd = new SqlCommand(sqlUpd, Sqlconn))
        //                    commupd.ExecuteNonQuery();
        //            }

        //        }
        //        catch (Exception) { throw; }
        //        finally
        //        {
        //            Sqlconn.Close();
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        LockTreeRefresh = false;
        //    }
        //}

        //private string CallNewTree(string Configuration)
        //{
        //    try
        //    {
        //        EndpointSettings set = Newtonsoft.Json.JsonConvert.DeserializeObject<EndpointSettings>(Configuration);
        //        GetTreeObject TreeObj = new GetTreeObject() { Configuration = set };
        //        TreeWidget tw = new TreeWidget(TreeObj, null);
        //        return tw.GetTreeforCache(TreeObj.Configuration.Locale);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        #endregion

    }
}

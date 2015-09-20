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
//using System.Web.Script.Serialization;
using System.IO;
using log4net;
using System.Web.Script.Serialization;
using log4net.Repository.Hierarchy;
using log4net.Core;
using System.Diagnostics;

namespace ISTAT.WebClientCachingService
{
    internal class CachingManager
    {
        #region Props

        enum LogType
        {
            Warning,
            Error,
            Info,
            Debug
        }

        private Timer TimerThread_Dashboard { get; set; }

        private Timer TimerThread_Tree { get; set; }
        private SqlConnection Sqlconn { get; set; }
        private bool LockWidgetRefresh = false;



        private ILog Logger;

        private string ConnectionString;// = ConfigurationManager.ConnectionStrings["ISTAT.WebClient.Caching.ConnectionString"].ConnectionString;
        private bool enableLog;// = bool.Parse(ConfigurationManager.AppSettings["EnableCacheWidgetLog"].ToString());
        private List<ConnectionStringSettings> lConnectionString = new List<ConnectionStringSettings>();
        

        #endregion

        #region Threads Methods

        public void Start()
        {
            foreach (ConnectionStringSettings conn in System.Configuration.ConfigurationManager.ConnectionStrings)
            {
                if (conn.Name.Substring(0, 1) == "_")
                    lConnectionString.Add(conn);
            }

            //Log4net
            string path = AppDomain.CurrentDomain.BaseDirectory + "log4net.xml";
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));
            Logger = LogManager.GetLogger("ISTAT.WebClientCaching");


            ////Windows Event log
            //System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
            //appLog.Source = "ISTATWebClientCachingService";
            //appLog.WriteEntry("An entry to the Application event log.");

            enableLog = bool.Parse(ConfigurationManager.AppSettings["EnableCacheWidgetLog"].ToString());

            if (enableLog)
            {
                WriteLog4netLine("Application Start", LogType.Info);
            }

            if (bool.Parse(ConfigurationManager.AppSettings["EnableCacheWidget"]))
            {
                int pollingDashboard = Int32.Parse(ConfigurationManager.AppSettings["PollingCacheWidget"]);
                TimerThread_Dashboard = new Timer(new TimerCallback(RefreshWidget), null, 0, pollingDashboard * 1000);
            }

        }

        internal void Stop()
        {
            if (enableLog)
            {
                WriteLog4netLine("Application End", LogType.Info);
            }

            if (TimerThread_Tree != null) TimerThread_Tree.Dispose();
            if (TimerThread_Dashboard != null) TimerThread_Dashboard.Dispose();
        }

        #endregion

        #region Widget Methods

        private void RefreshWidget(object tState)
        {
            if (LockWidgetRefresh)
            {
                return;
            }
            try
            {
                int refreshEach = Int32.Parse(ConfigurationManager.AppSettings["RefreshCacheWidget"]);
                List<WidgetObject> lUpdateableWidgets;
                Utils.App_Data_Path = Path.GetTempPath();
                WidgetInfo wdgInfo;
                string logMsgUpdated;

                LockWidgetRefresh = true;

                foreach (ConnectionStringSettings conn in lConnectionString)
                {
                    ConnectionString = conn.ConnectionString;

                    if (enableLog)
                        WriteLog4netLine("--- ELABORATION START FOR CONNECTION: " + conn.Name, LogType.Info);

                    CacheWidget widgetModel = new CacheWidget(ConnectionString);

                    // Elimino i Widget scaduti
                    widgetModel.DeleteExpiredWidget(refreshEach);
                    if (enableLog)
                        WriteLog4netLine("Deleted Expired Widget", LogType.Info);

                    // Prendo la lista dei Widget da inserire in cache
                    lUpdateableWidgets = widgetModel.GetUpdateableWidgets();
                    if (enableLog)
                        WriteLog4netLine("Get The Updateable Widget: " + lUpdateableWidgets.Count.ToString(), LogType.Info);

                    if (lUpdateableWidgets.Count <= 0)
                        return;

                    foreach (WidgetObject widget in lUpdateableWidgets)
                    {
                        foreach (TextLocalised locale in widget.text)
                        {
                            wdgInfo = widgetModel.GetWidgetInfo(locale.locale, widget.id);

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

                                    System.Globalization.CultureInfo cFrom =
                                        new System.Globalization.CultureInfo(
                                            (chartObject.Configuration.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");

                                    System.Globalization.CultureInfo cTo
                                        = new System.Globalization.CultureInfo(
                                            (ConfigurationManager.AppSettings["DecimalSeparator"].ToString().Trim() == ".") ? "EN" : "IT");

                                    ChartWidget dataWidget = new ChartWidget(chartObject, null, cFrom, cTo);
                                    SessionImplObject ret = dataWidget.GetDataChart();
                                    widgetModel.InsertWidget(widget.id, ret.SavedChart, locale.locale);

                                    if (enableLog)
                                    {
                                        logMsgUpdated = string.Format("Updated cache of Chart Widget with ID:{0} -dsb title:{1} -wdg title:{2} ", widget.id.ToString(), wdgInfo.dsbTitle, wdgInfo.wdgTitle);
                                        WriteLog4netLine(logMsgUpdated, LogType.Info);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == "No results found")
                                    {
                                        string message = string.Format("No results found for widget with ID:{0} -dsb title:{1} -wdg title:{2} . Check the configuration of the widget.", widget.id.ToString(), wdgInfo.dsbTitle, wdgInfo.wdgTitle);
                                        WriteLog4netLine(message, LogType.Error);
                                    }
                                    else
                                    {
                                        WriteLog4netLine(string.Format("Generic Error for widget with ID:{0} -dsb title:{1} -wdg title:{2} -- {3}", widget.id.ToString(), wdgInfo.dsbTitle, wdgInfo.wdgTitle, ex.Message), LogType.Error);
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
                                    DataWidget dWidget = new DataWidget(dataObject, null, bool.Parse(ConfigurationManager.AppSettings["ParseSDMXAttributes"]));

                                    SessionImplObject ret = dWidget.GetData(out DataStream);

                                    System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                                    System.IO.TextWriter textWriter = new System.IO.StreamWriter(memoryStream);


                                    System.Globalization.CultureInfo cFrom =
                                        new System.Globalization.CultureInfo(
                                            (dataObject.Configuration.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");
                                    System.Globalization.CultureInfo cTo =
                                        new System.Globalization.CultureInfo(
                                            (ConfigurationManager.AppSettings["DecimalSeparator"].ToString().Trim() == ".") ? "EN" : "IT");

                                    DataWidget.StreamDataTable(
                                        DataStream,
                                        textWriter,
                                        bool.Parse(ConfigurationManager.AppSettings["ParseSDMXAttributes"]),
                                        cFrom,
                                        cTo);

                                    textWriter.Flush();
                                    byte[] bytesInStream = memoryStream.ToArray();
                                    memoryStream.Close();

                                    var htmlOutput = System.Text.Encoding.Default.GetString(bytesInStream);

                                    widgetModel.InsertWidget(widget.id, htmlOutput, locale.locale);

                                    if (enableLog)
                                        WriteLog4netLine(string.Format("Updated cache of Table Widget with ID:{0} -dsb title:{1} -wdg title:{2} ", widget.id.ToString(), wdgInfo.dsbTitle, wdgInfo.wdgTitle), LogType.Info);

                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == "No results found")
                                    {
                                        string message = string.Format("No results found for widget with ID:{0} -dsb title:{1} -wdg title:{2}. Check the configuration of the widget.", widget.id.ToString(), wdgInfo.dsbTitle, wdgInfo.wdgTitle);
                                        WriteLog4netLine(message, LogType.Error);
                                    }
                                    else
                                    {
                                        WriteLog4netLine(string.Format("Generic Error for widget with ID:{0} -dsb title:{1} -wdg title:{2} -- {3}", widget.id.ToString(), wdgInfo.dsbTitle, wdgInfo.wdgTitle, ex.Message), LogType.Error);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exm)
            {
                WriteLog4netLine("Master Error: " + exm.Message, LogType.Error);
            }
            finally
            {
                LockWidgetRefresh = false;
            }
        }

        #endregion

        #region Private Methods

        private void WriteLog4netLine(string message, LogType logType)
        {
            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = Level.Info;

            switch (logType)
            {
                case LogType.Warning:
                    Logger.Warn(message);
                    break;
                case LogType.Error:
                    Logger.Error(message);
                    break;
                case LogType.Info:
                    Logger.Info(message);
                    break;
                case LogType.Debug:
                    Logger.Debug(message);
                    break;
            }

            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = Level.Off;
        }

        #endregion

    }
}

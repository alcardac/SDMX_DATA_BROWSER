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
using System.Web.Script.Serialization;
using System.IO;
using log4net;

namespace ISTAT.WebClient.CacheManager.Manager
{
    internal class ThreadBackgroundManager
    {
        #region Props

        private Timer TimerThread_Dashboard { get; set; }

        private Timer TimerThread_Tree { get; set; }
        private SqlConnection Sqlconn { get; set; }
        private bool LockTreeRefresh = false;
        private bool LockWidgetRefresh = false;
        private ConnectionStringSettings connectionStringSetting;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ThreadBackgroundManager));

        #endregion

        #region Threads Methods 

        internal void Start()
        {
            connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
            if (connectionStringSetting == null  || string.IsNullOrEmpty(connectionStringSetting.ConnectionString)
                || connectionStringSetting.ConnectionString.ToString().ToLower()=="file")
                return;
            Sqlconn = new SqlConnection(connectionStringSetting.ConnectionString);


            if (WebClientSettings.Instance.EnableCacheTree)
            {
                int refreshEach = WebClientSettings.Instance.RefreshCacheTree;//MilliSecond
                TimerThread_Tree = new Timer(new TimerCallback(RefreshTree), null, 0, refreshEach);
            }
        }

        internal void Stop()
        {
            if (TimerThread_Tree != null) TimerThread_Tree.Dispose();
            if (TimerThread_Dashboard != null) TimerThread_Dashboard.Dispose();
        }

        #endregion 

        #region Tree Methods

        private void RefreshTree(object tState)
        {
            if (LockTreeRefresh)
                return;
            try
            {
                LockTreeRefresh = true;
                DateTime dtdel = DateTime.Now.AddHours(WebClientSettings.Instance.DeleteCacheTree * -1);
                string DelSql = string.Format(@"DELETE from SavedTree WHERE (CAST(SUBSTRING ( LastRequest ,0 , 9 ) as int)<{0}) OR (CAST(SUBSTRING ( LastRequest ,0 , 9 ) as int) = {0} AND CAST(SUBSTRING ( LastRequest ,10 , 4 ) as int) <{1})", dtdel.ToString("yyyyMMdd"), dtdel.ToString("HHmm"));

                Sqlconn.Open();
                try
                {
                    using (SqlCommand comm = new SqlCommand(DelSql, Sqlconn))
                    {
                        comm.ExecuteNonQuery();
                    }
                    string SqlTree = "SELECT * FROM SavedTree";
                    DataTable dtres = new DataTable();
                    using (SqlCommand commTree = new SqlCommand(SqlTree, Sqlconn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(commTree))
                        {
                            da.Fill(dtres);
                        }
                    }
                    List<string> upd = new List<string>();
                    foreach (DataRow riga in dtres.Rows)
                    {
                        string idtree = riga["TreeId"].ToString();
                        string Newtree = CallNewTree(riga["Configuration"].ToString());
                        if (string.IsNullOrEmpty(Newtree)) continue;
                        upd.Add(string.Format(@" UPDATE SavedTree SET SavedTreeJson='{0}', LastUpdate='{1}' WHERE TreeId={2}", Newtree, DateTime.Now.ToString("yyyyMMdd HHmm"), idtree));
                    }
                    foreach (var sqlUpd in upd)
                    {
                        using (SqlCommand commupd = new SqlCommand(sqlUpd, Sqlconn))
                            commupd.ExecuteNonQuery();
                    }

                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                LockTreeRefresh = false;
            }
        }

        private string CallNewTree(string Configuration)
        {
            try
            {
                EndpointSettings set = Newtonsoft.Json.JsonConvert.DeserializeObject<EndpointSettings>(Configuration);
                GetTreeObject TreeObj = new GetTreeObject() { Configuration = set };
                TreeWidget tw = new TreeWidget(TreeObj, null);
                return tw.GetTreeforCache(TreeObj.Configuration.Locale);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion 

    }
}

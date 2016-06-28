using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

using log4net;

using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class SettingsWidget
    {
        public SqlConnection Sqlconn { get; set; }
        private const string ErrorOccured = "{\"error\" : true }";
        private const string ErrorOccuredMess = "{\"error\" : true, \"message\" : {0} }";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryWidget));

        public SettingsWidget(string connectionString)
        {
            if (connectionString.ToLower() == "file")
            { Sqlconn = null; }
            else
            { Sqlconn = new SqlConnection(connectionString); }
        }

        public GetEndpointSettings Load()
        {
            GetEndpointSettings ret = null;
            if (Sqlconn != null)
            { ret = LoadDB(); }
            else
            { ret = LoadFile(); }
            return ret;
        }

        public GetEndpointSettings LoadFile()
        {

            GetEndpointSettings ret = new GetEndpointSettings();
            ret.settings = new Settings();
            ret.settings.view_tree = true;
            ret.settings.view_tree_req = true;
            ret.settings.view_tree_select = true;
            ret.settings.view_login = false;

            EndpointSettings endpointSetting = new EndpointSettings();
            ret.endpoints = new List<EndpointSettings>();
            foreach (EndPointElement endPointEl in IRConfiguration.Config.EndPoints)
            {
                endpointSetting = new EndpointSettings();
                endpointSetting.Locale = endPointEl.Locale;
                endpointSetting.IDNode = endPointEl.IDNode;
                endpointSetting.Title = endPointEl.Title;
                endpointSetting.DecimalSeparator = endPointEl.DecimalSeparator;
                endpointSetting.Domain = endPointEl.Domain;
                endpointSetting.EnableHTTPAuthentication = endPointEl.EnableHTTPAuthentication;
                endpointSetting.EnableProxy = endPointEl.EnableProxy;
                endpointSetting.EndPoint = endPointEl.EndPoint;
                endpointSetting.EndPointV20 = endPointEl.EndPointV20;
                endpointSetting.EndPointType = endPointEl.EndPointType;
                endpointSetting.EndPointSource = endPointEl.EndPointSource;
                endpointSetting.Password = endPointEl.Password;
                endpointSetting.Prefix = endPointEl.Prefix;
                endpointSetting.ProxyPassword = endPointEl.ProxyPassword;
                endpointSetting.ProxyServer = endPointEl.ProxyServer;
                endpointSetting.ProxyServerPort = endPointEl.ProxyServerPort;
                endpointSetting.ProxyUserName = endPointEl.ProxyUserName;
                endpointSetting.UseSystemProxy = endPointEl.UseSystemProxy;
                endpointSetting.UserName = endPointEl.UserName;
                endpointSetting.Wsdl = endPointEl.Wsdl;
                endpointSetting.Active = endPointEl.Active;
                endpointSetting.UseUncategorysed = endPointEl.UseUncategorysed;
                endpointSetting.UseVirtualDf = endPointEl.UseVirtualDf;
                endpointSetting.Ordinamento = endPointEl.Ordinamento;
                //endpointSetting._TypeEndpoint = endPointEl._TypeEndpoint;
                //ret.endpoints.Add(new EndpointSettings(endpointSetting));
                ret.endpoints.Add(endpointSetting);
            }
            
            return ret;
        }

        public GetEndpointSettings LoadDB()
        {

            GetEndpointSettings ret = null;

            try
            {
                Sqlconn.Open();

                string sqlquery;

                sqlquery = string.Format(@"SELECT * FROM [Settings]");
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    IDataReader resAdd = comm.ExecuteReader();
                    if (resAdd.Read()) ret = (GetEndpointSettings)new JavaScriptSerializer().Deserialize(resAdd.GetString(resAdd.GetOrdinal("Setting")), typeof(GetEndpointSettings));
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
            finally
            {
                Sqlconn.Close();
            }
            return ret;
        }
        public bool Save(GetEndpointSettings PostDataArrived)
        {
            bool success = false;
            try
            {
                if (PostDataArrived == null) throw new Exception("Input Error");

                Sqlconn.Open();

                string sqlquery;

                sqlquery = string.Format(@"DELETE FROM [Settings]");
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    int resAdd = comm.ExecuteNonQuery();
                    
                }

                sqlquery = string.Format(@"INSERT INTO [Settings] ([Setting]) VALUES('{0}')",new JavaScriptSerializer().Serialize(PostDataArrived).Replace("'", "''"));
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    int resAdd = comm.ExecuteNonQuery();
                    if (resAdd == 0)
                    {
                        success = false;
                        throw new Exception("Setting not insert");
                    }
                }

                success=true;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
            finally
            {
                Sqlconn.Close();
            }
            return success;
        }

        public UserSettingResponseObject LoadUserSetting(string userCode)
        {
            UserSettingResponseObject ret = null;
            
            if (Sqlconn != null)
            { ret = LoadUserSettingDB(userCode); }
            else
            { ret = LoadUserSettingFile(userCode); }
            
            return ret;
        }

        public UserSettingResponseObject LoadUserSettingFile(string userCode)
        {

               UserSettingResponseObject ret = new UserSettingResponseObject();
               if (ConfigurationManager.AppSettings["main_fontFamily"]!=null)
               { ret.main_fontFamily = ConfigurationManager.AppSettings["main_fontFamily"].ToString(); }
               else
               { ret.main_fontFamily = "Arial"; }

               if (ConfigurationManager.AppSettings["main_fontSize"] != null)
               { ret.main_fontSize = ConfigurationManager.AppSettings["main_fontSize"].ToString(); }
               else
               { ret.main_fontSize = "12"; }

               if (ConfigurationManager.AppSettings["main_containerWidth"] != null)
               { ret.main_containerWidth = ConfigurationManager.AppSettings["main_containerWidth"].ToString(); }
               else
               { ret.main_containerWidth = "100%"; }

               if (ConfigurationManager.AppSettings["main_css"] != null)
               { ret.main_css = ConfigurationManager.AppSettings["main_css"].ToString(); }
               else
               { ret.main_css = "Content/style/custom/sistan.css"; }

            return ret;
        }
 
        public UserSettingResponseObject LoadUserSettingDB(string userCode) {

            UserSettingResponseObject ret = null;

            try
            {
                Sqlconn.Open();

                string sqlquery;

                sqlquery = string.Format(@"SELECT * FROM [UserSettings] WHERE [UserCode]='" + userCode + "'");
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    using (IDataReader resAdd = comm.ExecuteReader())
                    {
                        if (resAdd.Read())
                        {
                            ret = (UserSettingResponseObject)new JavaScriptSerializer().Deserialize(resAdd.GetString(resAdd.GetOrdinal("Settings")), typeof(UserSettingResponseObject));
                        }
                    }
                }
                if (ret == null) {
                    sqlquery = string.Format(@"SELECT * FROM [UserSettings] WHERE [UserCode]='DEFAULT_SETT'");
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        using (IDataReader resAdd = comm.ExecuteReader())
                        {
                            if (resAdd.Read())
                            {
                                ret = (UserSettingResponseObject)new JavaScriptSerializer().Deserialize(resAdd.GetString(resAdd.GetOrdinal("Settings")), typeof(UserSettingResponseObject));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
            finally
            {
                Sqlconn.Close();
            }
            return ret;
        }
        public bool SaveUserSetting(GetUserSettingObject PostDataArrived)
        {
            bool success = false;
            try
            {
                if (PostDataArrived == null) throw new Exception("Input Error");

                Sqlconn.Open();

                string sqlquery;
                sqlquery = string.Format(@"DELETE FROM [UserSettings] WHERE [UserCode]='" + PostDataArrived.userCode + "'");
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    int resAdd = comm.ExecuteNonQuery();
                }

                sqlquery = string.Format(@"INSERT INTO [UserSettings] ([UserCode],[Settings]) VALUES('{0}','{1}')",PostDataArrived.userCode, new JavaScriptSerializer().Serialize(PostDataArrived.setting).Replace("'", "''"));
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    int resAdd = comm.ExecuteNonQuery();
                    if (resAdd == 0)
                    {
                        success = false;
                        throw new Exception("User Setting not insert");
                    }
                }
                success = true;


            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
            finally
            {
                Sqlconn.Close();

                if (PostDataArrived.userRole == 1)
                {
                    success = SaveDefaultSetting(PostDataArrived);
                }
            }
            return success;
        }

        public bool SaveDefaultSetting(GetUserSettingObject PostDataArrived)
        {
            bool success = false;
            try
            {
                if (PostDataArrived == null) throw new Exception("Input Error");

                Sqlconn.Open();

                string sqlquery;
                sqlquery = string.Format(@"DELETE FROM [UserSettings] WHERE [UserCode]='DEFAULT_SETT'");
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    int resAdd = comm.ExecuteNonQuery();
                }

                sqlquery = string.Format(@"INSERT INTO [UserSettings] ([UserCode],[Settings]) VALUES('DEFAULT_SETT','{0}')", new JavaScriptSerializer().Serialize(PostDataArrived.setting).Replace("'", "''"));
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    int resAdd = comm.ExecuteNonQuery();
                    if (resAdd == 0)
                    {
                        success = false;
                        throw new Exception("User Setting not insert");
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
            finally
            {
                Sqlconn.Close();
            }
            return success;
        }

    }
}

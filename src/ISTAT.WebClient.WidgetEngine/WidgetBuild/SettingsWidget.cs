using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

using log4net;

using ISTAT.WebClient.WidgetComplements.Model;
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
            Sqlconn = new SqlConnection(connectionString);
        }

        public GetEndpointSettings Load()
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

        public UserSettingResponseObject LoadUserSetting(string userCode) {

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

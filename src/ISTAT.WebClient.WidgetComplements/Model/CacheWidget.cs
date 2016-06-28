using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using log4net;

namespace ISTAT.WebClient.WidgetComplements.Model
{
    public class CacheWidget
    {
        public SqlConnection Sqlconn { get; set; }

        public CacheWidget(string connectionString)
        {
            if (connectionString != null && connectionString.ToLower() != "file")
            { Sqlconn = new SqlConnection(connectionString); }
            else
            { Sqlconn = null; }
        }

        public WidgetInfo GetWidgetInfo(string locale, int widgetID)
        {
            WidgetInfo wdgInfo = new WidgetInfo();
            SqlDataReader oReader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.GetWidgetInfo";

                    SqlParameter pLocale = new SqlParameter("@LOCALE", SqlDbType.VarChar, 5);
                    pLocale.Value = locale;
                    oComm.Parameters.Add(pLocale);

                    SqlParameter pWidgetID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWidgetID.Value = widgetID;
                    oComm.Parameters.Add(pWidgetID);

                    oReader = oComm.ExecuteReader();

                    if (oReader.Read())
                    {
                        wdgInfo.dsbTitle = oReader["dsb_text_Title"].ToString();
                        wdgInfo.wdgTitle = oReader["wdg_text_Title"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }

            return wdgInfo;

        }

        public void DeleteExpiredWidget(int secondsToExpire)
        {

            try
            {
                using (SqlCommand oComm = new SqlCommand())
                {
                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.DeleteExpiredWidget";

                    SqlParameter pSeconds = new SqlParameter("@SECONDS", SqlDbType.Int);
                    pSeconds.Value = secondsToExpire;
                    oComm.Parameters.Add(pSeconds);

                    oComm.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }
        }

        public List<WidgetObject> GetUpdateableWidgets()
        {

            List<WidgetObject> lWidgets = new List<WidgetObject>();
            SqlDataReader oReader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.GetUpdateableWidgets";

                    oReader = oComm.ExecuteReader();

                    while (oReader.Read())
                    {
                        WidgetObject widget = new WidgetObject()
                        {
                            id = (int)oReader["wdg_id"],
                            cssClass = oReader["wdg_class"] != DBNull.Value ? oReader["wdg_class"].ToString() : string.Empty,
                            rowID = (int)oReader["wdg_row_id"],
                            cell = (int)oReader["wdg_cell"],
                            type = oReader["wdg_type"] != DBNull.Value ? oReader["wdg_type"].ToString() : string.Empty,
                            chartype = oReader["wdg_chartType"] != DBNull.Value ? oReader["wdg_chartType"].ToString() : string.Empty,
                            v = oReader["wdg_v"] != DBNull.Value ? (bool)oReader["wdg_v"] : false,
                            vt = oReader["wdg_vt"] != DBNull.Value ? (bool)oReader["wdg_vt"] : false,
                            vc = oReader["wdg_vc"] != DBNull.Value ? (bool)oReader["wdg_vc"] : false,
                            endPoint = oReader["wdg_endPoint"] != DBNull.Value ? oReader["wdg_endPoint"].ToString() : string.Empty,
                            endPointType = oReader["wdg_endPointType"] != DBNull.Value ? oReader["wdg_endPointType"].ToString() : string.Empty,
                            endPointV20 = oReader["wdg_endPointV20"] != DBNull.Value ? oReader["wdg_endPointV20"].ToString() : string.Empty,
                            endPointDecimalSeparator = oReader["wdg_decimalCulture"] != DBNull.Value ? oReader["wdg_decimalCulture"].ToString() : string.Empty,
                            endPointSource = oReader["wdg_endpointSource"] != DBNull.Value ? oReader["wdg_endpointSource"].ToString() : string.Empty,
                            dataflow_id = oReader["wdg_dataflow_id"] != DBNull.Value ? oReader["wdg_dataflow_id"].ToString() : string.Empty,
                            dataflow_agency_id = oReader["wdg_dataflow_agency_id"] != DBNull.Value ? oReader["wdg_dataflow_agency_id"].ToString() : string.Empty,
                            dataflow_version = oReader["wdg_dataflow_version"] != DBNull.Value ? oReader["wdg_dataflow_version"].ToString() : string.Empty,
                            criteria = oReader["wdg_criteria"] != DBNull.Value ? oReader["wdg_criteria"].ToString() : string.Empty,
                            layout = oReader["wdg_layout"] != DBNull.Value ? oReader["wdg_layout"].ToString() : string.Empty
                        };

                        widget.text = new List<TextLocalised>() { new TextLocalised() { locale = oReader["wdg_text_Locale"].ToString() } };

                        lWidgets.Add(widget);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }

            return lWidgets;
        }

        public void InsertWidget(int widgetID, string widgetData, string locale)
        {
            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.InsertWidget";

                    SqlParameter pWidgetID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWidgetID.Value = widgetID;
                    oComm.Parameters.Add(pWidgetID);

                    SqlParameter pWidgetData = new SqlParameter("@WIDGETDATA", SqlDbType.NText);
                    pWidgetData.Value = widgetData;
                    oComm.Parameters.Add(pWidgetData);

                    SqlParameter pLocale = new SqlParameter("@LOCALE", SqlDbType.VarChar, 5);
                    pLocale.Value = locale;
                    oComm.Parameters.Add(pLocale);

                    oComm.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }
        }

        public void DeleteWidget(int widgetID)
        {
            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.DeleteWidget";

                    SqlParameter pWidgetID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWidgetID.Value = widgetID;
                    oComm.Parameters.Add(pWidgetID);

                    oComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }
        }

        public void ClearWidgetCache()
        {
            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.ClearWidgetCache";

                    oComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }
        }

        public SavedWidget GetWidget(int widgetID, string locale)
        {
            SavedWidget savedWidget = null;
            SqlDataReader reader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.GetWidget";

                    SqlParameter pWidgetID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWidgetID.Value = widgetID;
                    oComm.Parameters.Add(pWidgetID);

                    SqlParameter pLocale = new SqlParameter("@LOCALE", SqlDbType.VarChar);
                    pLocale.Value = locale;
                    oComm.Parameters.Add(pLocale);

                    reader = oComm.ExecuteReader();

                    if (reader.Read())
                    {
                        savedWidget = new SavedWidget();
                        savedWidget.savedWidgetID = (int)reader["SWID"];
                        savedWidget.widgetID = (int)reader["wdg_id"];
                        savedWidget.widgetData = reader["WidgetData"].ToString();
                        savedWidget.locale = reader["Locale"].ToString();
                        savedWidget.dtUpdate = Convert.ToDateTime(reader["DTUpdate"]);
                    }

                }

                return savedWidget;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }
        }

        public bool IsCachedWidget(int widgetID, string locale)
        {
            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "Caching.IsCachedWidget";

                    SqlParameter pWidgetID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWidgetID.Value = widgetID;
                    oComm.Parameters.Add(pWidgetID);

                    SqlParameter pLocale = new SqlParameter("@LOCALE", SqlDbType.VarChar);
                    pLocale.Value = locale;
                    oComm.Parameters.Add(pLocale);

                    SqlParameter pExist = new SqlParameter("@EXIST", SqlDbType.Bit);
                    pExist.Direction = ParameterDirection.Output;
                    oComm.Parameters.Add(pExist);

                    oComm.ExecuteNonQuery();

                    return (bool)pExist.Value;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }
        }
    }
}

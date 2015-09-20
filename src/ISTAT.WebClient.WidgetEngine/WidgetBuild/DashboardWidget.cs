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
    public class DashboardWidget
    {
        public SqlConnection Sqlconn { get; set; }
        private const string ErrorOccured = "{\"error\" : true }";
        private const string ErrorOccuredMess = "{\"error\" : true, \"message\" : {0} }";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryWidget));

        public DashboardWidget(string connectionString)
        {
            Sqlconn = new SqlConnection(connectionString);
        }

        #region Method

        #region Dashboard Methods

        public List<GetDashboardObject> Load(int dashboardId = -1, bool onlyActive = false)
        {
            List<GetDashboardObject> lDashBoard = null;
            try
            {
                int? dbID;
                bool? active;

                if (dashboardId == -1)
                    dbID = null;
                else
                    dbID = dashboardId;

                if (onlyActive)
                    active = true;
                else
                    active = null;

                lDashBoard = GetDashBoardsInfo(dbID, active);

                foreach (GetDashboardObject db in lDashBoard)
                {
                    db.text = GetDashBoardTexts(db.id);
                    db.rows = GetDashBoardRows(db.id);
                    foreach (DashboardRow row in db.rows)
                    {
                        row.widgets = GetDashBoardWidget(row.id);
                        foreach (WidgetObject widget in row.widgets)
                        {
                            widget.text = GetDashBoardWidgetText(widget.id);
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
            return lDashBoard;
        }
        
        public int CreateDashBoard(GetDashboardObject PostDataArrived)
        {
            int idDBRet;
            string locales = "", values = "";

            foreach (TextLocalised txt in PostDataArrived.text)
            {
                locales += txt.locale + "|";
                values += txt.title + "|";
            }

            locales = locales.Substring(0, locales.Length - 1);
            values = values.Substring(0, values.Length - 1);

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.Create";

                    SqlParameter pUserCode = new SqlParameter("@USERCODE", SqlDbType.NVarChar, 255);
                    pUserCode.Value = PostDataArrived.usercode;
                    oComm.Parameters.Add(pUserCode);

                    SqlParameter pRowNum = new SqlParameter("@ROWNUM", SqlDbType.Int);
                    pRowNum.Value = PostDataArrived.numrow;
                    oComm.Parameters.Add(pRowNum);

                    SqlParameter pLocales = new SqlParameter("@LOCALES", SqlDbType.VarChar, 2000);
                    pLocales.Value = locales;
                    oComm.Parameters.Add(pLocales);

                    SqlParameter pValues = new SqlParameter("@VALUES", SqlDbType.VarChar, 5000);
                    pValues.Value = values;
                    oComm.Parameters.Add(pValues);

                    SqlParameter pDashBoardID = new SqlParameter("@IDDASHBOARD", SqlDbType.Int);
                    pDashBoardID.Direction = ParameterDirection.Output;
                    oComm.Parameters.Add(pDashBoardID);

                    oComm.ExecuteNonQuery();

                    idDBRet = (int)pDashBoardID.Value;
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

            return idDBRet;
        }
        
        public List<GetDashboardObject> GetDashBoardsInfo(int? dashboardId, bool? active)
        {

            List<GetDashboardObject> lDBObjects = new List<GetDashboardObject>();
            SqlDataReader oReader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.GetDashBoardInfo";

                    SqlParameter pDashBoardID = new SqlParameter("@IDDASHBOARD", SqlDbType.Int);
                    pDashBoardID.Value = dashboardId;
                    oComm.Parameters.Add(pDashBoardID);

                    SqlParameter pActive = new SqlParameter("@ACTIVE", SqlDbType.Bit);
                    pActive.Value = active;
                    oComm.Parameters.Add(pActive);

                    oReader = oComm.ExecuteReader();

                    while (oReader.Read())
                    {
                        GetDashboardObject dbObject = new GetDashboardObject()
                        {
                            id = (int)oReader["dsb_id"] ,
                            usercode = oReader["dsb_userCode"].ToString() ,
                            numrow = (int)oReader["dsb_rowsNum"] ,
                            date = oReader["dsb_date"] != DBNull.Value ? oReader["dsb_date"].ToString() : string.Empty,
                            active = (bool)oReader["dsb_active"]	
                        };

                        lDBObjects.Add(dbObject);
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

            return lDBObjects;
        }

        public List<TextLocalised> GetDashBoardTexts(int dashboardId)
        {

            List<TextLocalised> lTexts = new List<TextLocalised>();
            SqlDataReader oReader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.GetDashBoardText";

                    SqlParameter pDashBoardID = new SqlParameter("@IDDASHBOARD", SqlDbType.Int);
                    pDashBoardID.Value = dashboardId;
                    oComm.Parameters.Add(pDashBoardID);

                    oReader = oComm.ExecuteReader();

                    while (oReader.Read())
                    {
                        TextLocalised textsLocalised = new TextLocalised()
                        {
                            title = oReader["dsb_text_Title"] != DBNull.Value ? oReader["dsb_text_Title"].ToString() : string.Empty,
                            locale = oReader["dsb_text_Locale"] != DBNull.Value ? oReader["dsb_text_Locale"].ToString() : string.Empty,  
                        };

                        lTexts.Add(textsLocalised);
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

            return lTexts;
        }

        public bool Delete(int dashboardId)
        {

            bool success = true;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.Delete";

                    SqlParameter pDashBoardID = new SqlParameter("@IDDASHBOARD", SqlDbType.Int);
                    pDashBoardID.Value = dashboardId;
                    oComm.Parameters.Add(pDashBoardID);

                    oComm.ExecuteNonQuery();

                }

            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }

            return success;
        }

        public bool Active(int dashboardId, bool enable)
        {

            bool success = true;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.Activate";

                    SqlParameter pDashBoardID = new SqlParameter("@IDDASHBOARD", SqlDbType.Int);
                    pDashBoardID.Value = dashboardId;
                    oComm.Parameters.Add(pDashBoardID);

                    SqlParameter pActive = new SqlParameter("@ACTIVE", SqlDbType.Bit);
                    pActive.Value = enable;
                    oComm.Parameters.Add(pActive);

                    oComm.ExecuteNonQuery();

                }

            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }

            return success;
        }

        #endregion

        #region Rows Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <param name="splitted"></param>
        /// <param name="order">if -1 = next order</param>
        public DashboardRow AddRow(int dashboardId, bool split, int order)
        {
            DashboardRow DBRowRet;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.AddRow";

                    SqlParameter pDSBID = new SqlParameter("@DSBID", SqlDbType.Int);
                    pDSBID.Value = dashboardId;
                    oComm.Parameters.Add(pDSBID);

                    SqlParameter pSplitted = new SqlParameter("@SPLITTED", SqlDbType.Bit);
                    pSplitted.Value = split;
                    oComm.Parameters.Add(pSplitted);

                    SqlParameter pOrder = new SqlParameter("@ORDER", SqlDbType.Int);
                    pOrder.Direction = ParameterDirection.InputOutput;
                    pOrder.Value = order;
                    oComm.Parameters.Add(pOrder);

                    SqlParameter pRowID = new SqlParameter("@ROWID", SqlDbType.Int);
                    pRowID.Direction = ParameterDirection.Output;
                    oComm.Parameters.Add(pRowID);

                    oComm.ExecuteNonQuery();

                    DBRowRet = new DashboardRow()
                    {
                        id = (int)pRowID.Value,
                        order = (int)pOrder.Value,
                        splitted = split,
                        widgets = null
                    };
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

            return DBRowRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dRow"></param>
        /// <returns></returns>
        public DashboardRow UpdateRow(DashboardRow dRow)
        {
            DashboardRow DBRowRet;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.UpdateRow";

                    SqlParameter pSplitted = new SqlParameter("@SPLITTED", SqlDbType.Bit);
                    pSplitted.Value = dRow.splitted;
                    oComm.Parameters.Add(pSplitted);

                    SqlParameter pOrder = new SqlParameter("@ORDER", SqlDbType.Int);
                    pOrder.Direction = ParameterDirection.InputOutput;
                    pOrder.Value = dRow.order;
                    oComm.Parameters.Add(pOrder);

                    SqlParameter pRowID = new SqlParameter("@ROWID", SqlDbType.Int);
                    pRowID.Value = dRow.id;
                    oComm.Parameters.Add(pRowID);

                    oComm.ExecuteNonQuery();

                    DBRowRet = new DashboardRow()
                    {
                        id = dRow.id,
                        order = (int)pOrder.Value,
                        splitted = dRow.splitted,
                        widgets = dRow.widgets
                    };
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

            return DBRowRet;
        }

        public bool DeleteRow(int rowId)
        {
            bool ret = true;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.DeleteRow";

                    SqlParameter pRowID = new SqlParameter("@ROWID", SqlDbType.Int);
                    pRowID.Direction = ParameterDirection.Input;
                    pRowID.Value = rowId;
                    oComm.Parameters.Add(pRowID);

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

            return ret;
        }

        public List<DashboardRow> GetDashBoardRows(int dashboardId)
        {

            List<DashboardRow> lRows = new List<DashboardRow>();
            SqlDataReader oReader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.GetDashBoardRow";

                    SqlParameter pDashBoardID = new SqlParameter("@IDDASHBOARD", SqlDbType.Int);
                    pDashBoardID.Value = dashboardId;
                    oComm.Parameters.Add(pDashBoardID);

                    oReader = oComm.ExecuteReader();

                    while (oReader.Read())
                    {
                        DashboardRow dbRow = new DashboardRow()
                        {
                            id = (int)oReader["dsb_row_id"],
                            splitted = (bool)oReader["splitted"],
                            order = (int)oReader["order"]
                        };

                        lRows.Add(dbRow);
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

            return lRows;
        }

        #endregion

        #region Widget Methods

        public WidgetObject AddWidget(WidgetObject widgetObject)
        {
            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.AddWidget";

                    SqlParameter pWdgClass = new SqlParameter("@WDG_CLASS", SqlDbType.NVarChar);
                    pWdgClass.Value = widgetObject.cssClass;
                    oComm.Parameters.Add(pWdgClass);

                    SqlParameter pRowID = new SqlParameter("@WDG_ROW_ID", SqlDbType.Int);
                    pRowID.Value = widgetObject.rowID;
                    oComm.Parameters.Add(pRowID);

                    SqlParameter pWdgCell = new SqlParameter("@WDG_CELL", SqlDbType.Int);
                    pWdgCell.Value = widgetObject.cell;
                    oComm.Parameters.Add(pWdgCell);

                    SqlParameter pType = new SqlParameter("@WDG_TYPE", SqlDbType.NVarChar);
                    pType.Value = widgetObject.type;
                    oComm.Parameters.Add(pType);

                    SqlParameter pChartType = new SqlParameter("@WDG_CHARTTYPE", SqlDbType.NVarChar);
                    pChartType.Value = widgetObject.chartype;
                    oComm.Parameters.Add(pChartType);

                    SqlParameter pV = new SqlParameter("@WDG_V", SqlDbType.Bit);
                    pV.Value = widgetObject.v;
                    oComm.Parameters.Add(pV);

                    SqlParameter pVT = new SqlParameter("@WDG_VT", SqlDbType.Bit);
                    pVT.Value = widgetObject.vt;
                    oComm.Parameters.Add(pVT);

                    SqlParameter pVC = new SqlParameter("@WDG_VC", SqlDbType.Bit);
                    pVC.Value = widgetObject.vc;
                    oComm.Parameters.Add(pVC);

                    SqlParameter pEndPoint = new SqlParameter("@WDG_ENDPOINT", SqlDbType.NVarChar);
                    pEndPoint.Value = widgetObject.endPoint;
                    oComm.Parameters.Add(pEndPoint);

                    SqlParameter pEndPointType = new SqlParameter("@WDG_ENDPOINTTYPE", SqlDbType.NVarChar);
                    pEndPointType.Value = widgetObject.endPointType;
                    oComm.Parameters.Add(pEndPointType);

                    SqlParameter pEndPointV20 = new SqlParameter("@WDG_ENDPOINTV20", SqlDbType.NVarChar);
                    pEndPointV20.Value = widgetObject.endPointV20;
                    oComm.Parameters.Add(pEndPointV20);

                    SqlParameter pEndPointSource = new SqlParameter("@WDG_ENDPOINTSOURCE", SqlDbType.NVarChar);
                    pEndPointSource.Value = widgetObject.endPointSource;
                    oComm.Parameters.Add(pEndPointSource);

                    SqlParameter pEndPointDecimal = new SqlParameter("@WDG_ENDPOINTDECIMAL", SqlDbType.NVarChar);
                    pEndPointDecimal.Value = widgetObject.endPointDecimalSeparator;
                    oComm.Parameters.Add(pEndPointDecimal);

                    SqlParameter pDataFlowID = new SqlParameter("@WDG_DATAFLOW_ID", SqlDbType.NVarChar);
                    pDataFlowID.Value = widgetObject.dataflow_id;
                    oComm.Parameters.Add(pDataFlowID);

                    SqlParameter pAgencyID = new SqlParameter("@WDG_DATAFLOW_AGENCY_ID", SqlDbType.NVarChar);
                    pAgencyID.Value = widgetObject.dataflow_agency_id;
                    oComm.Parameters.Add(pAgencyID);

                    SqlParameter pVersion = new SqlParameter("@WDG_DATAFLOW_VERSION", SqlDbType.NVarChar);
                    pVersion.Value = widgetObject.dataflow_version;
                    oComm.Parameters.Add(pVersion);

                    SqlParameter pCriteria = new SqlParameter("@WDG_CRITERIA", SqlDbType.NText);
                    pCriteria.Value = widgetObject.criteria;
                    oComm.Parameters.Add(pCriteria);

                    SqlParameter pLayout = new SqlParameter("@WDG_LAYOUT", SqlDbType.NText);
                    pLayout.Value = widgetObject.layout;
                    oComm.Parameters.Add(pLayout);

                    SqlParameter pWDGID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWDGID.Direction = ParameterDirection.Output;
                    oComm.Parameters.Add(pWDGID);

                    oComm.ExecuteNonQuery();

                    widgetObject.id = (int)pWDGID.Value;

                    foreach (TextLocalised tl in widgetObject.text)
                    {
                        AddWidgetText(widgetObject.id, tl);
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

            return widgetObject;
        }

        public WidgetObject UpdateWidget(WidgetObject widgetObject)
        {
            bool ret = true;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.UpdateWidget";

                    SqlParameter pWDGID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWDGID.Value = widgetObject.id;
                    oComm.Parameters.Add(pWDGID);

                    SqlParameter pWdgClass = new SqlParameter("@WDG_CLASS", SqlDbType.NVarChar);
                    pWdgClass.Value = widgetObject.cssClass;
                    oComm.Parameters.Add(pWdgClass);

                    SqlParameter pRowID = new SqlParameter("@WDG_ROW_ID", SqlDbType.Int);
                    pRowID.Value = widgetObject.rowID;
                    oComm.Parameters.Add(pRowID);

                    SqlParameter pWdgCell = new SqlParameter("@WDG_CELL", SqlDbType.Int);
                    pWdgCell.Value = widgetObject.cell;
                    oComm.Parameters.Add(pWdgCell);

                    SqlParameter pType = new SqlParameter("@WDG_TYPE", SqlDbType.NVarChar);
                    pType.Value = widgetObject.type;
                    oComm.Parameters.Add(pType);

                    SqlParameter pChartType = new SqlParameter("@WDG_CHARTTYPE", SqlDbType.NVarChar);
                    pChartType.Value = widgetObject.chartype;
                    oComm.Parameters.Add(pChartType);

                    SqlParameter pV = new SqlParameter("@WDG_V", SqlDbType.Bit);
                    pV.Value = widgetObject.v;
                    oComm.Parameters.Add(pV);

                    SqlParameter pVT = new SqlParameter("@WDG_VT", SqlDbType.Bit);
                    pVT.Value = widgetObject.vt;
                    oComm.Parameters.Add(pVT);

                    SqlParameter pVC = new SqlParameter("@WDG_VC", SqlDbType.Bit);
                    pVC.Value = widgetObject.vc;
                    oComm.Parameters.Add(pVC);

                    SqlParameter pEndPoint = new SqlParameter("@WDG_ENDPOINT", SqlDbType.NVarChar);
                    pEndPoint.Value = widgetObject.endPoint;
                    oComm.Parameters.Add(pEndPoint);

                    SqlParameter pEndPointType = new SqlParameter("@WDG_ENDPOINTTYPE", SqlDbType.NVarChar);
                    pEndPointType.Value = widgetObject.endPointType;
                    oComm.Parameters.Add(pEndPointType);

                    SqlParameter pEndPointV20 = new SqlParameter("@WDG_ENDPOINTV20", SqlDbType.NVarChar);
                    pEndPointV20.Value = widgetObject.endPointV20;
                    oComm.Parameters.Add(pEndPointV20);


                    SqlParameter pEndPointSource = new SqlParameter("@WDG_ENDPOINTSOURCE", SqlDbType.NVarChar);
                    pEndPointSource.Value = widgetObject.endPointSource;
                    oComm.Parameters.Add(pEndPointSource);

                    SqlParameter pEndPointDecimal = new SqlParameter("@WDG_ENDPOINTDECIMAL", SqlDbType.NVarChar);
                    pEndPointDecimal.Value = widgetObject.endPointDecimalSeparator;
                    oComm.Parameters.Add(pEndPointDecimal);

                    SqlParameter pDataFlowID = new SqlParameter("@WDG_DATAFLOW_ID", SqlDbType.NVarChar);
                    pDataFlowID.Value = widgetObject.dataflow_id;
                    oComm.Parameters.Add(pDataFlowID);

                    SqlParameter pAgencyID = new SqlParameter("@WDG_DATAFLOW_AGENCY_ID", SqlDbType.NVarChar);
                    pAgencyID.Value = widgetObject.dataflow_agency_id;
                    oComm.Parameters.Add(pAgencyID);

                    SqlParameter pVersion = new SqlParameter("@WDG_DATAFLOW_VERSION", SqlDbType.NVarChar);
                    pVersion.Value = widgetObject.dataflow_version;
                    oComm.Parameters.Add(pVersion);

                    SqlParameter pCriteria = new SqlParameter("@WDG_CRITERIA", SqlDbType.NText);
                    pCriteria.Value = widgetObject.criteria;
                    oComm.Parameters.Add(pCriteria);

                    SqlParameter pLayout = new SqlParameter("@WDG_LAYOUT", SqlDbType.NText);
                    pLayout.Value = widgetObject.layout;
                    oComm.Parameters.Add(pLayout);

                    oComm.ExecuteNonQuery();

                    foreach (TextLocalised tl in widgetObject.text)
                    {
                        UpdateWidgetText(widgetObject.id, tl);
                    }

                }

            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }

            return widgetObject;
        }

        public bool DeleteWidget(int widgetID)
        {
            bool ret = true;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.DeleteWidget";

                    SqlParameter pWidgetID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWidgetID.Direction = ParameterDirection.Input;
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

            return ret;
        }

        public List<WidgetObject> GetDashBoardWidget(int rowId)
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
                    oComm.CommandText = "DashBoard.GetDashBoardWidget";

                    SqlParameter pRowID = new SqlParameter("@IDROW", SqlDbType.Int);
                    pRowID.Value = rowId;
                    oComm.Parameters.Add(pRowID);

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
                            endPointSource = oReader["wdg_endPointSource"] != DBNull.Value ? oReader["wdg_endPointSource"].ToString() : string.Empty,
                            endPointDecimalSeparator = oReader["wdg_decimalCulture"] != DBNull.Value ? oReader["wdg_decimalCulture"].ToString() : string.Empty,
                            dataflow_id = oReader["wdg_dataflow_id"] != DBNull.Value ? oReader["wdg_dataflow_id"].ToString() : string.Empty,
                            dataflow_agency_id = oReader["wdg_dataflow_agency_id"] != DBNull.Value ? oReader["wdg_dataflow_agency_id"].ToString() : string.Empty,
                            dataflow_version = oReader["wdg_dataflow_version"] != DBNull.Value ? oReader["wdg_dataflow_version"].ToString() : string.Empty,
                            criteria = oReader["wdg_criteria"] != DBNull.Value ? oReader["wdg_criteria"].ToString() : string.Empty,
                            layout = oReader["wdg_layout"] != DBNull.Value ? oReader["wdg_layout"].ToString() : string.Empty
                        };

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

        #endregion

        #region WidgetText Methods

        public int AddWidgetText(int wdgID, TextLocalised textLocalised)
        {
            int wdgTextID;

            try
            {
                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.AddWidgetText";

                    SqlParameter pWdgID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWdgID.Value = wdgID;
                    oComm.Parameters.Add(pWdgID);

                    SqlParameter pTitle = new SqlParameter("@WDG_TEXT_TITLE", SqlDbType.NVarChar);
                    pTitle.Value = textLocalised.title;
                    oComm.Parameters.Add(pTitle);

                    SqlParameter pContent = new SqlParameter("@WDG_TEXT_CONTENT", SqlDbType.NText);
                    pContent.Value = textLocalised.content;
                    oComm.Parameters.Add(pContent);

                    SqlParameter pLocale = new SqlParameter("@WDG_TEXT_LOCALE", SqlDbType.NVarChar);
                    pLocale.Value = textLocalised.locale;
                    oComm.Parameters.Add(pLocale);

                    SqlParameter pWDGTextID = new SqlParameter("@WDG_TEXT_ID", SqlDbType.Int);
                    pWDGTextID.Direction = ParameterDirection.Output;
                    oComm.Parameters.Add(pWDGTextID);

                    oComm.ExecuteNonQuery();

                    wdgTextID = (int)pWDGTextID.Value;
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

            return wdgTextID;
        }

        public bool UpdateWidgetText(int wdgID, TextLocalised textLocalised)
        {
            bool ret = true;

            try
            {
                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.UpdateWidgetText";


                    SqlParameter pWdgID = new SqlParameter("@WDG_ID", SqlDbType.Int);
                    pWdgID.Value = wdgID;
                    oComm.Parameters.Add(pWdgID);

                    SqlParameter pLocale = new SqlParameter("@WDG_TEXT_LOCALE", SqlDbType.NVarChar);
                    pLocale.Value = textLocalised.locale;
                    oComm.Parameters.Add(pLocale); 
                    
                    SqlParameter pTitle = new SqlParameter("@WDG_TEXT_TITLE", SqlDbType.NVarChar);
                    pTitle.Value = textLocalised.title;
                    oComm.Parameters.Add(pTitle);

                    SqlParameter pContent = new SqlParameter("@WDG_TEXT_CONTENT", SqlDbType.NText);
                    pContent.Value = textLocalised.content;
                    oComm.Parameters.Add(pContent);

                    oComm.ExecuteNonQuery();

                }

            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }
            finally
            {
                if (Sqlconn.State == ConnectionState.Open)
                    Sqlconn.Close();
            }

            return ret;
        }

        public bool DeleteWidgetText(int widgetTextID)
        {
            bool ret = true;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();
                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.DeleteWidgetText";

                    SqlParameter pWidgetTextID = new SqlParameter("@WDG_TEXT_ID", SqlDbType.Int);
                    pWidgetTextID.Direction = ParameterDirection.Input;
                    pWidgetTextID.Value = widgetTextID;
                    oComm.Parameters.Add(pWidgetTextID);

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

            return ret;
        }

        public List<TextLocalised> GetDashBoardWidgetText(int wdgId)
        {

            List<TextLocalised> lWidgetTexts = new List<TextLocalised>();
            SqlDataReader oReader;

            try
            {

                using (SqlCommand oComm = new SqlCommand())
                {

                    if (Sqlconn.State == ConnectionState.Closed)
                        Sqlconn.Open();

                    oComm.Connection = Sqlconn;
                    oComm.CommandType = CommandType.StoredProcedure;
                    oComm.CommandText = "DashBoard.GetDashBoardWidgetText";

                    SqlParameter pWdgID = new SqlParameter("@WDGID", SqlDbType.Int);
                    pWdgID.Value = wdgId;
                    oComm.Parameters.Add(pWdgID);

                    oReader = oComm.ExecuteReader();

                    while (oReader.Read())
                    {
                        TextLocalised widgetText = new TextLocalised()
                        {
                            content = oReader["wdg_text_Content"] != DBNull.Value ? oReader["wdg_text_Content"].ToString() : string.Empty,
                            title = oReader["wdg_text_Title"] != DBNull.Value ? oReader["wdg_text_Title"].ToString() : string.Empty,
                            locale = oReader["wdg_text_Locale"] != DBNull.Value ? oReader["wdg_text_Locale"].ToString() : string.Empty
                        };

                        lWidgetTexts.Add(widgetText);
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

            return lWidgetTexts;
        }      

        #endregion

        #endregion

    }
}

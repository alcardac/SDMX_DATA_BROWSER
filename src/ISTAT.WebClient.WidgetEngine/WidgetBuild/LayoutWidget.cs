using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using ISTAT.WebClient.WidgetEngine.Model;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class LayoutWidget
    {
        private GetCodemapObject LayObj { get; set; }
        private SessionImplObject SessionObj { get; set; }
        private IGetSDMX GetSDMXObject = null;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CodemapWidget));
        private const string ErrorOccured = "{\"error\" : true }";

        public LayoutWidget(GetCodemapObject layoutObj, SessionImplObject sessionObj)
        {
            LayObj = layoutObj;
            SessionObj = sessionObj;
            GetSDMXObject = WebServiceSelector.GetSdmxImplementation(this.LayObj.Configuration);
        }

        

        public SessionImplObject GetLayout()
        {
            try
            {
                ISdmxObjects structure = GetKeyFamily();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null || df == null)
                    throw new InvalidOperationException("DataStructure is not set");

                if (this.SessionObj.DafaultLayout == null)
                    this.SessionObj.DafaultLayout = new Dictionary<string, LayoutObj>();

                if (!this.SessionObj.DafaultLayout.ContainsKey(Utils.MakeKey(df)))
                {
                    LayoutObj deflay = GetDefaultLayout(df, kf);
                    this.SessionObj.DafaultLayout[Utils.MakeKey(df)] = deflay;
                }

                DefaultLayoutResponseObject defaultLayoutResponseObject = new DefaultLayoutResponseObject();
                defaultLayoutResponseObject.DefaultLayout = this.SessionObj.DafaultLayout[Utils.MakeKey(df)];
                this.SessionObj.SavedDefaultLayout = new JavaScriptSerializer().Serialize(defaultLayoutResponseObject);

                return this.SessionObj;
            }
            catch (InvalidOperationException ex)
            {
                Logger.Warn(Resources.ErrorMaxJsonLength);
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn(Resources.ErrorRecursionLimit);
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
        }

        public SessionImplObject GetLayout(ConnectionStringSettings connectionStringSetting)
        {
            try
            {
                ISdmxObjects structure = GetKeyFamily();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null || df == null)
                    throw new InvalidOperationException("DataStructure is not set");

                //if (this.SessionObj.DafaultLayout == null)
                this.SessionObj.DafaultLayout = new Dictionary<string, LayoutObj>();

                // Get automatic timeserie layout
                System.Data.SqlClient.SqlConnection Sqlconn = new System.Data.SqlClient.SqlConnection(connectionStringSetting.ConnectionString);
                Sqlconn.Open();
                string sqlquery = string.Format("Select * from Template where [tmplKey]='{0}'",
                    new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    LayObj.Dataflow.id + "+" + LayObj.Dataflow.agency + "+" + LayObj.Dataflow.version + "+" + LayObj.Configuration.EndPoint).Replace("'", "''"));
                using (System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand(sqlquery, Sqlconn))
                {
                    var reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        string layout = reader.GetString(reader.GetOrdinal("Layout"));
                        this.SessionObj.DafaultLayout[Utils.MakeKey(df)] =
                            (LayoutObj)new JavaScriptSerializer().Deserialize(layout, typeof(LayoutObj));

                        this.SessionObj.DafaultLayout[Utils.MakeKey(df)].block_axis_x = reader.GetBoolean(reader.GetOrdinal("BlockXAxe"));
                        this.SessionObj.DafaultLayout[Utils.MakeKey(df)].block_axis_y = reader.GetBoolean(reader.GetOrdinal("BlockYAxe"));
                        this.SessionObj.DafaultLayout[Utils.MakeKey(df)].block_axis_z = reader.GetBoolean(reader.GetOrdinal("BlockZAxe"));


                    }
                }
                Sqlconn.Close();

                DefaultLayoutResponseObject defaultLayoutResponseObject = new DefaultLayoutResponseObject();
                defaultLayoutResponseObject.DefaultLayout = (this.SessionObj.DafaultLayout.ContainsKey(Utils.MakeKey(df))) ? this.SessionObj.DafaultLayout[Utils.MakeKey(df)] : null;

                //if (defaultLayoutResponseObject.DefaultLayout == null){ return GetLayout(); }
                
                this.SessionObj.SavedDefaultLayout = new JavaScriptSerializer().Serialize(defaultLayoutResponseObject);
                
                return this.SessionObj;
            }
            catch (InvalidOperationException ex)
            {
                Logger.Warn(Resources.ErrorMaxJsonLength);
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn(Resources.ErrorRecursionLimit);
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(ErrorOccured);
            }
        }


        private static LayoutObj FindInConfigFile(IDataflowObject df, IDataStructureObject kf)
        {
            return null;
        }

        public static LayoutObj GetDefaultLayout(IDataflowObject df, IDataStructureObject kf)
        {

            LayoutObj lay = FindInConfigFile(df, kf);
            if (lay != null)
                return lay;



            lay = new LayoutObj();
            foreach (var item in kf.DimensionList.Dimensions)
            {
                if (item.TimeDimension)
                    lay.axis_y.Add(item.Id);
                else if (item.FrequencyDimension)
                    lay.axis_z.Add(item.Id);
                else
                    lay.axis_x.Add(item.Id);
            }
            return lay;
        }



        private ISdmxObjects GetKeyFamily()
        {
            if (this.SessionObj == null)
            {
                this.SessionObj = new SessionImplObject();
                this.SessionObj.SdmxObject = new SdmxObjectsImpl();
            }
            //
            IDataflowObject dataflow = this.SessionObj.SdmxObject.Dataflows.FirstOrDefault(d =>
                d.AgencyId == this.LayObj.Dataflow.agency && d.Id == this.LayObj.Dataflow.id && d.Version == this.LayObj.Dataflow.version);

            ISdmxObjects Structure = null;
            if (dataflow != null)
            {
                Structure = GetSDMXObject.GetStructure(dataflow, this.SessionObj.SdmxObject.DataStructures);
                Structure.AddDataflow(dataflow);
            }
            else
            {
                Structure = GetSDMXObject.GetStructure(this.LayObj.Dataflow.id, this.LayObj.Dataflow.agency, this.LayObj.Dataflow.version);
            }
            this.SessionObj.SdmxObject.Merge(Structure);



            return Structure;
        }
    }
}

using Estat.Sdmxsource.Extension.Constant;
using Estat.Sri.CustomRequests.Constants;
using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.DataRender;
using ISTAT.WebClient.WidgetComplements.Model.Enum;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetEngine.Builder.Tree;
using ISTAT.WebClient.WidgetEngine.Model;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using ISTAT.WebClient.WidgetEngine.Model.DataRender;
using ISTAT.WebClient.WidgetEngine.Model.DBData;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Sdmx.DataParser.Engine;
using Org.Sdmxsource.Sdmx.DataParser.Engine.Reader;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Util.Objects;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class DataWidget
    {
        private GetDataObject DataObj { get; set; }
        private SessionImplObject SessionObj { get; set; }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(TreeWidget));
        private const string ErrorOccured = "{\"error\" : true }";

        private IGetSDMX GetSDMXObject = null;
        private CodemapWidget codemapWidget = null;

        private BaseDataObject BDO { get; set; }

        private bool _useAttr { get; set; }

        public DataWidget(GetDataObject dataObj, SessionImplObject sessionObj, bool useAttr)
        {
            DataObj = dataObj;
            SessionObj = sessionObj;
            GetSDMXObject = WebServiceSelector.GetSdmxImplementation(this.DataObj.Configuration);
            BDO = new BaseDataObject(dataObj.Configuration, System.IO.Path.GetTempFileName());

            this._useAttr = useAttr;
        }

        public SessionImplObject GetData(out object DataStream)
        {
            try
            {
                // Init session objects
                if (this.SessionObj == null)
                {
                    this.SessionObj = new SessionImplObject();
                    this.SessionObj.SdmxObject = new SdmxObjectsImpl();
                }


                if (BDO == null || GetSDMXObject == null)
                    throw new Exception(Messages.label_error_network + " " + DataObj.Configuration.Title);


                codemapWidget = new CodemapWidget(new GetCodemapObject() { 
                    Configuration = this.DataObj.Configuration, 
                    Dataflow = this.DataObj.Dataflow,
                    PreviusCostraint=this.DataObj.Criteria },
                    this.SessionObj);
                ISdmxObjects structure = codemapWidget.GetDsd();
                IDataflowObject df = structure.Dataflows.FirstOrDefault();
                IDataStructureObject kf = structure.DataStructures.First();
                if (df == null) throw new InvalidOperationException("Dataflow is not set");

                /****************/
                // Get all codelist
                /****************/
                Dictionary<string, ICodelistObject> ConceptCodelists = codemapWidget.GetCodelistMap(df, kf, true);
                ComponentCodeDescriptionDictionary codemap = new ComponentCodeDescriptionDictionary();
                foreach (string ConceptId in ConceptCodelists.Keys)
                {
                    ICodelistObject codelist = ConceptCodelists[ConceptId];
                    Dictionary<string, string> codes = new Dictionary<string, string>();
                    foreach (ICode codeItem in codelist.Items)
                        codes.Add(codeItem.Id, TextTypeHelper.GetText(codeItem.Names, this.DataObj.Configuration.Locale));
                    codemap.Add(ConceptId, codes);
                }
                /****************/

                this.SessionObj.MergeObject(codemapWidget.SessionObj);

                #region Gestione last period
                if (this.DataObj.Criteria.ContainsKey(kf.TimeDimension.Id)
                 && this.DataObj.Criteria[kf.TimeDimension.Id].Count == 1
                 && !this.DataObj.Layout.axis_z.Contains(kf.TimeDimension.Id))
                {
                    int offsetTime = int.Parse(this.DataObj.Criteria[kf.TimeDimension.Id].First());
                    var codMap = codemap;
                    int lengthTime = codMap[kf.TimeDimension.Id].Count;

                    if ((lengthTime - offsetTime) >= 0)
                    {
                        var codes = codMap[kf.TimeDimension.Id].Reverse().Take(offsetTime);
                        List<string> _criteriaTime = (from c in codes select c.Key).ToList<string>();

                        this.DataObj.Criteria[kf.TimeDimension.Id] = new List<string>();
                        this.DataObj.Criteria[kf.TimeDimension.Id].Add(_criteriaTime.Last());
                        this.DataObj.Criteria[kf.TimeDimension.Id].Add(_criteriaTime.First());
                    }
                    else
                    {
                        this.DataObj.Criteria[kf.TimeDimension.Id] = new List<string>();
                        this.DataObj.Criteria[kf.TimeDimension.Id].Add(codemap[kf.TimeDimension.Id].First().Key);
                        this.DataObj.Criteria[kf.TimeDimension.Id].Add(codemap[kf.TimeDimension.Id].Last().Key);
                    }
                }
                #endregion

                LayoutObj layObj = InitLayout(df, kf);
                List<DataCriteria> Criterias = BDO.InitCriteria(kf, this.DataObj.Criteria);

                Dictionary<string, List<DataChacheObject>> DataCache = SessionObj.DataCache;
                // try get from cache 
                //IDataSetStore store = BDO.FindDataCache(df, kf, Criterias, ref DataCache);
                //if (store == null) store = BDO.GetDataset(df, kf, Criterias, ref DataCache);

                IDataSetStore store = BDO.GetDataset(df, kf, Criterias, ref DataCache, _useAttr);

                SessionObj.DataCache = DataCache;

                DataStream = new DataObjectForStreaming()
                {
                    Configuration = this.DataObj.Configuration,
                    store = store,
                    layObj = layObj,
                    Criterias = Criterias,
                    structure = structure,
                    codemap = codemap
                };


                return this.SessionObj;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw ex;
            }

        }

        public static void StreamDataTable(object DataObjectsForStreaming, TextWriter streamResponse, bool useAttr,CultureInfo cFrom,CultureInfo cTo)
        {
            if (DataObjectsForStreaming == null || !(DataObjectsForStreaming is DataObjectForStreaming))
                throw new Exception("Data not found");

            DataObjectForStreaming DataStream = (DataObjectForStreaming)DataObjectsForStreaming;
            
            #region CheckLayout
            DataStream.layObj.axis_x.ForEach(axisX => { if (!DataStream.store.ExistsColumn(axisX)) DataStream.layObj.axis_x.Remove(axisX); });
            DataStream.layObj.axis_y.ForEach(axisY => { if (!DataStream.store.ExistsColumn(axisY)) DataStream.layObj.axis_y.Remove(axisY); });
            DataStream.layObj.axis_z.ForEach(axisZ => { if (!DataStream.store.ExistsColumn(axisZ)) DataStream.layObj.axis_z.Remove(axisZ); });
            #endregion

            DataRender rea = new DataRender(
                DataStream.store,
                DataStream.Criterias,
                DataStream.layObj,
                DataStream.structure,
                DataStream.codemap,
                useAttr, 
                cFrom,cTo);
            rea.render(streamResponse);

            //DataStream.store.Dispose();
        }

        private LayoutObj InitLayout(IDataflowObject df, IDataStructureObject kf)
        {
            if (this.DataObj.Layout == null)
                return LayoutWidget.GetDefaultLayout(df, kf);
            return this.DataObj.Layout;
        }

        #region Download

        private static void GetModel(IDataSetRenderer renderer, IDataSetModel datasetModel, TextWriter streamResponse)
        {
            renderer.RenderAllTables(datasetModel, streamResponse);
        }
        private static void GetModel(IDataSetRenderer renderer, IDataSetModel datasetModel, MemoryStream memoryStream)
        {
            renderer.RenderAllTables(datasetModel, memoryStream);
        }

        private static IDataSetModel GetDataSetModel(DataObjectForStreaming dataStream)
        {
            IDataSetModel dataSetModel;

            dataStream.layObj.axis_x.ForEach(axisX => { if (!dataStream.store.ExistsColumn(axisX)) dataStream.layObj.axis_x.Remove(axisX); });
            dataStream.layObj.axis_y.ForEach(axisY => { if (!dataStream.store.ExistsColumn(axisY)) dataStream.layObj.axis_y.Remove(axisY); });
            dataStream.layObj.axis_z.ForEach(axisZ => { if (!dataStream.store.ExistsColumn(axisZ)) dataStream.layObj.axis_z.Remove(axisZ); });

            dataSetModel = new DataSetModelStore(dataStream.structure, dataStream.store);

            dataSetModel.Initialize();
            dataSetModel.UpdateAxis(dataStream.layObj.axis_z, dataStream.layObj.axis_x, dataStream.layObj.axis_y);

            return dataSetModel;
        }

        public static void GetDownloadStream(IDataSetRenderer renderer, DataObjectForStreaming dataStream, TextWriter streamResponse)
        {
            GetModel(renderer, GetDataSetModel(dataStream), streamResponse);
        }
        public static void GetDownloadStream(IDataSetRenderer renderer, DataObjectForStreaming dataStream, MemoryStream memoryStream)
        {
            GetModel(renderer, GetDataSetModel(dataStream), memoryStream);
        }

        public static void GetDataSetStream(IDataSetRenderer renderer, DataObjectForStreaming dataStream, TextWriter streamResponse)
        {
            //EndpointSettings DataObjConfiguration = dataStream.Configuration;

            //IDataStructureObject kf = dataStream.structure.DataStructures.First();

            ////DataObjectForStreaming
            //SDMXWSFunction op = SDMXWSFunction.GetCompactData;
            ////DataObjConfiguration

            //bool cross = (DataObjConfiguration._TypeEndpoint == EndpointType.V21 || DataObjConfiguration._TypeEndpoint == EndpointType.REST)
            //              ? NsiClientHelper.DataflowDsdIsCrossSectional(kf) : !Utils.IsTimeSeries(kf);
            //if (cross)
            //    op = SDMXWSFunction.GetCrossSectionalData;
            //var ser = new JavaScriptSerializer();
            //ser.MaxJsonLength = int.MaxValue;
            //try
            //{
            //    IGetSDMX GetSDMXObject = WebServiceSelector.GetSdmxImplementation(DataObjConfiguration);
            //    GetSDMXObject.ExecuteQuery(CreateQueryBean(df, kf, Criterias), op, FileTmpData);

            //}
            //catch (Exception ex)
            //{

            //}
            throw new NotImplementedException();
        }

        #endregion


    }
}

using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.Enum;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using ISTAT.WebClient.WidgetEngine.Model.DBData;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.DataParser.Engine;
using Org.Sdmxsource.Sdmx.DataParser.Engine.Reader;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.WidgetEngine.Model
{
    public class BaseDataObject
    {
        private int MaximumObservations = int.MaxValue;
        private String FileTmpData = "";
        private EndpointSettings DataObjConfiguration = null;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseDataObject));


        public BaseDataObject(EndpointSettings dataObjConfiguration, string fileTmpData)
        {
            DataObjConfiguration = dataObjConfiguration;
            FileTmpData = fileTmpData;
        }

        #region Private Method

        internal List<DataCriteria> InitCriteria(IDataStructureObject kf, Dictionary<string, List<string>> Criteria)
        {
            List<DataCriteria> criterias = new List<DataCriteria>();
            foreach (IComponent comp in kf.Components.Where(c => Criteria.ContainsKey(c.Id)))
            {
                criterias.Add(new DataCriteria() { component = comp.Id, values = Criteria[comp.Id] });
            }
            return criterias;
        }

        /*
        internal List<DataCriteria> InitCriteria(IDataStructureObject kf, Dictionary<string, List<string>> Criteria)
        {
            List<DataCriteria> criterias = new List<DataCriteria>();
            foreach (IComponent comp in kf.Components.Where(c => Criteria.ContainsKey(c.Id)))
            {
                criterias.Add(new DataCriteria() { component = comp.Id, values = Criteria[comp.Id] });
            }

            foreach (IComponent comp in kf.DimensionList.Dimensions.Where(c => !Criteria.ContainsKey(c.Id)))
            {
                criterias.Add(new DataCriteria() { component = comp.Id, values = Criteria[comp.Id] });
            }
            return criterias;
        }
        */


        public IDataQuery CreateQueryBean(IDataflowObject df, IDataStructureObject kf, List<DataCriteria> Criterias)
        {

            ISet<IDataQuerySelection> selections = new HashSet<IDataQuerySelection>();
            string startTime = String.Empty;
            string endTime = String.Empty;

            // Under the DataWhere only one child MUST reside.
            foreach (var queryComponent in Criterias)
            {
                if (queryComponent != null)
                {

                    if (!string.IsNullOrEmpty(queryComponent.component) && queryComponent.component != kf.TimeDimension.Id)
                    {
                        //if (queryComponent.values.Count > 0) baco 25/11/2015
                        if (queryComponent.values.Count > 0 && !string.IsNullOrEmpty(queryComponent.values[0]))
                        {
                            ISet<string> valuern = new HashSet<string>();
                            foreach (string c in queryComponent.values)
                            {
                                if (!string.IsNullOrEmpty(c))
                                    valuern.Add((c));
                            }
                            IDataQuerySelection selection = new DataQueryDimensionSelectionImpl(queryComponent.component, valuern);
                            selections.Add(selection);
                        }
                    }
                    else if (!string.IsNullOrEmpty(queryComponent.component) && queryComponent.component == kf.TimeDimension.Id)
                    {
                        if (queryComponent.values.Count > 0 && !string.IsNullOrEmpty(queryComponent.values[0]))
                        {
                            startTime = queryComponent.values[0];
                            //if (queryComponent.values.Count > 1 && !string.IsNullOrEmpty(queryComponent.values[1]))
                            if (queryComponent.values.Count > 1 && !string.IsNullOrEmpty(queryComponent.values[queryComponent.values.Count-1]))
                                endTime = queryComponent.values[queryComponent.values.Count - 1];
                        }
                    }
                }
            }
            IDataQuerySelectionGroup sel = new DataQuerySelectionGroupImpl(selections, null, null);
            if ((string.IsNullOrEmpty(startTime)) && (!string.IsNullOrEmpty(endTime)))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(endTime), new SdmxDateCore(endTime));
            }
            else if ((!string.IsNullOrEmpty(startTime)) && (string.IsNullOrEmpty(endTime)))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime), new SdmxDateCore(startTime));
            }
            else if ((!string.IsNullOrEmpty(startTime)) && (!string.IsNullOrEmpty(endTime)))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime), new SdmxDateCore(endTime));
            }
            IList<IDataQuerySelectionGroup> selGroup = new List<IDataQuerySelectionGroup>();
            selGroup.Add(sel);
            IDataQuery query;
            if (DataObjConfiguration._TypeEndpoint == EndpointType.REST)
            {
                query = new DataQueryFluentBuilder().Initialize(kf, df).
                      WithDataQuerySelectionGroup(selGroup).Build();
            }
            else
            {
                query = new DataQueryFluentBuilder().Initialize(kf, df).
                       WithOrderAsc(true).
                       WithMaxObservations(MaximumObservations).
                       WithDataQuerySelectionGroup(selGroup).Build();
            }
            return query;
        }

        private IDataQuery CreateQueryBean(IDataflowObject df, IDataStructureObject kf)
        {
            IDataQuery query;
            if (DataObjConfiguration._TypeEndpoint == EndpointType.REST)
            {
                query = new DataQueryFluentBuilder().Initialize(kf, df).Build();
            }
            else
            {
                query = new DataQueryFluentBuilder().Initialize(kf, df).
                       WithOrderAsc(true).
                       WithMaxObservations(MaximumObservations).Build();
            }
            return query;
        }

        internal IDataSetStore GetDataset(IDataflowObject df, IDataStructureObject kf, List<DataCriteria> Criterias, ref Dictionary<string, List<DataChacheObject>> DataCache,bool useAttr)
        {
            // if it is not time series then assume it is cross
            SDMXWSFunction op = SDMXWSFunction.GetCompactData;
            bool cross = (DataObjConfiguration._TypeEndpoint == EndpointType.V21 || DataObjConfiguration._TypeEndpoint == EndpointType.REST)
                          ? NsiClientHelper.DataflowDsdIsCrossSectional(kf) : !Utils.IsTimeSeries(kf);
            if (cross)
                op = SDMXWSFunction.GetCrossSectionalData;

            var ser = new JavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            try
            {
                //commentato vecchio codice
                //IGetSDMX GetSDMXObject = WebServiceSelector.GetSdmxImplementation(DataObjConfiguration);
                //GetSDMXObject.ExecuteQuery(CreateQueryBean(df, kf, Criterias), op, FileTmpData);

                /*
                #region Connessione e Creazione DB SQLLite
                string table = Path.Combine(Utils.GetAppPath(), string.Format(CultureInfo.InvariantCulture, "{0}-{1}.sqlite", Utils.MakeKey(df).Replace("+", "_").Replace(".", ""), Guid.NewGuid()));
                string ConnectionString = string.Format(CultureInfo.InvariantCulture, Constants.FileDBSettingsFormat, table);
                var info = new DBInfo(ConnectionString);
                string tempTable = "table_" + Utils.MakeKey(df).Replace("+", "_").Replace(".", "");
                IDataSetStore store = new DataSetStoreDB(info, tempTable, kf, true, useAttr);
                #endregion
                fine vecchio codice*/

                //Salvo in Session
                /*
                if (DataCache == null)
                    DataCache = new Dictionary<string, List<DataChacheObject>>();
                if (!DataCache.ContainsKey(Utils.MakeKey(df)))
                    DataCache[Utils.MakeKey(df)] = new List<DataChacheObject>();
                */
                //string table=null;
                //FABIO NEW
                //IDataSetStore store = FindDataCacheChart(df, kf, Criterias, ref DataCache, useAttr,out table);
                
                //if (store == null) store = GetDataset(df, kf, Criterias, ref DataCache, useAttr);
                #region Connessione e Creazione DB SQLLite FABIO se nullo lo istanzio
//                if (store == null)
//                {
                    string table = null;
                    IGetSDMX GetSDMXObject = WebServiceSelector.GetSdmxImplementation(DataObjConfiguration);
                    GetSDMXObject.ExecuteQuery(CreateQueryBean(df, kf, Criterias), op, FileTmpData);

                    #region Connessione e Creazione DB SQLLite
                    table = Path.Combine(Utils.GetAppPath(), string.Format(CultureInfo.InvariantCulture, "{0}-{1}.sqlite", Utils.MakeKey(df).Replace("+", "_").Replace(".", ""), Guid.NewGuid()));
                    string ConnectionString = string.Format(CultureInfo.InvariantCulture, Constants.FileDBSettingsFormat, table);
                    var info = new DBInfo(ConnectionString);
                    string tempTable = "table_" + Utils.MakeKey(df).Replace("+", "_").Replace(".", "");
                    IDataSetStore store = new DataSetStoreDB(info, tempTable, kf, true, useAttr);
                    #endregion

                    using (var dataLocation = new FileReadableDataLocation(FileTmpData))
                    {
                        switch (op)
                        {
                            case SDMXWSFunction.GetCompactData:
                                var compact = new CompactDataReaderEngine(dataLocation, df, kf);
                                var readerCompact = new SdmxDataReader(kf, store);
                                readerCompact.ReadData(compact);
                                break;

                            case SDMXWSFunction.GetCrossSectionalData:
                                var dsdCrossSectional = (ICrossSectionalDataStructureObject)kf;
                                var crossSectional = new CrossSectionalDataReaderEngine(dataLocation, dsdCrossSectional, df);
                                var reader = new SdmxDataReader(kf, store);
                                reader.ReadData(crossSectional);
                                break;

                            default:
                                throw new ArgumentException(Resources.ExceptionUnsupported_operation + op.ToString(), "operation");
                        }
                    }

//                }
                #endregion FABIO



                //using (var dataLocation = new FileReadableDataLocation(FileTmpData))
                //{
                //    switch (op)
                //    {
                //        case SDMXWSFunction.GetCompactData:
                //            var compact = new CompactDataReaderEngine(dataLocation, df, kf);
                //            var readerCompact = new SdmxDataReader(kf, store);
                //            readerCompact.ReadData(compact);
                //            break;

                //        case SDMXWSFunction.GetCrossSectionalData:
                //            var dsdCrossSectional = (ICrossSectionalDataStructureObject)kf;
                //            var crossSectional = new CrossSectionalDataReaderEngine(dataLocation, dsdCrossSectional, df);
                //            var reader = new SdmxDataReader(kf, store);
                //            reader.ReadData(crossSectional);
                //            break;

                //        default:
                //            throw new ArgumentException(Resources.ExceptionUnsupported_operation + op.ToString(), "operation");
                //    }
                //}



                /*
                Dictionary<string, List<string>> Criteri = new Dictionary<string, List<string>>();
                Criterias.ForEach(c => Criteri.Add(c.component, c.values));
                DataChacheObject dco = new DataChacheObject()
                {
                    Criterias = Criteri,
                    DBFileName = table,
                };
                
                //aggiunta da fabio
                DataCache[Utils.MakeKey(df)].Clear();
                //fine aggiunta fabio
                DataCache[Utils.MakeKey(df)].Add(dco);
                 */
                return store;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw ex;
            }
            finally
            {
                //delete the temporary file
                if (File.Exists(FileTmpData))
                    File.Delete(FileTmpData);

 
            }
        }

        internal IDataSetStore FindDataCacheChart(IDataflowObject df, IDataStructureObject kf, List<DataCriteria> Criterias, ref Dictionary<string, List<DataChacheObject>> DataCache, bool useAttr, out string DBFileName)
        {
            DataChacheObject findCache = null;
            Dictionary<string, List<string>> Criteri = new Dictionary<string, List<string>>();
            Criterias.ForEach(c => Criteri.Add(c.component, c.values));
            DBFileName = null;

            if (DataCache != null)
            {
                string DfID = Utils.MakeKey(df);
                if (DataCache.ContainsKey(DfID))
                {
                    List<DataChacheObject> singleDataCache = DataCache[DfID];
                    findCache = singleDataCache.Find(dc => DictionaryEqual(dc.Criterias, Criteri));
                }
            }

            if (findCache != null)
            {
                if (!string.IsNullOrEmpty(findCache.DBFileName) && File.Exists(findCache.DBFileName))
                {
                    #region Connessione e Creazione DB SQLLite
                    string ConnectionString = string.Format(CultureInfo.InvariantCulture, Constants.FileDBSettingsFormat, findCache.DBFileName);
                    var info = new DBInfo(ConnectionString);
                    string tempTable = "table_" + Utils.MakeKey(df).Replace("+", "_").Replace(".", "");
                    IDataSetStore store = new DataSetStoreDB(info, tempTable, kf, false, useAttr);
                    DBFileName = findCache.DBFileName;
                    return store;
                    #endregion
                }
                return null;
            }
            return null;
        }


        internal IDataSetStore FindDataCache(IDataflowObject df, IDataStructureObject kf, List<DataCriteria> Criterias, ref Dictionary<string, List<DataChacheObject>> DataCache, bool useAttr, out string DBFileName)
        {
            DataChacheObject findCache = null;
            Dictionary<string, List<string>> Criteri = new Dictionary<string, List<string>>();
            Criterias.ForEach(c => Criteri.Add(c.component, c.values));
            DBFileName = null;

            if (DataCache != null)
            {
                string DfID = Utils.MakeKey(df);
                if (DataCache.ContainsKey(DfID))
                {
                    List<DataChacheObject> singleDataCache = DataCache[DfID];
                    findCache = singleDataCache.Find(dc => DictionaryEqual(dc.Criterias, Criteri));

                    Dictionary<string, List<string>> CriteriDataCache = new Dictionary<string, List<string>>();
                    if (singleDataCache.Count > 0)
                    { CriteriDataCache = singleDataCache.FirstOrDefault().Criterias; }

                    bool pippo= false;
                    if (CriteriDataCache.Count >0 )
                     pippo= DictionaryContain(singleDataCache.FirstOrDefault().Criterias, Criteri);

                    if (pippo)
                    findCache = singleDataCache.FirstOrDefault();
                }
            }

            if (findCache != null)
            {
                if (!string.IsNullOrEmpty(findCache.DBFileName) && File.Exists(findCache.DBFileName))
                {
                    #region Connessione e Creazione DB SQLLite
                    string ConnectionString = string.Format(CultureInfo.InvariantCulture, Constants.FileDBSettingsFormat, findCache.DBFileName);
                    var info = new DBInfo(ConnectionString);
                    string tempTable = "table_" + Utils.MakeKey(df).Replace("+", "_").Replace(".", "");
                    IDataSetStore store = new DataSetStoreDB(info, tempTable, kf, false, useAttr);
                    //DBFileName = findCache.DBFileName;
                    store.SetCriteria(Criterias);
                    return store;
                    #endregion
                }
                return null;
            }
            return null;
        }

private bool DictionaryContain(IDictionary<string, List<string>> x, IDictionary<string, List<string>> y)
{
    // early-exit checks
    if (null == y)
        return null == x;
    if (null == x)
        return false;
    if (object.ReferenceEquals(x, y))
        return true;
    if (x.Count < y.Count)
        return false;

    // check keys are the same
   /* foreach (string k in x.Keys)
        if (!y.ContainsKey(k))
            return false;
    */
    // check values are the same
    foreach (string k in y.Keys)
    {
        List<string> sublist = x[k];
        foreach (string valore in y[k])
        {
             if (!sublist.Contains(valore, StringComparer.OrdinalIgnoreCase))
              return false;
        }
            
            //return false;
    }
    return true;
}

        private bool DictionaryEqual(IDictionary<string, List<string>> first, IDictionary<string, List<string>> second)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;


            foreach (var kvp in first)
            {
                List<string> secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (kvp.Value.Count != secondValue.Count) return false;
                if (!kvp.Value.All(p => secondValue.Contains(p))) return false;

            }
            return true;
        }

        #endregion
    }
}

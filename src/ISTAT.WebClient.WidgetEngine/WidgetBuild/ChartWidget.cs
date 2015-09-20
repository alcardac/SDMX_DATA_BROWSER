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
using System.Configuration;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class ChartWidget
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(TreeWidget));

        private GetChartObject ChartObj { get; set; }

        private SessionImplObject SessionObj { get; set; }

        private IGetSDMX GetSDMXObject = null;
        private CodemapWidget codemapWidget = null;

        private BaseDataObject BDO { get; set; }

        private CultureInfo cFrom;
        private CultureInfo cTo;

        public ChartWidget(GetChartObject chartObj, SessionImplObject sessionObj, CultureInfo cFrom, CultureInfo cTo)
        {
            ChartObj = chartObj;
            SessionObj = sessionObj;
            GetSDMXObject = WebServiceSelector.GetSdmxImplementation(this.ChartObj.Configuration);
            BDO = new BaseDataObject(chartObj.Configuration, System.IO.Path.GetTempFileName());

            this.cFrom = cFrom;
            this.cTo = cTo;
        }

        public SessionImplObject GetDataChart()
        {
            try
            {
                // Init session objects
                if (this.SessionObj == null)
                {
                    this.SessionObj = new SessionImplObject();
                    this.SessionObj.SdmxObject = new SdmxObjectsImpl();
                }
                
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = int.MaxValue;

                #region +++ Caching +++
                ConnectionStringSettings connectionStringSetting;
                CacheWidget cache = null;
                bool UseWidgetCache = (WebClientSettings.Instance != null) ? WebClientSettings.Instance.UseWidgetCache : false;
                if (UseWidgetCache)
                {
                    connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                    cache = new CacheWidget(connectionStringSetting.ConnectionString);
                }
                if (ChartObj.WidgetId > 0 && UseWidgetCache)
                {
                    SavedWidget widget = cache.GetWidget(ChartObj.WidgetId, ChartObj.Configuration.Locale);
                    if (widget != null && !String.IsNullOrEmpty(widget.widgetData))
                    {
                        this.SessionObj.SavedChart = widget.widgetData;
                        return this.SessionObj;
                    }
                } 
                #endregion

                if (BDO == null || GetSDMXObject == null) throw new Exception(Messages.label_error_network);
                
                codemapWidget = new CodemapWidget(
                    new GetCodemapObject() {
                    PreviusCostraint=this.ChartObj.Criteria,
                    Configuration = this.ChartObj.Configuration, 
                    Dataflow = this.ChartObj.Dataflow }, 
                    this.SessionObj);


                ISdmxObjects structure = codemapWidget.GetDsd();
                IDataflowObject df = structure.Dataflows.FirstOrDefault();
                IDataStructureObject kf = structure.DataStructures.First();
                if (kf == null) throw new InvalidOperationException("DSD is not set");
                if (df == null) throw new InvalidOperationException("Dataflow is not set");

                Dictionary<string, ICodelistObject> ConceptCodelists = codemapWidget.GetCodelistMap(df, kf, false);
                ComponentCodeDescriptionDictionary codemap = new ComponentCodeDescriptionDictionary();
                foreach (string ConceptId in ConceptCodelists.Keys)
                {
                    ICodelistObject codelist = ConceptCodelists[ConceptId];
                    Dictionary<string, string> codes = new Dictionary<string, string>();

                    foreach (ICode codeItem in codelist.Items)
                        codes.Add(codeItem.Id, TextTypeHelper.GetText(codeItem.Names, this.ChartObj.Configuration.Locale));
                    codemap.Add(ConceptId, codes);
                    
                    var useFix20 = (ConfigurationManager.AppSettings["UseFix20Criteria"].ToString().ToLower() == "true");
                    if (useFix20)
                    {
                        if (!(codelist.Items.Count > 1)) {
                            this.ChartObj.Criteria.Remove(ConceptId);   
                        }
                    }

                }

                this.SessionObj.MergeObject(codemapWidget.SessionObj);

                #region Gestione last period
                if (this.ChartObj.Criteria.ContainsKey(kf.TimeDimension.Id)
                 && this.ChartObj.Criteria[kf.TimeDimension.Id].Count == 1)
                {
                    int offsetTime = int.Parse(this.ChartObj.Criteria[kf.TimeDimension.Id].First());
                    var codMap = codemap;
                    int lengthTime = codMap[kf.TimeDimension.Id].Count;

                    if ((lengthTime - offsetTime) >= 0)
                    {
                        var codes = codMap[kf.TimeDimension.Id].Reverse().Take(offsetTime);
                        List<string> _criteriaTime = (from c in codes select c.Key).ToList<string>();

                        this.ChartObj.Criteria[kf.TimeDimension.Id] = new List<string>();
                        this.ChartObj.Criteria[kf.TimeDimension.Id].Add(_criteriaTime.Last());
                        this.ChartObj.Criteria[kf.TimeDimension.Id].Add(_criteriaTime.First());
                    }
                    else
                    {
                        this.ChartObj.Criteria[kf.TimeDimension.Id] = new List<string>();
                        this.ChartObj.Criteria[kf.TimeDimension.Id].Add(codemap[kf.TimeDimension.Id].First().Key);
                        this.ChartObj.Criteria[kf.TimeDimension.Id].Add(codemap[kf.TimeDimension.Id].Last().Key);
                    }
                }
                #endregion

                List<DataCriteria> Criterias = BDO.InitCriteria(kf, this.ChartObj.Criteria);

                Dictionary<string, List<DataChacheObject>> DataCache = SessionObj.DataCache;

                IDataSetStore store = BDO.GetDataset(df, kf, Criterias, ref DataCache, false);

                SessionObj.DataCache = DataCache;

                DataObjectForStreaming DataStream = new DataObjectForStreaming()
                {
                    store = store,
                    Criterias = Criterias,
                    structure = structure,
                    codemap = codemap
                };

                ChartResponseObject ChartResponse = new ChartResponseObject();
                ChartResponse.series_title = TextTypeHelper.GetText(df.Names, this.ChartObj.Configuration.Locale);
                ChartResponse.series = BuildChart(store, kf, ConceptCodelists);
                ChartResponse.primary_name =
                    (this.ChartObj.ObsValue[0] == "v") ? Messages.label_varValue :
                    (this.ChartObj.ObsValue[0] == "vt") ? Messages.label_varTrend :
                    (this.ChartObj.ObsValue[0] == "vc") ? Messages.label_varCyclical : string.Empty;
                ChartResponse.secondary_name =
                    (this.ChartObj.ObsValue.Count > 1) ?
                    (this.ChartObj.ObsValue[1] == "v") ? Messages.label_varValue :
                    (this.ChartObj.ObsValue[1] == "vt") ? Messages.label_varTrend :
                    (this.ChartObj.ObsValue[1] == "vc") ? Messages.label_varCyclical : string.Empty : string.Empty;
                ChartResponse.x_name = (!string.IsNullOrEmpty(ChartObj.DimensionAxe)) ? ChartObj.DimensionAxe : kf.TimeDimension.Id; ;

                // 23/07/2015
                // calcolo massimo e minimo 
                decimal? primary_max = null;
                decimal? primary_min = null;
                decimal? secondary_max = null;
                decimal? secondary_min = null;
                decimal costantemax = 1.1m;
                decimal costantemin = 0.9m;
                
                foreach (serieType serie in ChartResponse.series) {

                    if (serie.axisYType == "secondary")
                    {
                        //fabio 12/08/2015
                        //decimal max = (decimal)serie.dataPoints.Where(m => m.y != null).Max(d => d.y);
                        decimal max = Convert.ToDecimal(serie.dataPoints.Where(m => m.y != null).Max(d => d.y));
                        if (secondary_max == null || max > secondary_max) secondary_max = max;

                        //fabio 12/08/2015
                        //decimal min = (decimal)serie.dataPoints.Where(m => m.y != null).Min(d => d.y);
                        decimal min = Convert.ToDecimal(serie.dataPoints.Where(m => m.y != null).Min(d => d.y));                        
                        if (secondary_min==null || min < secondary_min) secondary_min = min;
                        
                        //fabio 12/08/2015
                        if (secondary_min == secondary_max) { secondary_min = secondary_min * costantemin; secondary_max = secondary_max * costantemax; }                        
                    }
                    else
                    {
                        //fabio 12/08/2015
                        //decimal max = (decimal)serie.dataPoints.Where(m => m.y != null).Max(d => d.y);
                        decimal max = Convert.ToDecimal(serie.dataPoints.Where(m => m.y != null).Max(d => d.y));                        
                        if (primary_max==null || max > primary_max) primary_max = max;

                        //fabio 12/08/2015
                        //decimal min = (decimal)serie.dataPoints.Where(m => m.y != null).Min(d => d.y);
                          decimal min = Convert.ToDecimal(serie.dataPoints.Where(m => m.y != null).Min(d => d.y));                        
                        if (primary_min==null || min < primary_min) primary_min = min;
                        
                        //fabio 12/08/2015
                        if (primary_min == primary_max) { primary_min = primary_min * costantemin; primary_max = primary_max * costantemax; }                        
                    }
                }
                if (primary_max != null && primary_min!=null)
                {
                    ChartResponse.primary_max = (decimal)primary_max;
                    ChartResponse.primary_min = (decimal)primary_min;
                } 
                if (secondary_max != null && secondary_min!=null)
                {
                    ChartResponse.secondary_max = (decimal)secondary_max;
                    ChartResponse.secondary_min = (decimal)secondary_min;
                }

                this.SessionObj.SavedChart = ser.Serialize(ChartResponse);

                // +++ Caching +++
                if (ChartObj.WidgetId > 0 && UseWidgetCache)
                {
                    cache.InsertWidget(ChartObj.WidgetId, this.SessionObj.SavedChart, ChartObj.Configuration.Locale);
                }

                return this.SessionObj;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw ex;
            }
        }

        private List<serieType> BuildChart(
            IDataSetStore store, 
            IDataStructureObject kf, 
            Dictionary<string, ICodelistObject> codelists)
        {

            List<string> sort = new List<string>();
            foreach (string col in store.GetAllColumns())
                if (col != kf.TimeDimension.Id && col != kf.PrimaryMeasure.Id) sort.Add(col);
            store.SetSort(sort);

            // Dimensione sull'asse X
            string XConcept =
                (!string.IsNullOrEmpty(ChartObj.DimensionAxe)) ?
                ChartObj.DimensionAxe :
                kf.TimeDimension.Id;
            // Codici sull'asse X
            Dictionary<string, int> XPosition = new Dictionary<string, int>();
            if (codelists.ContainsKey(XConcept))
            {
                ICodelistObject CCodes = codelists[XConcept];
                for (int i = 0; i < CCodes.Items.Count; i++)
                    XPosition.Add(CCodes.Items[i].Id, i);
            }

            #region Dimensione usata per la descrizione
            string DescConcept = string.Empty;
            bool single_serie = true;
            foreach (var obj in ChartObj.Criteria)
            {
                if (//obj.Key != kf.TimeDimension.Id &&
                    obj.Key != kf.FrequencyDimension.Id)
                {
                    DescConcept = obj.Key;
                    if (obj.Value.Count > 1 && obj.Key != XConcept)
                    {
                        single_serie = false;
                        DescConcept = obj.Key;
                        break;
                    }
                }
            }

            bool inLegend = !single_serie;
            inLegend = inLegend || (single_serie && ChartObj.ObsValue.Count > 1);

            #endregion

            List<serieType> series = new List<serieType>();
            List<serieType> series_s = new List<serieType>();
            var v = new Dictionary<string, decimal>();

            IDataReader datareader = store.CreateDataReader(true);
            while (datareader.Read())
            {
                decimal obs = 0;
                object vt = null;
                object vc = null;

                var obs_val = datareader[kf.PrimaryMeasure.Id];
                var xcode = (string)datareader[XConcept];
                var xCodeName = TextTypeHelper.GetText(codelists[XConcept].GetCodeById(xcode).Names, this.ChartObj.Configuration.Locale);

                //fabio 12/08/2015
                //string customKeyCode = (ChartObj.CustomKey != null) ? datareader[ChartObj.CustomKey].ToString() : string.Empty;
                string customKeyCode = (ChartObj.CustomKey != null && ChartObj.CustomKey != "") ? datareader[ChartObj.CustomKey].ToString() : string.Empty;                
                var customSerie = (ChartObj.CustomChartType != null) ?
                                        (!string.IsNullOrEmpty(customKeyCode)) ?
                                            (from c in ChartObj.CustomChartType where c.code == customKeyCode select c).FirstOrDefault() :
                                            null : null;

                string serieKey = string.Empty;
                string serieName = string.Empty;

                bool is_obs_value = false;
                try
                {
                    obs = Convert.ToDecimal(obs_val.ToString(), cFrom);
                    is_obs_value = true;
                    obs_val = Math.Round(obs, 1);
                }
                catch
                {
                    //fabio 12/08/2015 aggiunta
                    obs_val = null;
                    is_obs_value = true;
                    
                    //fabio 12/08/2015 eliminata
                    //is_obs_value = false;                    
                }

                // if not time serie no varation 
                if (XConcept == kf.TimeDimension.Id)
                {
                    #region Calcolo variazioni

                    if (is_obs_value)
                    {
                        var time_p = xcode;
                        int anno = 0;
                        int period = 0;

                        bool _errTimePeriod = false;

                        bool _annual = !((string)time_p).Contains("-");
                        bool _quater = false;
                        bool _seme = false;

                        #region ESTRAGGO ANNO E PERIOD
                        if (_annual)
                        {
                            _errTimePeriod = !(int.TryParse(((string)time_p), out anno));
                        }
                        else
                        {
                            _errTimePeriod = !(int.TryParse(((string)time_p).Split('-')[0], out anno));

                            string _p = ((string)time_p).Split('-')[1];

                            if (_quater = _p.StartsWith("Q")) _p = _p.Substring(1);
                            if (_seme = _p.StartsWith("S")) _p = _p.Substring(1);

                            _errTimePeriod = !(int.TryParse(_p, out period));
                        }
                        #endregion

                        if (!_errTimePeriod)
                        {
                            string serieKeyStr = string.Empty;
                            string _sep = string.Empty;
                            foreach (var dim in kf.DimensionList.Dimensions)
                            {
                                serieKeyStr += ((dim.Id != XConcept) ? _sep + datareader[dim.Id] : string.Empty);
                                _sep = "+";
                                if (dim.Id == DescConcept)
                                {
                                    serieName =
                                        TextTypeHelper.GetText(
                                        codelists[DescConcept].GetCodeById(datareader[dim.Id].ToString()).Names,
                                        this.ChartObj.Configuration.Locale);
                                }
                            }
                            serieKey = serieKeyStr;

                            string vi_k = string.Empty;
                            string vf_k = string.Empty;

                            // Calcolo variazione congiunturale

                            vf_k = serieKeyStr + anno + "_" + (period);
                            if (!_annual)
                                if (period == 1)
                                    if (_seme)
                                        vi_k = serieKeyStr + (anno - 1) + "_2";
                                    else if (_quater)
                                        vi_k = serieKeyStr + (anno - 1) + "_4";
                                    else
                                        vi_k = serieKeyStr + (anno - 1) + "_12";
                                else
                                    vi_k = serieKeyStr + anno + "_" + (period - 1);
                            else vi_k = serieKeyStr + (anno - 1) + "_" + (period);

                            var vi = (v.ContainsKey(vi_k.ToString())) ? (object)v[vi_k] : null;

                            try
                            {
                                decimal _vi;
                                // non specificare il cFrom nella conversione 
                                // poichè vi è il valore gia convertito
                                _vi = Convert.ToDecimal(vi.ToString());
                                //_vi = Convert.ToDecimal(vi.ToString(), cFrom);
                                if (_vi == 0) vc = null;
                                else vc = Math.Round((((obs - _vi) / _vi) * 100), 1);
                            }
                            catch {
                                vc = null;
                            }

                            // Calcolo variazione tendenziale
                            vi_k = serieKeyStr + (anno - 1) + "_" + (period);
                            vf_k = serieKeyStr + anno + "_" + (period);
                            vi = (v.ContainsKey(vi_k.ToString())) ? (object)v[vi_k] : null;

                            try
                            {
                                decimal _vi;
                                _vi = Convert.ToDecimal(vi.ToString());
                                //_vi = Convert.ToDecimal(vi.ToString(), cFrom);
                                if (_vi == 0) vc = null;
                                else vt = Math.Round((((obs - _vi) / _vi) * 100), 1);
                            }
                            catch
                            {
                                vt = null;
                            }

                            v.Add(vf_k, obs);
                        }
                    }
                    #endregion
                }
                else
                {
                    // Retrive unique key and label serie
                    string serieKeyStr = string.Empty;
                    string _sep = string.Empty;
                    foreach (var dim in kf.DimensionList.Dimensions)
                    {
                        serieKeyStr += ((dim.Id != XConcept) ? _sep + datareader[dim.Id] : string.Empty);
                        _sep = "+";
                        if (dim.Id == DescConcept)
                        {
                            serieName =
                                TextTypeHelper.GetText(
                                codelists[DescConcept].GetCodeById(datareader[dim.Id].ToString()).Names,
                                this.ChartObj.Configuration.Locale);
                        }
                    }
                    serieKey = serieKeyStr;
                }

                #region Primary Serie
                object primary_obs =
                            (ChartObj.ObsValue[0] == "v") ? (is_obs_value) ? obs_val : null :
                            (ChartObj.ObsValue[0] == "vt") ? vt :
                            (ChartObj.ObsValue[0] == "vc") ? vc : null;

                bool isNew = false;
                serieType newSerie = null;
                newSerie = series.Find(s => s.serieKey == ChartObj.ObsValue[0] + "_" + serieKey);//SerieName);

                if (newSerie == null)
                {
                    string _type = (customSerie != null) ? customSerie.chartType.ToString() : ChartObj.ChartType;

                    string _name =  (ChartObj.ObsValue[0] == "vt") ? Messages.label_varTrend + "% " :
                            (ChartObj.ObsValue[0] == "vc") ? Messages.label_varCyclical + "% " :
                            (single_serie)? Messages.label_varValue:    string.Empty;

                    if (!single_serie)
                    {
                        _name += ((customSerie != null) ? customSerie.title.ToString() : serieName);
                    }

                    isNew = true;
                    newSerie = new serieType()
                    {
                        name = _name,
                        serieKey = ChartObj.ObsValue[0] + "_" + serieKey,
                        showInLegend = inLegend,
                        type = _type,
                        dataPoints = new List<dataPointType>(),
                        axisYType = "primary",
                        lineThickness = 1f,
                        markerType = "circle",  //"circle", "square", "cross", "none"
                        markerSize = (_type == "bubble" || _type == "scatter") ? 10f : 1f,
                    };
                }

                newSerie.dataPoints.Add(new dataPointType()
                {
                    label = xCodeName,
                    legendText = xCodeName,
                    y = primary_obs,
                    x = XPosition[xcode]
                });

                if (isNew) series.Add(newSerie);
                #endregion

                // if not time serie no secondary 
                if (XConcept == kf.TimeDimension.Id)
                {
                    #region Seconday serie
                    if (ChartObj.ObsValue.Count > 1)
                    {
                        string _type = (customSerie != null) ? customSerie.chartType.ToString() : ChartObj.ChartType;
                        string _name = (ChartObj.ObsValue[1] == "vt") ? Messages.label_varTrend + "% " :
                            (ChartObj.ObsValue[1] == "vc") ? Messages.label_varCyclical + "% " : Messages.label_varValue + " ";

                        if (!single_serie)
                        {
                            _name += ((customSerie != null) ? customSerie.title.ToString() : serieName);
                        }
                        object secondary_obs =
                        (ChartObj.ObsValue[1] == "v") ? (is_obs_value) ? obs_val : null :
                        (ChartObj.ObsValue[1] == "vt") ? vt :
                        (ChartObj.ObsValue[1] == "vc") ? vc : null;

                        bool isNew_s = false;
                        serieType newSerie_s = null;
                        newSerie_s = series_s.Find(s => s.serieKey == ChartObj.ObsValue[1] + "_" + serieKey);//SerieName);
                        if (newSerie_s == null)
                        {
                            isNew_s = true;
                            newSerie_s = new serieType()
                            {
                                name = _name,
                                serieKey = ChartObj.ObsValue[1] + "_" + serieKey,
                                showInLegend = inLegend,
                                type = _type,
                                dataPoints = new List<dataPointType>(),
                                axisYType = "secondary",
                                lineThickness = 1f,
                                markerType = "circle",  //"circle", "square", "cross", "none"
                                markerSize = (_type == "bubble" || _type == "scatter") ? 10f : 1f,
                            };
                        }
                        newSerie_s.dataPoints.Add(new dataPointType()
                        {
                            label = xCodeName,
                            legendText = xCodeName,
                            y = secondary_obs,
                            x = XPosition[xcode]
                        });

                        if (isNew_s) series_s.Add(newSerie_s);
                    }
                    #endregion
                }
            }
            series.AddRange(series_s);

            #region Series
            foreach (var serie in series)
            {
                var sortedCodes = serie.dataPoints.OrderBy<dataPointType, int>(o => int.Parse(o.x.ToString())).ToArray();
                serie.dataPoints.Clear();
                serie.dataPoints.AddRange(sortedCodes);

                for (int i = 0; i < serie.dataPoints.Count; i++)
                {
                    serie.dataPoints[i].x = i;
                }

            }
            #endregion

            return series;
        }

    }
}

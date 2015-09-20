using Estat.Sri.CustomRequests.Constants;
using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using ISTAT.WebClient.WidgetEngine.Model;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class CodemapWidget
    {
        private GetCodemapObject CodemapObj { get; set; }
        internal SessionImplObject SessionObj { get; set; }
        private IGetSDMX GetSDMXObject = null;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CodemapWidget));


        public CodemapWidget(GetCodemapObject codemapObj, SessionImplObject sessionObj)
        {
            CodemapObj = codemapObj;
            SessionObj = sessionObj;
            GetSDMXObject = WebServiceSelector.GetSdmxImplementation(this.CodemapObj.Configuration);

            if (this.SessionObj == null)
            {
                this.SessionObj = new SessionImplObject();
                this.SessionObj.SdmxObject = new SdmxObjectsImpl();
            }

        }

        public SessionImplObject GetCodemap()
        {
            try
            {
                if (GetSDMXObject == null)
                    throw new Exception(Messages.label_error_network + " " + CodemapObj.Configuration.Title);

                ISdmxObjects structure = GetDsd();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");

                Dictionary<string, ICodelistObject> ConceptCodelists = 
                    GetCodelistMap(df, kf, true);

                CodemapResponseObject codemapret = new CodemapResponseObject()
                {
                    codemap = ParseCodelist(ConceptCodelists),
                    key_time_dimension = kf.TimeDimension.Id,
                    dataflow = new MaintenableObj()
                    {
                        id = df.Id,
                        agency = df.AgencyId,
                        version = df.Version,
                        name = TextTypeHelper.GetText(df.Names, this.CodemapObj.Configuration.Locale),
                        description = TextTypeHelper.GetText(df.Descriptions, this.CodemapObj.Configuration.Locale)
                    }
                };

                this.SessionObj.SavedCodemap = new JavaScriptSerializer().Serialize(codemapret);

            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw ex;
            }

            return this.SessionObj;

        }

        public SessionImplObject GetCodemap(ConnectionStringSettings connectionStringSetting)
        {
            try
            {
                ISdmxObjects structure = GetDsd();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");
                if (df == null)
                    throw new InvalidOperationException("Dataflow is not set");

                Dictionary<string, ICodelistObject> ConceptCodelists = GetCodelistMap(df, kf, false);

                this.SessionObj.SdmxObject.Codelists.Clear();
                foreach (ICodelistObject codelist in ConceptCodelists.Values)
                    this.SessionObj.SdmxObject.AddCodelist(codelist);

                ConceptCodelists = this.GetCodelistMap(df, kf, false);
                TemplateWidget templateWidget = new TemplateWidget(connectionStringSetting.ConnectionString);
                var template = templateWidget.GetSingle(new GetTemplateObject()
                {
                    Template = new TemplateObject()
                    {
                        Dataflow = CodemapObj.Dataflow,
                        Configuration = CodemapObj.Configuration,
                    }
                });
                CodemapSpecificResponseObject codemapret = new CodemapSpecificResponseObject()
                {
                    codemap = ParseCodelist(ConceptCodelists),
                    costraint = (template != null) ? template.Criteria : null,
                    hideDimension = (template != null) ? template.HideDimension : null,
                    enabledVar = (template != null) ? template.EnableVaration : true,
                    enabledCri = (template != null) ? template.EnableCriteria : true,
                    enabledDec = (template != null) ? template.EnableDecimal : true,
                    key_time_dimension = kf.TimeDimension.Id,
                    freq_dimension = kf.FrequencyDimension.Id,
                    dataflow = new MaintenableObj()
                    {
                        id = df.Id,
                        agency = df.AgencyId,
                        version = df.Version,
                        name = TextTypeHelper.GetText(df.Names, this.CodemapObj.Configuration.Locale),
                        description = TextTypeHelper.GetText(df.Descriptions, this.CodemapObj.Configuration.Locale)
                    }
                };


                this.SessionObj.SavedCodemap = new JavaScriptSerializer().Serialize(codemapret);
                return this.SessionObj;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw ex;
            }
        }

        public SessionImplObject GetSpecificCodemap(bool firstDimension, ConnectionStringSettings connectionStringSetting)
        {
            try
            {
                ISdmxObjects structure = GetDsd();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");

                TemplateWidget templateWidget = new TemplateWidget(connectionStringSetting.ConnectionString);
                var template=templateWidget.GetSingle(new GetTemplateObject() {
                    Template=new TemplateObject(){ 
                        Dataflow = CodemapObj.Dataflow,
                        Configuration = CodemapObj.Configuration,
                }});


                string dimension = null;
                Dictionary<string, ICodelistObject> ConceptCodelists = null;
                // Se ha una template forzo il retrive di tutte le codelist per evitare problemi in stampa tabella
                if (template!=null)
                {
                    ConceptCodelists = this.GetCodelistMap(df, kf, false);
                }
                else {
                    dimension = (firstDimension) ? kf.DimensionList.Dimensions.FirstOrDefault().Id : CodemapObj.Codelist;
                    ConceptCodelists=GetCodelistMap(dimension, df, kf);
                }

                CodemapSpecificResponseObject codemapret = new CodemapSpecificResponseObject()
                {
                    codemap = ParseCodelist(ConceptCodelists),
                    costraint = (template != null) ? template.Criteria : null,
                    hideDimension = (template != null) ? template.HideDimension : null,
                    enabledVar = (template != null) ? template.EnableVaration : true,
                    enabledCri = (template != null) ? template.EnableCriteria : true,
                    enabledDec = (template != null) ? template.EnableDecimal : true,
                    codelist_target = dimension,
                    key_time_dimension = kf.TimeDimension.Id,
                    freq_dimension = kf.FrequencyDimension.Id,
                    dataflow = new MaintenableObj()
                    {
                        id = df.Id,
                        agency = df.AgencyId,
                        version = df.Version,
                        name = TextTypeHelper.GetText(df.Names, this.CodemapObj.Configuration.Locale),
                        description = TextTypeHelper.GetText(df.Descriptions, this.CodemapObj.Configuration.Locale)
                    }
                };

                this.SessionObj.SavedCodemap = new JavaScriptSerializer().Serialize(codemapret);
                return this.SessionObj;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw ex;
            }
        }

        public Dictionary<string, ICodelistObject> GetCodelistMap(
            IDataflowObject df,
            IDataStructureObject kf,
            bool withAttribute)
        {
            Dictionary<string, ICodelistObject> Conceptcodelist = new Dictionary<string, ICodelistObject>();
            if (kf != null && df != null)
            {
                foreach (IDimension component in
                    kf.DimensionList.Dimensions.Where(
                    c => c.HasCodedRepresentation()
                        && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)))
                    Conceptcodelist.Add(component.Id, this.GetCodeList(df, kf, component));


                if (withAttribute && kf.AttributeList != null)
                {
                    foreach (IComponent component in kf.AttributeList.Attributes.Where(c => c.HasCodedRepresentation() && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)))
                        Conceptcodelist.Add(component.Id, this.GetCodeList(df, kf, component));
                }

                if (Conceptcodelist.ContainsKey(kf.FrequencyDimension.Id) == true
                    && Conceptcodelist[kf.FrequencyDimension.Id] != null)
                    Conceptcodelist.Add(kf.TimeDimension.Id, this.GetTimeCodeList(Conceptcodelist[kf.FrequencyDimension.Id], df, kf));

                if (this.SessionObj.CodelistConstrained == null) this.SessionObj.CodelistConstrained = new Dictionary<string, Dictionary<string, ICodelistObject>>();
                this.SessionObj.CodelistConstrained[Utils.MakeKey(df)] = Conceptcodelist;

            }
            return Conceptcodelist;
        }

        public Dictionary<string, ICodelistObject> GetCodelistMap(
            string dim,
            IDataflowObject df,
            IDataStructureObject kf)
        {

            Dictionary<string, ICodelistObject> Conceptcodelist = new Dictionary<string, ICodelistObject>();

            foreach (IDimension component in kf.DimensionList.Dimensions)
                Conceptcodelist.Add(component.Id, null);

            if (dim != kf.TimeDimension.Id)
            {
                var dimCodelist = kf.DimensionList.Dimensions.Where(
                    c => c.HasCodedRepresentation()
                    && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)
                    && c.Id == dim).FirstOrDefault();
                Conceptcodelist[dimCodelist.Id] = this.GetCodeListCostraint(df, kf, dimCodelist);
            }
            else
            {
                var freqCodelist = (Conceptcodelist[kf.FrequencyDimension.Id] != null) ? Conceptcodelist[kf.FrequencyDimension.Id] : this.GetCodeListCostraint(df, kf, kf.FrequencyDimension);
                Conceptcodelist[kf.TimeDimension.Id] = this.GetTimeCodeListCostraint(freqCodelist, df, kf);
            }
            return Conceptcodelist;
        }

        public ISdmxObjects GetDsd()
        {
            IDataflowObject dataflow = null;
            if (this.SessionObj.SdmxObject != null 
                && this.SessionObj.SdmxObject.Dataflows != null)
            {
                dataflow = this.SessionObj.SdmxObject.Dataflows.FirstOrDefault(d => d.AgencyId == this.CodemapObj.Dataflow.agency && d.Id == this.CodemapObj.Dataflow.id && d.Version == this.CodemapObj.Dataflow.version);
            }

            ISdmxObjects Structure = null;
            if (dataflow != null)
            {
                Structure = GetSDMXObject.GetStructure(dataflow, this.SessionObj.SdmxObject.DataStructures);
                Structure.AddDataflow(dataflow);
            }
            else
            {
                Structure = GetSDMXObject.GetStructure(this.CodemapObj.Dataflow.id, this.CodemapObj.Dataflow.agency, this.CodemapObj.Dataflow.version);
            }

            if (this.SessionObj.SdmxObject != null)
                this.SessionObj.SdmxObject.Merge(Structure);
            else this.SessionObj.SdmxObject = Structure;

            return Structure;
        }


        public int GetCountObservation() {

                ISdmxObjects structure = GetDsd();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");
                if (df == null)
                    throw new InvalidOperationException("Dataflow is not set");

                var currentComponent = "CL_COUNT";

                IContentConstraintMutableObject criteria = new ContentConstraintMutableCore();
                criteria.Id = currentComponent;
                criteria.AddName("en", "english");
                criteria.AgencyId = "agency";
                ICubeRegionMutableObject region = new CubeRegionMutableCore();

                if (currentComponent != null)
                {
                    IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                    keyValue.Id = currentComponent;
                    keyValue.AddValue(SpecialValues.DummyMemberValue);
                    region.AddKeyValue(keyValue);

                    if (CodemapObj.PreviusCostraint != null)
                    {
                        foreach (string costreintKey in CodemapObj.PreviusCostraint.Keys)
                        {
                            if (costreintKey == currentComponent) continue;
                            if (costreintKey == kf.TimeDimension.Id)
                            {

                                // Qui considerare il caso in qui in CodemapObj.PreviusCostraint[costreintKey][0] ci sia solo un valore, ke equivale alla data da.
                                if (CodemapObj.PreviusCostraint[costreintKey].Count > 1)
                                {
                                    DateTime MinDate = GetDateTimeFromSDMXTimePeriod(CodemapObj.PreviusCostraint[costreintKey][0].ToString(), 'M'); //DateTime.ParseExact(CodemapObj.PreviusCostraint[costreintKey][0].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
                                    DateTime MaxDate = GetDateTimeFromSDMXTimePeriod(CodemapObj.PreviusCostraint[costreintKey][1].ToString(), 'M'); //DateTime.ParseExact(CodemapObj.PreviusCostraint[costreintKey][1].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                                    if (MinDate.CompareTo(MaxDate) > 0)
                                    {
                                        criteria.StartDate = MinDate;
                                        criteria.EndDate = MaxDate;
                                    }
                                    else {
                                        criteria.StartDate = MinDate;
                                        criteria.EndDate = MinDate;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var code in CodemapObj.PreviusCostraint[costreintKey])
                                {
                                    IKeyValuesMutable _keyValue = new KeyValuesMutableImpl();
                                    _keyValue.Id = costreintKey;
                                    _keyValue.AddValue(code);
                                    region.AddKeyValue(_keyValue);
                                }
                            }
                        }
                    }

                }
                criteria.IncludedCubeRegion = region;

            int count = GetSDMXObject.GetDataflowDataCount(df, criteria);
            return count;
        }


        private Dictionary<string, CodemapObj> ParseCodelist(Dictionary<string, ICodelistObject> Codemap)
        {
            Dictionary<string, CodemapObj> dr_codemap = new Dictionary<string, CodemapObj>();

            foreach (string conceptId in Codemap.Keys)
            {
                //List<string> criteri = new List<string>();
                ICodelistObject codelist = Codemap[conceptId];
                if (codelist != null)
                {

                    if (this.SessionObj != null && this.SessionObj.SdmxObject != null) {
                        this.SessionObj.SdmxObject.AddCodelist(codelist);
                    }

                    CodemapObj codemap = new CodemapObj()
                    {
                        title = TextTypeHelper.GetText(codelist.Names, this.CodemapObj.Configuration.Locale),
                        codes = new Dictionary<string, CodeObj>()
                    };

                    foreach (ICode codeItem in codelist.Items)
                        codemap.codes.Add(
                            codeItem.Id.ToString(),
                            new CodeObj()
                            {
                                name = TextTypeHelper.GetText(codeItem.Names, this.CodemapObj.Configuration.Locale),
                                parent = codeItem.ParentCode
                            });

                    codemap.codes = codemap.codes.OrderBy(c => c.Value.parent == null).ToDictionary(c => c.Key, c => c.Value);
                    dr_codemap.Add(conceptId, codemap);

                }
                else { dr_codemap.Add(conceptId, null); }

            }
            return dr_codemap;
        }

        private ICodelistObject GetCodeList(IDataflowObject df, IDataStructureObject kf, IComponent component)
        {
            ICodelistObject codes = null;
            if (this.SessionObj.CodelistConstrained != null && this.SessionObj.CodelistConstrained.ContainsKey(Utils.MakeKey(df)))
            {
                if (this.SessionObj.CodelistConstrained[Utils.MakeKey(df)].ContainsKey(component.Id))
                {
                    codes = this.SessionObj.CodelistConstrained[Utils.MakeKey(df)][component.Id];
                }
            }


            if (codes == null)
            {
                bool Contrained = component.StructureType.EnumType != Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataAttribute;
                #region Criteria
                List<IContentConstraintMutableObject> criterias = new List<IContentConstraintMutableObject>();

                if (Contrained)
                {
                    IContentConstraintMutableObject criteria = new ContentConstraintMutableCore();
                    string currentComponent = component.ConceptRef.ChildReference.Id;
                    criteria.Id = currentComponent ?? "SPECIAL";
                    criteria.AddName("en", "english");
                    criteria.AgencyId = "agency";
                    ICubeRegionMutableObject region = new CubeRegionMutableCore();

                    if (currentComponent != null)
                    {
                        IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                        keyValue.Id = currentComponent;
                        keyValue.AddValue(SpecialValues.DummyMemberValue);
                        region.AddKeyValue(keyValue);
                    }
                    criteria.IncludedCubeRegion = region;
                    criterias.Add(criteria);
                }

                #endregion
                codes = GetSDMXObject.GetCodelist(df, kf, component, criterias, Contrained);
            }

            return codes;
        }
        private ICodelistObject GetTimeCodeList(ICodelistObject FreqCodelist, IDataflowObject df, IDataStructureObject kf)
        {

            ICodelistObject CL_TIME_MA = GetCodeList(df, kf, kf.TimeDimension);
            if (CL_TIME_MA == null || CL_TIME_MA.Items == null || CL_TIME_MA.Items.Count != 2)
                return CL_TIME_MA;

            //string format_time = (CL_TIME_MA.Items[0].Id.Contains("-")) ? "yyyy-MM-dd" : "yyyy";
            string time_min_normal = CL_TIME_MA.Items[0].Id;
            string time_max_normal = CL_TIME_MA.Items[1].Id;

            string FrequencyDominant = null;
            if (CodemapObj.PreviusCostraint != null
                && CodemapObj.PreviusCostraint.ContainsKey(kf.FrequencyDimension.Id))
                FrequencyDominant = CodemapObj.PreviusCostraint[kf.FrequencyDimension.Id].First();
            if (FrequencyDominant == null)
            {
                FrequencyDominant = "A";
                if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "S") > 0) FrequencyDominant = "S";
                if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "Q") > 0) FrequencyDominant = "Q";
                if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "M") > 0) FrequencyDominant = "M";
            }
            if (!CL_TIME_MA.Items[0].Id.Contains("-"))
            {
                time_min_normal = string.Format("{0}-{1}-{2}", CL_TIME_MA.Items[0].Id, "01", "01");
                time_max_normal = string.Format("{0}-{1}-{2}", CL_TIME_MA.Items[1].Id, "01", "01");
            }
            else
            {
                var time_p_c = CL_TIME_MA.Items[0].Id.Split('-');
                if (time_p_c.Length == 2)
                {
                    int mul =
                        (FrequencyDominant == "M") ? 1 :
                        (FrequencyDominant == "Q") ? 3 :
                        (FrequencyDominant == "S") ? 6 : 0;

                    int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                    time_p_c = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("S"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("Q"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("M"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
            }


            DateTime MinDate = DateTime.ParseExact(time_min_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
            DateTime MaxDate = DateTime.ParseExact(time_max_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

            ICodelistMutableObject CL_TIME = new CodelistMutableCore();
            CL_TIME.Id = CL_TIME_MA.Id;
            CL_TIME.AgencyId = CL_TIME_MA.AgencyId;
            CL_TIME.Version = CL_TIME_MA.Version;

            CL_TIME_MA.Names.ToList().ForEach(n => CL_TIME.AddName(n.Locale, n.Value));

            DateTime ActualDate = MinDate;
            switch (FrequencyDominant)
            {
                case "A":
                    #region Aggiungo gli Annual
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString();
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddYears(1);
                    }
                    #endregion
                    break;
                case "S":
                    #region Aggiungo gli Semestrali
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString() + "-S" + (ActualDate.Month < 6 ? "1" : "2");
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(6);
                    }
                    #endregion
                    break;
                case "Q":
                    #region Aggiungo i Quartely
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString() + "-Q" + ((ActualDate.Month - 1) / 3 + 1).ToString();
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(3);
                    }
                    #endregion
                    break;
                case "M":
                    #region Aggiungo i Mensili
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.ToString("yyyy-MM");
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(1);
                    }
                    #endregion
                    break;
                default:
                    break;
            }

            return CL_TIME.ImmutableInstance;
        }

        private ICodelistObject GetCodeListCostraint(
            IDataflowObject df,
            IDataStructureObject kf,
            IComponent component)
        {
            ICodelistObject codes = null;

            bool Contrained = component.StructureType.EnumType != Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.DataAttribute;

            #region Criteria
            List<IContentConstraintMutableObject> criterias = new List<IContentConstraintMutableObject>();
            if (Contrained)
            {
                string currentComponent = component.ConceptRef.ChildReference.Id;

                IContentConstraintMutableObject criteria = new ContentConstraintMutableCore();
                criteria.Id = currentComponent ?? "SPECIAL";
                criteria.AddName("en", "english");
                criteria.AgencyId = "agency";

                ICubeRegionMutableObject region = new CubeRegionMutableCore();

                if (currentComponent != null)
                {
                    IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                    keyValue.Id = currentComponent;
                    keyValue.AddValue(SpecialValues.DummyMemberValue);
                    region.AddKeyValue(keyValue);

                    if (CodemapObj.PreviusCostraint != null)
                    {
                        foreach (string costreintKey in CodemapObj.PreviusCostraint.Keys)
                        {
                            if (costreintKey == currentComponent) continue;
                            if (costreintKey == kf.TimeDimension.Id)
                            {

                                // Qui considerare il caso in qui in CodemapObj.PreviusCostraint[costreintKey][0] ci sia solo un valore, ke equivale alla data da.
                                if (CodemapObj.PreviusCostraint[costreintKey].Count > 1)
                                //if (!string.IsNullOrEmpty(CodemapObj.PreviusCostraint[costreintKey][1]))
                                {
                                    DateTime MinDate = GetDateTimeFromSDMXTimePeriod(CodemapObj.PreviusCostraint[costreintKey][0].ToString(), 'M'); //DateTime.ParseExact(CodemapObj.PreviusCostraint[costreintKey][0].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
                                    DateTime MaxDate = GetDateTimeFromSDMXTimePeriod(CodemapObj.PreviusCostraint[costreintKey][1].ToString(), 'M'); //DateTime.ParseExact(CodemapObj.PreviusCostraint[costreintKey][1].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

                                    criteria.StartDate = MinDate;
                                    criteria.EndDate = MaxDate;
                                }
                            }
                            else
                            {
                                foreach (var code in CodemapObj.PreviusCostraint[costreintKey])
                                {
                                    IKeyValuesMutable _keyValue = new KeyValuesMutableImpl();
                                    _keyValue.Id = costreintKey;
                                    _keyValue.AddValue(code);
                                    region.AddKeyValue(_keyValue);
                                }
                            }
                        }
                    }

                }
                criteria.IncludedCubeRegion = region;
                criterias.Add(criteria);


            }
            #endregion

            codes = GetSDMXObject.GetCodelist(df, kf, component, criterias, Contrained);

            return codes;
        }

        private ICodelistObject GetTimeCodeListCostraint(ICodelistObject FreqCodelist, IDataflowObject df, IDataStructureObject kf)
        {

            ICodelistObject CL_TIME_MA = GetCodeListCostraint(df, kf, kf.TimeDimension);
            if (CL_TIME_MA == null
                || CL_TIME_MA.Items == null
                || CL_TIME_MA.Items.Count != 2)
                return CL_TIME_MA;

            //string format_time = (CL_TIME_MA.Items[0].Id.Contains("-")) ? "yyyy-MM-dd" : "yyyy";
            string time_min_normal = CL_TIME_MA.Items[0].Id;
            string time_max_normal = CL_TIME_MA.Items[1].Id;

            string FrequencyDominant = null;
            if (CodemapObj.PreviusCostraint != null
                && CodemapObj.PreviusCostraint.ContainsKey(kf.FrequencyDimension.Id))
                FrequencyDominant = CodemapObj.PreviusCostraint[kf.FrequencyDimension.Id].First();
            if (FrequencyDominant == null)
            {
                FrequencyDominant = "A";
                if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "S") > 0) FrequencyDominant = "S";
                if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "Q") > 0) FrequencyDominant = "Q";
                if (FreqCodelist.Items.Count(c => c.Id.ToUpper() == "M") > 0) FrequencyDominant = "M";
            }
            if (!CL_TIME_MA.Items[0].Id.Contains("-"))
            {
                time_min_normal = string.Format("{0}-{1}-{2}", CL_TIME_MA.Items[0].Id, "01", "01");
                time_max_normal = string.Format("{0}-{1}-{2}", CL_TIME_MA.Items[1].Id, "01", "01");
            }
            else
            {
                var time_p_c = CL_TIME_MA.Items[0].Id.Split('-');
                if (time_p_c.Length == 2)
                {
                    int mul =
                        (FrequencyDominant == "M") ? 1 :
                        (FrequencyDominant == "Q") ? 3 :
                        (FrequencyDominant == "S") ? 6 : 0;

                    int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                    time_p_c = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("S"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("Q"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (CL_TIME_MA.Items[0].Id.Contains("M"))
                {
                    var time_p = CL_TIME_MA.Items[0].Id.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_min_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_p = CL_TIME_MA.Items[1].Id.Split('-');
                    t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_max_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
            }

            DateTime MinDate = DateTime.ParseExact(time_min_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
            DateTime MaxDate = DateTime.ParseExact(time_max_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

            ICodelistMutableObject CL_TIME = new CodelistMutableCore();
            CL_TIME.Id = CL_TIME_MA.Id;
            CL_TIME.AgencyId = CL_TIME_MA.AgencyId;
            CL_TIME.Version = CL_TIME_MA.Version;

            CL_TIME_MA.Names.ToList().ForEach(n => CL_TIME.AddName(n.Locale, n.Value));

            DateTime ActualDate = MinDate;
            switch (FrequencyDominant)
            {
                case "A":
                    #region Aggiungo gli Annual
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString();
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddYears(1);
                    }
                    #endregion
                    break;
                case "S":
                    #region Aggiungo gli Semestrali
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString() + "-S" + (ActualDate.Month < 6 ? "1" : "2");
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(6);
                    }
                    #endregion
                    break;
                case "Q":
                    #region Aggiungo i Quartely
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.Year.ToString() + "-Q" + ((ActualDate.Month - 1) / 3 + 1).ToString();
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(3);
                    }
                    #endregion
                    break;
                case "M":
                    #region Aggiungo i Mensili
                    while (ActualDate.CompareTo(MaxDate) <= 0)
                    {
                        ICodeMutableObject code = new CodeMutableCore();
                        code.Id = ActualDate.ToString("yyyy-MM");
                        code.AddName("en", code.Id);
                        CL_TIME.AddItem(code);

                        ActualDate = ActualDate.AddMonths(1);
                    }
                    #endregion
                    break;
                default:
                    break;
            }

            return CL_TIME.ImmutableInstance;
        }

        private DateTime GetDateTimeFromSDMXTimePeriod(string SDMXTime, char FrequencyDominant)
        {

            string time_normal = string.Empty;

            if (!SDMXTime.Contains("-"))
            {
                time_normal = string.Format("{0}-{1}-{2}", SDMXTime, "01", "01");
            }
            else
            {
                var time_p_c = SDMXTime.Split('-');

                if (time_p_c.Length == 2)
                {
                    int mul =
                        (FrequencyDominant == 'M') ? 1 :
                        (FrequencyDominant == 'Q') ? 3 :
                        (FrequencyDominant == 'S') ? 6 : 0;

                    int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    time_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (SDMXTime.Contains("S"))
                {
                    var time_p = SDMXTime.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (SDMXTime.Contains("Q"))
                {
                    var time_p = SDMXTime.Split('-');
                    int t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (SDMXTime.Contains("M"))
                {
                    var time_p = SDMXTime.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
            }
            return DateTime.ParseExact(time_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
        }
    }
}

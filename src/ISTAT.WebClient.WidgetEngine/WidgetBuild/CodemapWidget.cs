using Estat.Sri.CustomRequests.Constants;
using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
//using ISTAT.WebClient.WidgetComplements.Model.NSIWC;
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
using System.Web.SessionState;
using System.Web.Mvc;
using System.Configuration;
using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
using ISTAT.WebClient;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class CodemapWidget : Controller
    {
        private GetCodemapObject CodemapObj { get; set; }
        internal SessionImplObject SessionObj { get; set; }
        private IGetSDMX GetSDMXObject = null;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CodemapWidget));
        private static readonly string _emptyJSON = new JavaScriptSerializer().Serialize(string.Empty);

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



        public SessionImplObject GetCodemap(SessionQuery query, ConnectionStringSettings connectionStringSetting)
        {
            try
            {
                ISdmxObjects structure = query.Structure;// GetDsd();
                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();
                

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");
                if (df == null)
                    throw new InvalidOperationException("Dataflow is not set");

                Dictionary<string, ICodelistObject> ConceptCodelists = GetCodelistMap(query, false);

                if (this.SessionObj.SdmxObject!=null )
                { this.SessionObj.SdmxObject.Codelists.Clear(); 

                foreach (ICodelistObject codelist in ConceptCodelists.Values)
                    this.SessionObj.SdmxObject.AddCodelist(codelist);
                }

                /*check if exist connection and one template*/
                TemplateWidget templateWidget = new TemplateWidget();
                var template = new TemplateObject();

                ///if (connectionStringSetting.ConnectionString!=null && connectionStringSetting.ConnectionString.ToLower() != "file")
                if (connectionStringSetting.ConnectionString != null)
                {
                    templateWidget = new TemplateWidget(connectionStringSetting.ConnectionString);
                    template = templateWidget.GetSingle(new GetTemplateObject()
                    {
                        Template = new TemplateObject()
                        {
                            Dataflow = CodemapObj.Dataflow,
                            Configuration = CodemapObj.Configuration,
                        }
                    });
                }
                else
                { template = null; }

                
                CodemapSpecificResponseObject codemapret = new CodemapSpecificResponseObject()
                {
                    codemap = ParseCodelist(ConceptCodelists),
                    costraint = (template != null) ? template.Criteria : null,
                    hideDimension = (template != null) ? template.HideDimension : null,
                    //enabledVar = (template != null) ? template.EnableVaration : true,
                    enabledVar = (template != null) ? template.EnableVaration : false,
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



        public SessionImplObject GetSpecificCodemap(bool firstDimension, ConnectionStringSettings connectionStringSetting, SessionQuery query)
        {
            try
            {
                ISdmxObjects structure;
                //ISdmxObjects structure = query._structure;
                if (query.Structure == null)
                { structure = GetDsd(); }
                else
                { structure = 
                    query.Structure; }

                IDataflowObject df = structure.Dataflows.First();
                IDataStructureObject kf = structure.DataStructures.First();
                
                query.Structure = structure;
                query.Dataflow = df;

                if (kf == null)
                    throw new InvalidOperationException("DataStructure is not set");
                

                TemplateWidget templateWidget = new TemplateWidget();
                var template = new TemplateObject();
                //if (connectionStringSetting.ConnectionString !=null && connectionStringSetting.ConnectionString.ToLower() != "file")
                if (connectionStringSetting.ConnectionString != null)
                {
                    templateWidget = new TemplateWidget(connectionStringSetting.ConnectionString);
                    template = templateWidget.GetSingle(new GetTemplateObject()
                    {
                        Template = new TemplateObject()
                        {
                            Dataflow = CodemapObj.Dataflow,
                            Configuration = CodemapObj.Configuration,
                        }
                    });
                }
                else
                { template = null; }

                string dimension = null;
                Dictionary<string, ICodelistObject> ConceptCodelists = null;
                
                // Se ha una template forzo il retrive di tutte le codelist per evitare problemi in stampa tabella
                if (template!=null)
                {
                    ConceptCodelists = this.GetCodelistMap(query, false);
                    query.Criteria = template.Criteria;

                    if (template.Criteria.ContainsKey(kf.TimeDimension.Id))
                    {
                        List<string> typeCheck = template.Criteria[kf.TimeDimension.Id] as List<string>;
                        if (typeCheck != null)
                            typeCheck.Sort();
                        query.SetCriteriaTime(typeCheck);
                    }
                }
                else
                 {
                    dimension = (firstDimension) ? kf.DimensionList.Dimensions.FirstOrDefault().Id : CodemapObj.Codelist;                    
                    IComponent component = kf.GetComponent(dimension);
                    ICodelistObject ConceptCodelistsComponent = GetCodeList(query, component);
                    //se chiedo tutte le codelist insieme
                    //ConceptCodelists = GetCodelistMap(query, false);
                    //se chiedo una codelist alla volta
                    //ConceptCodelists = GetCodelistMap(component.Id, df, kf;
                    ConceptCodelists = GetCodelistMap(component.Id, df, kf,query);


                  }


                CodemapSpecificResponseObject codemapret = new CodemapSpecificResponseObject()
                {
                    codemap = ParseCodelist(ConceptCodelists,kf,query),
                    costraint = (template != null) ? template.Criteria : null,
                    hideDimension = (template != null) ? template.HideDimension : null,
                    //enabledVar = (template != null) ? template.EnableVaration : true,
                    enabledVar = (template != null) ? template.EnableVaration : false,
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
             SessionQuery query,
             bool withAttribute)
        {
            Dictionary<string, ICodelistObject> Conceptcodelist = new Dictionary<string, ICodelistObject>();
            IDataflowObject df = query._dataflow;
            IDataStructureObject kf = query._keyFamily;

            if (kf != null && df != null)
            {
                RetrieveMissingCodeslists(query);

                foreach (IDimension component in
                    kf.DimensionList.Dimensions.Where(
                    c => c.HasCodedRepresentation()
                        && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)))
                    Conceptcodelist.Add(component.Id, this.GetCodeList(query, component));


                if (withAttribute && kf.AttributeList != null)
                {
                    foreach (IComponent component in kf.AttributeList.Attributes.Where(c => c.HasCodedRepresentation() && !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId)))
                        Conceptcodelist.Add(component.Id, this.GetCodeList(query, component));
                }

                if (Conceptcodelist.ContainsKey(kf.FrequencyDimension.Id) == true
                    && Conceptcodelist[kf.FrequencyDimension.Id] != null)                    
                    Conceptcodelist.Add(kf.TimeDimension.Id, this.GetTimeCodeList(Conceptcodelist[kf.FrequencyDimension.Id], df, kf,query));

                if (this.SessionObj.CodelistConstrained == null) this.SessionObj.CodelistConstrained = new Dictionary<string, Dictionary<string, ICodelistObject>>();
                this.SessionObj.CodelistConstrained[Utils.MakeKey(df)] = Conceptcodelist;

            }
            return Conceptcodelist;
        }

        public Dictionary<string, ICodelistObject> GetCodelistMap(
            string dim,
            IDataflowObject df,
            IDataStructureObject kf,
            SessionQuery query)
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



                //fabio nuovo 25/11/2015
                //var dimCodelistSession = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == dimCodelist.Representation.Representation.MaintainableReference.MaintainableId).FirstOrDefault();
                var dimCodelistSession = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == dimCodelist.ConceptRef.FullId).FirstOrDefault();
                if (dimCodelistSession != null)
                { Conceptcodelist[dimCodelist.Id] = dimCodelistSession; }
                else
                {
                    //Conceptcodelist[dimCodelist.Id] = this.GetCodeListCostraint(df, kf, dimCodelist);
                    Conceptcodelist[dimCodelist.Id] = GetCodeList(query, dimCodelist);
                }
                //fine 
            }
            else
            {
                //fabio nuovo 25/11/2015
                var freqCodelist = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == kf.FrequencyDimension.Representation.Representation.MaintainableId).FirstOrDefault();
                if (freqCodelist != null)
                { Conceptcodelist[kf.FrequencyDimension.Id] = freqCodelist; }
                else
                { freqCodelist = (Conceptcodelist[kf.FrequencyDimension.Id] != null) ? Conceptcodelist[kf.FrequencyDimension.Id] : this.GetCodeListCostraint(df, kf, kf.FrequencyDimension); }

                var TimeDimensionCodelist = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == "CL_" + kf.TimeDimension.Id).FirstOrDefault();

                if (TimeDimensionCodelist != null)
                { Conceptcodelist[kf.TimeDimension.Id] = TimeDimensionCodelist; }
                else
                { Conceptcodelist[kf.TimeDimension.Id] = this.GetTimeCodeListCostraint(freqCodelist, df, kf); }
                //fine

                //var freqCodelist = (Conceptcodelist[kf.FrequencyDimension.Id] != null) ? Conceptcodelist[kf.FrequencyDimension.Id] : this.GetCodeListCostraint(df, kf, kf.FrequencyDimension);
                //Conceptcodelist[kf.TimeDimension.Id] = this.GetTimeCodeListCostraint(freqCodelist, df, kf);
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

                /*var dimCodelist =
                   kf.DimensionList.Dimensions.FirstOrDefault(
                   c => c.HasCodedRepresentation()
                       && c.ConceptRef.FullId == dim);*/


                //fabio nuovo 25/11/2015
                //var dimCodelistSession = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == dimCodelist.Representation.Representation.MaintainableReference.MaintainableId).FirstOrDefault();
                var dimCodelistSession = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == dimCodelist.ConceptRef.FullId).FirstOrDefault();
                if (dimCodelistSession != null)
                { Conceptcodelist[dimCodelist.Id] = dimCodelistSession; }
                else
                { Conceptcodelist[dimCodelist.Id] = this.GetCodeListCostraint(df, kf, dimCodelist);
                }
                //fine 
            }
            else
            {
                //fabio nuovo 25/11/2015
                var freqCodelist = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == kf.FrequencyDimension.Representation.Representation.MaintainableId).FirstOrDefault();
                if (freqCodelist != null)
                { Conceptcodelist[kf.FrequencyDimension.Id] = freqCodelist; }
                else
                { freqCodelist = (Conceptcodelist[kf.FrequencyDimension.Id] != null) ? Conceptcodelist[kf.FrequencyDimension.Id] : this.GetCodeListCostraint(df, kf, kf.FrequencyDimension); }

                var TimeDimensionCodelist = this.SessionObj.SdmxObject.Codelists.Where(c => c.Id == "CL_"+kf.TimeDimension.Id).FirstOrDefault();

                if (TimeDimensionCodelist != null)
                { Conceptcodelist[kf.TimeDimension.Id] = TimeDimensionCodelist; }
                else
                { Conceptcodelist[kf.TimeDimension.Id] = this.GetTimeCodeListCostraint(freqCodelist, df, kf); }
                //fine

                //var freqCodelist = (Conceptcodelist[kf.FrequencyDimension.Id] != null) ? Conceptcodelist[kf.FrequencyDimension.Id] : this.GetCodeListCostraint(df, kf, kf.FrequencyDimension);
                //Conceptcodelist[kf.TimeDimension.Id] = this.GetTimeCodeListCostraint(freqCodelist, df, kf);
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
            //fabio nuovo 25/11/2015
            if (dataflow != null && this.SessionObj.SdmxObject.HasConceptSchemes)
            {
                 Structure=this.SessionObj.SdmxObject;
            }
            else if (dataflow != null && !this.SessionObj.SdmxObject.HasConceptSchemes)
            {
                Structure = GetSDMXObject.GetStructure(dataflow, this.SessionObj.SdmxObject.DataStructures);
                Structure.AddDataflow(dataflow);
            }
            else
            {
                Structure = GetSDMXObject.GetStructure(this.CodemapObj.Dataflow.id, this.CodemapObj.Dataflow.agency, this.CodemapObj.Dataflow.version);
            }
            //fine
            if (this.SessionObj.SdmxObject != null)
                this.SessionObj.SdmxObject.Merge(Structure);
            else this.SessionObj.SdmxObject = Structure;

            return Structure;
        }


        public int GetCountObservation(SessionQuery query)
        {

            ISdmxObjects structure = query.Structure;
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

                if (query.Criteria != null)
                {
                    foreach (string costreintKey in query.Criteria.Keys)
                    {
                        if (costreintKey == currentComponent) continue;
                        if (costreintKey == kf.TimeDimension.Id)
                        {

                            // Qui considerare il caso in qui in CodemapObj.PreviusCostraint[costreintKey][0] ci sia solo un valore, ke equivale alla data da.
                            if (query.Criteria[costreintKey].Count > 1)
                            {
                                DateTime MinDate = GetDateTimeFromSDMXTimePeriod(query.Criteria[costreintKey][0].ToString(), 'M'); //DateTime.ParseExact(CodemapObj.PreviusCostraint[costreintKey][0].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
                                DateTime MaxDate = GetDateTimeFromSDMXTimePeriod(query.Criteria[costreintKey][1].ToString(), 'M'); //DateTime.ParseExact(CodemapObj.PreviusCostraint[costreintKey][1].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
                                //baco fabio
                                if (MinDate.CompareTo(MaxDate) > 0)
                                {
                                    criteria.StartDate = MaxDate;
                                    criteria.EndDate = MinDate;
                                }
                                else
                                {
                                    criteria.StartDate = MinDate;
                                    criteria.EndDate = MaxDate;
                                }


                            }
                        }
                        else
                        {
                            foreach (var code in query.Criteria[costreintKey])
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
            
            GetSDMXObject = WebServiceSelector.GetSdmxImplementation(this.CodemapObj.Configuration);

            int count = GetSDMXObject.GetDataflowDataCount(df, criteria);

            return count;
        }





        private Dictionary<string, CodemapObj> ParseCodelist(Dictionary<string, ICodelistObject> Codemap, IDataStructureObject kf,SessionQuery query)
        {
            Dictionary<string, CodemapObj> dr_codemap = new Dictionary<string, CodemapObj>();
            //IDataStructureObject kf = query._keyFamily;
            

            foreach (string conceptId in Codemap.Keys)
            {
                ICodelistObject codelist = Codemap[conceptId];
                if (codelist != null)
                {
                    if (this.SessionObj != null && this.SessionObj.SdmxObject != null)
                    { this.SessionObj.SdmxObject.AddCodelist(codelist); }

                    var component =
                       kf.DimensionList.Dimensions.FirstOrDefault(
                       c => c.HasCodedRepresentation()
                           && c.ConceptRef.FullId == conceptId);
                           //&& !string.IsNullOrEmpty(c.Representation.Representation.MaintainableReference.MaintainableId));
                           //&& c.Representation.Representation.MaintainableReference.MaintainableId == Codemap[conceptId].Id);
                           //&& kf.Id == conceptId);
                    
                    if (component != null )
                    {
                        //var conceptScheme = this.SessionObj.SdmxObject.ConceptSchemes.FirstOrDefault(c => c.Id == component.ConceptRef.MaintainableId);
                        var conceptScheme = query.Structure.ConceptSchemes.FirstOrDefault(c => c.Id == component.ConceptRef.MaintainableId);
                        //var conceptScheme = this.GetCodeList(query, component);
                        //GetCodelistMap(query, false);

                        foreach (var concept in conceptScheme.Items)
                        {
                            //if (component.Id == concept.Id)
                           if (component.ConceptRef.FullId == concept.Id)
                           // if (component.ConceptRef.FullId == concept.AsReference.FullId)
                            //if (component.Representation.Representation.MaintainableId == concept.IdentifiableParent.Id)
                            {
                                //if (Codemap[conceptId]==concept){
                                CodemapObj codemap = new CodemapObj()
                                {
                                    title = TextTypeHelper.GetText(concept.Names, this.CodemapObj.Configuration.Locale),
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
                        }

                     }
                     else {  //time_period code_list esiste nel codemap ma non esiste nel concept schema
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
                }
                else { dr_codemap.Add(conceptId, null); }

            }
            return dr_codemap;
        }


        /*ParseCodelist(Dictionary<string, ICodelistObject> Codemap) è errata perchè non riporta i title del concept schema*/
        private Dictionary<string, CodemapObj> ParseCodelist(Dictionary<string, ICodelistObject> Codemap)
        {
            Dictionary<string, CodemapObj> dr_codemap = new Dictionary<string, CodemapObj>();

            foreach (string conceptId in Codemap.Keys)
            {
                //List<string> criteri = new List<string>();
                ICodelistObject codelist = Codemap[conceptId];
                if (codelist != null)
                {

                    if (this.SessionObj != null && this.SessionObj.SdmxObject != null)
                    {
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
     /*NUOVO SESSION QUERY*/   
        /// <summary>
        /// Get a CodeList for the specified component from either <see cref="INsiClient"/> or from <see cref="SessionQuery"/> cache.
        /// </summary>
        /// <param name="query">
        /// The SessionQuery holding the current dataflow and possibly the artefact cache
        /// </param>
        /// <param name="component">
        /// The SDMX DSD component to retrieve the artefact for
        /// </param>
        /// <returns>
        /// A artefact or null
        /// </returns>
        private ICodelistObject GetCodeList(SessionQuery query, IComponent component)
        {
            //INsiClient nsiClient = GetNSIClient(this.Context);
            ICodelistObject codes = query.GetCachedCodelist(component);
            if (codes == null)
            {
                //QueryBuilder queryBuilder = new QueryBuilder(query);
                //codes = IGetSDMX.GetCodelist(query.Dataflow, query.KeyFamily, component, queryBuilder.CreateConstraintBean(component.ConceptRef.ChildReference.Id),true);
                codes = this.GetCodeListCostraint(query.Dataflow, query.KeyFamily, component); 
                query.UpdateCodelistMap(codes, component);
                //NEL CASO DI CODE TIME LIST VENGONO RIPARSATE
                codes = query.GetCachedCodelist(component);
            }
            /*cort code @order@
            var sortedCodes = codes.Items.OrderBy<ICode, int>(
                 o =>
                 {
                     var corder = o.Annotations.FirstOrDefault(mutableObject => string.Equals(mutableObject.Type, "@ORDER@"));
                     return corder != null ? int.Parse(corder.Text[0].Value) : 0;
                 }).ToArray();

            foreach (ICode obj in codes.Items)
                codes.Items.Remove(obj);

            foreach (ICode obj in sortedCodes)
                codes.Items.Add(obj);
            */

            return codes;
        }

        /// <summary>
        /// Retrieve any missing codelists
        /// </summary>
        /// <param name="query">
        /// 
        /// The SessionQuery holding the current dataflow and possibly the artefact cache
        /// </param>
        /// 
        public void RetrieveMissingCodeslists(SessionQuery query)
        {
            foreach (IDimension component in query.KeyFamily.DimensionList.Dimensions)
            {
                if (component.HasCodedRepresentation() && !string.IsNullOrEmpty(component.Representation.Representation.MaintainableReference.MaintainableId))
                {
                    this.GetCodeList(query, component);
                }
            }
        }

        public string ComponentSave(string concept, string[] ids, SessionQuery query)
        {
            //SessionQuery query = SessionQueryManager.GetSessionQuery(HttpContext.Current.Session);
            if (query == null || query.KeyFamily == null)
            {
                throw new InvalidOperationException("query is not set");
            }

            IDimension component = Utils.GetComponentByName(query.KeyFamily, concept);

            ICodelistObject codes = query.GetCachedCodelist(component);
            query.ClearCodeListAfter(component);
            if (ids.Length == 0)
            {
                query.RemoveComponent(component);
                return _emptyJSON;
            }

           // try
            //{
                if (codes == null)
                {
                    //QueryBuilder queryBuilder = new QueryBuilder(query);
                    //INsiClient nsiClient = GetNSIClient(this.Context);
                    //codes = nsiClient.GetCodelist(query.Dataflow, query.KeyFamily, component, queryBuilder.CreateConstraintBean(component.ConceptRef.ChildReference.Id));
                    codes = this.GetCodeListCostraint(query.Dataflow, query.KeyFamily, component); 
                    query.UpdateCodelistMap(codes, component);
                }

                var codesList = new List<ICode>();

                //time dimensione with region period
                if (component.TimeDimension && ids.Length == 1)
                {
                    {
                        int offsetTime = int.Parse(ids.First());
                        int lengthTime = codes.Items.Count;

                        if ((lengthTime - offsetTime) >= 0)
                        {
                            var _codes = codes.Items.Reverse().Take(offsetTime);// codMap[kf.TimeDimension.Id].Reverse().Take(offsetTime);
                            List<string> _criteriaTime = (from c in _codes select c.Id).ToList<string>();

                            codesList.Add((ICode)codes.GetCodeById(_criteriaTime.Last()));
                            codesList.Add((ICode)codes.GetCodeById(_criteriaTime.First()));
/*
                            this.DataObj.Criteria[kf.TimeDimension.Id] = new List<string>();
                            this.DataObj.Criteria[kf.TimeDimension.Id].Add(_criteriaTime.Last());
                            this.DataObj.Criteria[kf.TimeDimension.Id].Add(_criteriaTime.First());
 * */
                        }
                        else
                        {
                            /*
                            this.DataObj.Criteria[kf.TimeDimension.Id] = new List<string>();
                            this.DataObj.Criteria[kf.TimeDimension.Id].Add(codemap[kf.TimeDimension.Id].First().Key);
                            this.DataObj.Criteria[kf.TimeDimension.Id].Add(codemap[kf.TimeDimension.Id].Last().Key);
                             */
                            var _codes = codes.Items.Reverse().Take(lengthTime);// codMap[kf.TimeDimension.Id].Reverse().Take(offsetTime);
                            List<string> _criteriaTime = (from c in _codes select c.Id).ToList<string>();

                            codesList.Add((ICode)codes.GetCodeById(_criteriaTime.First()));
                            codesList.Add((ICode)codes.GetCodeById(_criteriaTime.Last()));                             

                        }
                    }                
                }
                else
                {
                    foreach (string id in ids)
                    {
                        codesList.Add((ICode)codes.GetCodeById(id));
                    }
                }        
    
                    query.SaveComponent(component, codesList);
                return _emptyJSON;
          //  }
        }
        /*FINE NUOVO SESSION QUERY*/   



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

            /*fabio 04/11/2015 v3.0.0.0*/
            if (time_min_normal.CompareTo(time_max_normal) > 0)
            {
                time_max_normal = CL_TIME_MA.Items[0].Id;
                time_min_normal = CL_TIME_MA.Items[1].Id;
            }
            else
            {
                time_max_normal = CL_TIME_MA.Items[1].Id;
                time_min_normal = CL_TIME_MA.Items[0].Id;
            }
            /*fine fabio 04/11/2015 v3.0.0.0*/

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
           //fabio baco 
            //CultureInfo enEn = new CultureInfo("en");
            //DateTime MinDate = DateTime.ParseExact(time_min_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);
            //DateTime MaxDate = DateTime.ParseExact(time_max_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);
            

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


        private ICodelistObject GetTimeCodeList(ICodelistObject FreqCodelist, IDataflowObject df, IDataStructureObject kf,SessionQuery query)
        {

            ICodelistObject CL_TIME_MA = this.GetCodeList(query, kf.TimeDimension);
            if (CL_TIME_MA == null || CL_TIME_MA.Items == null || CL_TIME_MA.Items.Count != 2)
                return CL_TIME_MA;

            //string format_time = (CL_TIME_MA.Items[0].Id.Contains("-")) ? "yyyy-MM-dd" : "yyyy";
            string time_min_normal = CL_TIME_MA.Items[0].Id;
            string time_max_normal = CL_TIME_MA.Items[1].Id;

            /*fabio 04/11/2015 v3.0.0.0*/
            if (time_min_normal.CompareTo(time_max_normal) > 0)
            {
                time_max_normal = CL_TIME_MA.Items[0].Id;
                time_min_normal = CL_TIME_MA.Items[1].Id;
            }
            else
            {
                time_max_normal = CL_TIME_MA.Items[1].Id;
                time_min_normal = CL_TIME_MA.Items[0].Id;
            }
            /*fine fabio 04/11/2015 v3.0.0.0*/

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
            //fabio baco 
            //CultureInfo enEn = new CultureInfo("en");
            //DateTime MinDate = DateTime.ParseExact(time_min_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);
            //DateTime MaxDate = DateTime.ParseExact(time_max_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);


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
 //           ICodelistObject codes = GetCodeList(query, component);

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

                                    if (MinDate.CompareTo(MaxDate) > 0)
                                    {
                                        criteria.StartDate = MaxDate;
                                        criteria.EndDate = MinDate;
                                    }
                                    else
                                    {
                                        criteria.StartDate = MinDate;
                                        criteria.EndDate = MaxDate;
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

            /*fabio 04/11/2015 v3.0.0.0*/            
            if (time_min_normal.CompareTo(time_max_normal) > 0)
            {
                time_max_normal = CL_TIME_MA.Items[0].Id;
                time_min_normal = CL_TIME_MA.Items[1].Id;
            }
            else
            {
                time_max_normal = CL_TIME_MA.Items[1].Id;
                time_min_normal = CL_TIME_MA.Items[0].Id;
            }
            /*fine fabio 04/11/2015 v3.0.0.0*/



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

            /*CultureInfo enEn = new CultureInfo("en");
            DateTime MinDate = DateTime.ParseExact(time_min_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);
            DateTime MaxDate = DateTime.ParseExact(time_max_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);
            */

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


                    //int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    //time_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                    /*fabio 04/11/2015 v3.0.0.0*/
                    int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    string time_normal_qs = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_normal =
                        (FrequencyDominant == 'M') ? string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01") :
                        (FrequencyDominant == 'Q') ? time_normal_qs :
                        (FrequencyDominant == 'S') ? time_normal_qs : string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01");
                    /*fine fabio 04/11/2015 v3.0.0.0*/
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
            //CultureInfo enEn = new CultureInfo("en");
            //return DateTime.ParseExact(time_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);

            return DateTime.ParseExact(time_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

        }



    }
}

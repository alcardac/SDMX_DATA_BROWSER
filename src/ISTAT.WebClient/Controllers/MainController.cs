using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using ISTAT.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;




using System.IO;
using System.Globalization;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model;
using System.Configuration;
using System.Web.Script.Serialization;
using ISTAT.WebClient.WidgetEngine.Model.DataRender;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using ISTAT.WebClient.WidgetEngine.Model.DBData;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetEngine.Model;
using ISTAT.WebClient.WidgetComplements.Model.JSObject.Input;
using ISTAT.WebClient.WidgetComplements.Model.JSObject.Output;
using System.Web.SessionState;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Estat.Sdmxsource.Extension.Constant;
using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
//using Estat.Nsi.Client;

namespace ISTAT.WebClient.Controllers
{         


    public static class JSONConst {
        public readonly static string Error = "{\"error\" : true }";
        public readonly static string ErrorMaxRecords = "{\"error\" : true }";
        public readonly static string Success = "{\"success\" : true }";
    }

    public class MainController : Controller
    {

        // Utility
        public static string RemoveSpecialCharacters(string str)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private ControllerSupport CS = new ControllerSupport();
        private bool UseWidgetCache = WebClientSettings.Instance.UseWidgetCache;

        private ConnectionStringSettings connectionStringSetting
        {
            get
            {

                ConnectionStringSettings connectionStringSetting = new ConnectionStringSettings();
                
                if (ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"].ConnectionString.ToLower() != "file")
                {
                    connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                    if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                        throw new Exception("ConnectionString not set");
                }
                else
                { connectionStringSetting.ConnectionString = null; }

                return connectionStringSetting;
            }
        }

        string SESSION_KEY = "StructureCache";

        public ActionResult ClearSession() {
            // Clear session on change ws
            Session[SESSION_KEY] = null;

            var query = new SessionQuery { CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture };
            if (SessionQueryManager.SessionQueryExistsAndIsValid(Session))
            {
                query = SessionQueryManager.GetSessionQuery(Session);
            }
            query.Reset();

            return CS.ReturnForJQuery(JSONConst.Success);
        }
        public ActionResult IsCachingDataSet()
        {
            GetDataObject PostDataArrived = CS.GetPostData<GetDataObject>(this.Request);
            CacheWidget cache = new CacheWidget(connectionStringSetting.ConnectionString);

            /*reset query*/
            var query = new SessionQuery { CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture };
            if (SessionQueryManager.SessionQueryExistsAndIsValid(Session))
            {
                query = SessionQueryManager.GetSessionQuery(Session);
            }
            query.Reset();


            if (PostDataArrived.WidgetId > 0 && UseWidgetCache)
            {
                if (cache.IsCachedWidget(PostDataArrived.WidgetId, System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Trim()))
                    return CS.ReturnForJQuery(JSONConst.Success);
            }

            return CS.ReturnForJQuery(JSONConst.Error);
        }

        public ActionResult GetTreeLocale()
        {
            try
            {
                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);

                EndpointSettings set = query._endpointSettings;
                GetTreeObject TreeObj = new GetTreeObject() { Configuration = set };
                TreeWidget treeWidget = new TreeWidget(TreeObj, null);
                string ret = treeWidget.GetTreeforCache(System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);

                return CS.ReturnForJQuery(ret);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }


        public ActionResult GetTree()
        {
            try
            {
                // Get parameter
                GetTreeObject PostDataArrived = CS.GetPostData<GetTreeObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;


                TreeWidget treeWidget = new TreeWidget(PostDataArrived, connectionStringSetting.ConnectionString);
                SessionImplObject ret = treeWidget.GetTree();


                // Clear session on change ws
                Session[SESSION_KEY] = null;

                /*NUOVO */
                var query = new SessionQuery { CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture };
                if (SessionQueryManager.SessionQueryExistsAndIsValid(Session))
                {
                    query = SessionQueryManager.GetSessionQuery(Session);
                }
                else
                {
                    query = new SessionQuery { CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture };
                    SessionQueryManager.SaveSessionQuery(Session, query);
                }
                query._endpointSettings = PostDataArrived.Configuration;

                // Clear sessionQueryManager              
                ISdmxObjects obj = ret.SdmxObject;

                // Get categories - first without annotations 
                IList<ICategorySchemeObject> categoriesWithAnnotation = new List<ICategorySchemeObject>();
                IList<ICategorySchemeObject> categoriesWithoutAnnotation = new List<ICategorySchemeObject>();

                foreach (var categoryScheme in obj.CategorySchemes)
                {
                    if (categoryScheme.Annotations.Count > 0 && categoryScheme.Annotations[0].FromAnnotation() == CustomAnnotationType.CategorySchemeNodeOrder)
                    {
                        categoriesWithAnnotation.Add(categoryScheme);
                    }
                    else
                    {
                        categoriesWithoutAnnotation.Add(categoryScheme);
                    }
                }

                IEnumerable<ICategorySchemeObject> categoriesWithAnnotationOrderedBy = categoriesWithAnnotation.OrderBy(category => Convert.ToInt64(category.Annotations[0].ValueFromAnnotation()));

                IEnumerable<ICategorySchemeObject> categories = categoriesWithoutAnnotation.Concat(categoriesWithAnnotationOrderedBy);

                //Get dataflows
                ISet<IDataflowObject> dataflows = obj.Dataflows;

                // Get categorisations - first without annotations
                IList<ICategorisationObject> categorisationsWithAnnotation = new List<ICategorisationObject>();
                IList<ICategorisationObject> categorisationsWithoutAnnotation = new List<ICategorisationObject>();

                foreach (var categorisation in obj.Categorisations)
                {
                    if (categorisation.Annotations.Count > 0 && categorisation.Annotations[0].FromAnnotation() == CustomAnnotationType.CategorySchemeNodeOrder)
                    {
                        categorisationsWithAnnotation.Add(categorisation);
                    }
                    else
                    {
                        categorisationsWithoutAnnotation.Add(categorisation);
                    }
                }

                IEnumerable<ICategorisationObject> categorisationsWithAnnotationOrderedBy = categorisationsWithAnnotation.OrderBy(categ => Convert.ToInt64(categ.Annotations[0].ValueFromAnnotation()));

                IEnumerable<ICategorisationObject> categorisations = categorisationsWithoutAnnotation.Concat(categorisationsWithAnnotationOrderedBy);

                query.InitializeDataflowMap(dataflows);

                // Get dsds
                ISet<IDataStructureObject> dataStructure = obj.DataStructures;
                query.ResetDsds();
                query.Dsds = dataStructure;

                query.SetDataflowTree(categories, dataflows, categorisations, dataStructure);

                /*FINE NUOVO*/

                /*old*/
                return CS.ReturnForJQuery(ret.SavedTree);
                //return CS.ReturnForJQuery(query._dataflowTreeList.GetJSTree());
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

        public ActionResult GetCodemap()
        {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                // Clear session in All Codelist Costraint mode criteria
                Session[SESSION_KEY] = null;

                CodemapWidget codemapWidget = new CodemapWidget(PostDataArrived, null);

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                SessionImplObject ret = codemapWidget.GetCodemap(query,connectionStringSetting);

                // store current SessionImplObject in session
                Session[SESSION_KEY] = ret;

                return CS.ReturnForJQuery(ret.SavedCodemap);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }


        /*update slice*/
        public ActionResult GetCodemapLayout()
        {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                string selectionKey = PostDataArrived.Selectionkey;
                string selectionVal = PostDataArrived.Selectionval;

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                query.DatasetModel.UpdateSliceKeyValue(selectionKey, selectionVal);
                
                return CS.ReturnForJQuery(ModelJson(query));

            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }


        /*get CODELIST with mode "COSTRAINT_NO_LIMIT"*/
        public ActionResult GetCodemapCostraintNoLimit()
        {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                // Clear session in All Codelist Costraint mode criteria
                Session[SESSION_KEY] = null;

                CodemapWidget codemapWidget = new CodemapWidget(PostDataArrived, null);

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                SessionImplObject ret = codemapWidget.GetCodemap(query,connectionStringSetting);

                // store current SessionImplObject in session
                Session[SESSION_KEY] = ret;

                return CS.ReturnForJQuery(ret.SavedCodemap);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

        public ActionResult GetSpecificCodemap() {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                // Check if a SessionImplObject is store in session
                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();

                //Clear query datasetmodel and store
                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                query.ClearData();

                CodemapWidget codemapWidget = new CodemapWidget(PostDataArrived, sdmxObj);

                SessionImplObject ret = codemapWidget.GetSpecificCodemap(string.IsNullOrEmpty(PostDataArrived.Codelist) ? true : false, connectionStringSetting,query);
                
                // store current SessionImplObject in session
                if (Session[SESSION_KEY] == null) Session[SESSION_KEY] = ret;
                else ((SessionImplObject)Session[SESSION_KEY]).MergeObject(ret);

                return CS.ReturnForJQuery(ret.SavedCodemap);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }
        public ActionResult SetCostraint()
        {
            try
            {

                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();


                CodemapWidget codemapWidget = new CodemapWidget(PostDataArrived, sdmxObj);
                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                query.Criteria = PostDataArrived.PreviusCostraint;
                
                //Se è il primo constraint è vuoto
                if (PostDataArrived.PreviusCostraint.Count > 0)
                { string ret = codemapWidget.ComponentSave(PostDataArrived.Codelist, PostDataArrived.PreviusCostraint[PostDataArrived.Codelist].ToArray(), query); }

                var count=0;

                return CS.ReturnForJQuery(count);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }

        public ActionResult GetLayout()
        {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;


                // Check if a SessionImplObject is store in session
                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();
                
                LayoutWidget layoutWidget = new LayoutWidget(PostDataArrived, sdmxObj);

                SessionImplObject ret = layoutWidget.GetLayout();

                if (Session[SESSION_KEY] == null) Session[SESSION_KEY] = ret;
                else ((SessionImplObject)Session[SESSION_KEY]).MergeObject(ret);
                
                return CS.ReturnForJQuery(ret.SavedDefaultLayout);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

        public ActionResult GetDefaultLayout()
        {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                // Check if a SessionImplObject is store in session
                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();
                
                LayoutWidget layoutWidget = new LayoutWidget(PostDataArrived, sdmxObj);

                SessionImplObject ret = layoutWidget.GetLayout(connectionStringSetting);

                if (Session[SESSION_KEY] == null) Session[SESSION_KEY] = ret;
                else ((SessionImplObject)Session[SESSION_KEY]).MergeObject(ret);

                return CS.ReturnForJQuery(ret.SavedDefaultLayout);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

        public ActionResult GetData()
        {
            try
            {
                // Not remove this linee
                Utils.App_Data_Path = HttpContext.Server.MapPath("~/App_Data/");

                    GetDataObject PostDataArrived = CS.GetPostData<GetDataObject>(this.Request);
                    PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                    SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();

                    SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                    query._endpointSettings = PostDataArrived.Configuration;

                    CodemapWidget codemapWidget = new CodemapWidget(new GetCodemapObject()
                    {
                        Configuration = PostDataArrived.Configuration
                    }, sdmxObj);

                        CacheWidget cache = new CacheWidget(connectionStringSetting.ConnectionString);

                        string CodeForStreaming = Guid.NewGuid().ToString();
                        // +++ Caching +++
                        if (PostDataArrived.WidgetId > 0 && UseWidgetCache)
                        {
                            SavedWidget widget = cache.GetWidget(PostDataArrived.WidgetId, System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
                            if (widget != null && !String.IsNullOrEmpty(widget.widgetData))
                            {
                                Session[CodeForStreaming] = widget;
                                return CS.ReturnForJQuery("{\"code\":\"" + CodeForStreaming + "\"}");
                            }
                        }

                        var count = codemapWidget.GetCountObservation(query);
                        long maxResultObs = (long)ISTAT.WebClient.WidgetComplements.Model.Settings.WebClientSettings.Instance.MaxResultObs;

                        if (count <= maxResultObs)
                        {
                        DataWidget dataWidget = new DataWidget(
                            PostDataArrived, sdmxObj,
                            (ConfigurationManager.AppSettings["ParseSDMXAttributes"].ToString().ToLower() == "true"));

                        object DataStream = null;
                        SessionImplObject ret = dataWidget.GetData(out DataStream, query);

                        // store current SessionImplObject in session
                        //nuovo fabio
                        if (Session[SESSION_KEY] == null) Session[SESSION_KEY] = ret;
                        else ((SessionImplObject)Session[SESSION_KEY]).MergeObject(ret);
                        //fine nuovo fabio


                        DataObjectForStreaming dOFS = DataStream as DataObjectForStreaming;
                        if (dOFS != null && PostDataArrived.WidgetId > 0)
                            dOFS.WidgetID = PostDataArrived.WidgetId;

                        Session[CodeForStreaming] = DataStream;

                        return CS.ReturnForJQuery("{\"code\":\"" + CodeForStreaming + "\"}");
                    }
                    else
                        {
                            return CS.ReturnForJQuery(Messages.label_out_max_results + "(max:"+ maxResultObs + " - record:" + count + ")"); 
                        }
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

        public ActionResult GetDataLayout()
        {
            try
            {
                // Not remove this linee
                Utils.App_Data_Path = HttpContext.Server.MapPath("~/App_Data/");

                GetDataObject PostDataArrived = CS.GetPostData<GetDataObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                SessionImplObject sdmxObj =
                 (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();

                DataWidget dataWidget = new DataWidget(
                    PostDataArrived, sdmxObj,
                    (ConfigurationManager.AppSettings["ParseSDMXAttributes"].ToString().ToLower() == "true"));
                
                object DataStream = null;

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);
                SessionImplObject ret = dataWidget.GetData(out DataStream, query);

                if (Session[SESSION_KEY] == null) Session[SESSION_KEY] = ret;
                else ((SessionImplObject)Session[SESSION_KEY]).MergeObject(ret);

                string CodeForStreaming = Guid.NewGuid().ToString();
                Session[CodeForStreaming] = DataStream;

                return CS.ReturnForJQuery("{\"code\":\"" + CodeForStreaming + "\"}");
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }

       
        public StreamResponseAction GetDataTable(string id)
        {
            try
            {
                if (UseWidgetCache)
                {
                    string htmlRet;
                    SavedWidget widget = Session[id] as SavedWidget;

                    if (widget != null && !String.IsNullOrEmpty(widget.widgetData))
                    {
                        htmlRet = widget.widgetData;
                        return new StreamResponseAction(htmlRet);
                    }
                }

                long maxContent = (long)ISTAT.WebClient.WidgetComplements.Model.Settings.WebClientSettings.Instance.MaxResultHTML;

                string CodeForStreaming = id;

                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                System.IO.TextWriter textWriter = new System.IO.StreamWriter(memoryStream);

                System.Globalization.CultureInfo cFrom =
                    new System.Globalization.CultureInfo(
                        (((DataObjectForStreaming)Session[CodeForStreaming]).Configuration.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");
                System.Globalization.CultureInfo cTo =
                    new System.Globalization.CultureInfo(
                        (ConfigurationManager.AppSettings["DecimalCulture"].ToString().Trim() == ".") ? "EN" : "IT");

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);


                DataWidget.StreamDataTable(
                    Session[CodeForStreaming],
                    textWriter,
                    (ConfigurationManager.AppSettings["ParseSDMXAttributes"].ToString().ToLower() == "true"),
                    cFrom, cTo, query);


                textWriter.Flush(); // added this line
                byte[] bytesInStream = memoryStream.ToArray(); // simpler way of converting to array
                memoryStream.Close();

                if (UseWidgetCache)
                {
                    CacheWidget cache = new CacheWidget(connectionStringSetting.ConnectionString);

                    DataObjectForStreaming dOFS = Session[CodeForStreaming] as DataObjectForStreaming;
                    if (dOFS != null && dOFS.WidgetID > 0)
                    {
                        var htmlOutput = System.Text.Encoding.Default.GetString(bytesInStream);
                        cache.InsertWidget(dOFS.WidgetID, htmlOutput, dOFS.Configuration.Locale);
                    }
                }

                if ((bytesInStream.Length / 1000) > maxContent)
                {
                    this.HttpContext.Response.Clear();
                    return new StreamResponseAction("Number of cells too big. Select a less number of cells.");
                }
                else
                {
                    this.HttpContext.Response.Clear();
                    this.HttpContext.Response.ContentType = "text/html";
                    //this.HttpContext.Response.AddHeader("content-disposition", "attachment;    filename=name_you_file.xls");
                    this.HttpContext.Response.BinaryWrite(bytesInStream);
                    this.HttpContext.Response.End();
                }

                return new StreamResponseAction();
            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }
        }
        public ActionResult GetChartData() 
        {
            try
            {
                // Not remove this linee
                Utils.App_Data_Path = HttpContext.Server.MapPath("~/App_Data/");
                
                GetChartObject PostDataArrived = CS.GetPostData<GetChartObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();
                
                System.Globalization.CultureInfo cFrom =
                    new System.Globalization.CultureInfo(
                        (PostDataArrived.Configuration.DecimalSeparator.ToString().Trim() == ".") ? "EN" : "IT");
                System.Globalization.CultureInfo cTo =
                    new System.Globalization.CultureInfo(
                        (ConfigurationManager.AppSettings["DecimalCulture"].ToString().Trim() == ".") ? "EN" : "IT");

                
                ChartWidget dataWidget = new ChartWidget(
                    PostDataArrived,
                    sdmxObj,
                    cFrom, cTo);
                
                SessionImplObject ret = dataWidget.GetDataChart();

                if (Session[SESSION_KEY] == null) Session[SESSION_KEY] = ret;
                else ((SessionImplObject)Session[SESSION_KEY]).MergeObject(ret);
                
                return CS.ReturnForJQuery(ret.SavedChart);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }
        public ActionResult GetMetadata() {
            try
            {
                GetMetaDataObject PostDataArrived = CS.GetPostData<GetMetaDataObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();

                SessionQuery query = SessionQueryManager.GetSessionQuery(Session);                

                if (PostDataArrived.ArtefactType == "*")
                {
                    #region Return summary DSD

                    CodemapWidget codemapWidget = new CodemapWidget(new GetCodemapObject()
                    {
                        Configuration = PostDataArrived.Configuration,
                        Dataflow = new MaintenableObj()
                        {
                            id = PostDataArrived.Artefact.id,
                            agency = PostDataArrived.Artefact.agency,
                            version = PostDataArrived.Artefact.version
                        }
                    }, sdmxObj);
                   /* ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                    if (connectionStringSetting == null
                        || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                        throw new Exception("ConnectionString not set");
                    */
                    //OLD
                    //var _ret = codemapWidget.GetCodemap(connectionStringSetting).SdmxObject;
                    //var _ret = codemapWidget.GetCodemap(query, connectionStringSetting).SdmxObject;
                    var conceptCodelist = codemapWidget.GetCodelistMap(query, false);
                    //sdmxObj.SdmxObject.Merge(_ret);

                    StructureResponceObject _dsd = new StructureResponceObject();
                    //var dsd = sdmxObj.SdmxObject.DataStructures.FirstOrDefault();
                    var dsd = query.Structure.DataStructures.FirstOrDefault();
                    _dsd.Dsd = new DsdObject()
                    {
                        Name = TextTypeHelper.GetText(dsd.Names, PostDataArrived.Configuration.Locale),
                        Desc = TextTypeHelper.GetText(dsd.Descriptions, PostDataArrived.Configuration.Locale),
                        Id = dsd.Id,
                        Agency = dsd.AgencyId,
                        Version = dsd.Version,
                    };
                    _dsd.Concept = new List<ConceptObject>();
                    
                    //foreach (var concept in sdmxObj.SdmxObject.ConceptSchemes)
                    foreach (var concept in query.Structure.ConceptSchemes)
                    {
                        _dsd.Concept.Add(
                            new ConceptObject()
                        {
                            Name = TextTypeHelper.GetText(concept.Names, PostDataArrived.Configuration.Locale),
                            Desc = TextTypeHelper.GetText(concept.Descriptions, PostDataArrived.Configuration.Locale),
                            Id = concept.Id,
                            Agency = concept.AgencyId,
                            Version = concept.Version,
                        });
                    }
                    _dsd.Codelist = new List<CodelistObject>();
                    foreach (var dimension in dsd.DimensionList.Dimensions.Where(c => c.HasCodedRepresentation()))
                    {
                        var codelistObj = new CodelistObject()
                            {
                                Id = (dimension.Representation.Representation.MaintainableReference.HasMaintainableId()) ? dimension.Representation.Representation.MaintainableReference.MaintainableId : string.Empty,
                                Agency = (dimension.Representation.Representation.MaintainableReference.HasAgencyId()) ? dimension.Representation.Representation.MaintainableReference.AgencyId : string.Empty,
                                Version = (dimension.Representation.Representation.MaintainableReference.HasVersion()) ? dimension.Representation.Representation.MaintainableReference.Version : string.Empty,
                            };

                        var codelist =
                        (from c in sdmxObj.SdmxObject.Codelists                      
                         where c.Id.ToString() == codelistObj.Id
                         && c.AgencyId.ToString() == codelistObj.Agency
                         && c.Version.ToString() == codelistObj.Version
                         select c).FirstOrDefault();

                        if (codelist == null) continue;

                        codelistObj.Name = TextTypeHelper.GetText(codelist.Names, PostDataArrived.Configuration.Locale);
                        codelistObj.Desc = TextTypeHelper.GetText(codelist.Descriptions, PostDataArrived.Configuration.Locale);
                        codelistObj.Items = new List<ItemObject>();

                        _dsd.Codelist.Add(codelistObj);
                    }
                    if (dsd.AttributeList != null)
                        foreach (var attribute in dsd.AttributeList.Attributes.Where(c => c.HasCodedRepresentation()))
                        {
                            var codelistObj = new CodelistObject()
                                {
                                    Id = (attribute.Representation.Representation.MaintainableReference.HasMaintainableId()) ? attribute.Representation.Representation.MaintainableReference.MaintainableId : string.Empty,
                                    Agency = (attribute.Representation.Representation.MaintainableReference.HasAgencyId()) ? attribute.Representation.Representation.MaintainableReference.AgencyId : string.Empty,
                                    Version = (attribute.Representation.Representation.MaintainableReference.HasVersion()) ? attribute.Representation.Representation.MaintainableReference.Version : string.Empty,
                                };

                            var codelist =
                            (from c in sdmxObj.SdmxObject.Codelists
                             where c.Id.ToString() == codelistObj.Id
                             && c.AgencyId.ToString() == codelistObj.Agency
                             && c.Version.ToString() == codelistObj.Version
                             select c).FirstOrDefault();

                            if (codelist == null) continue;

                            codelistObj.Name = TextTypeHelper.GetText(codelist.Names, PostDataArrived.Configuration.Locale);
                            codelistObj.Desc = TextTypeHelper.GetText(codelist.Descriptions, PostDataArrived.Configuration.Locale);
                            codelistObj.Items = new List<ItemObject>();

                            _dsd.Codelist.Add(codelistObj);
                        }

                    return CS.ReturnForJQuery(_dsd);

                    #endregion
                }
                else if (PostDataArrived.ArtefactType == "DSD")
                {
                    #region Return dettail DSD
                    DsdResponceObject _dsd = new DsdResponceObject();
                    //var dsd = sdmxObj.SdmxObject.DataStructures.FirstOrDefault();
                    var dsd = query.Structure.DataStructures.FirstOrDefault();
                    _dsd.Dsd = new DsdObject()
                    {
                        Name = TextTypeHelper.GetText(dsd.Names, PostDataArrived.Configuration.Locale),
                        Desc = TextTypeHelper.GetText(dsd.Descriptions, PostDataArrived.Configuration.Locale),
                        Id = dsd.Id,
                        Agency = dsd.AgencyId,
                        Version = dsd.Version,
                    };
                    
                    _dsd.Dimension = new List<DimensionObject>();
                    foreach (var dimension in dsd.DimensionList.Dimensions)
                    {

                        //var conceptScheme = (from c in sdmxObj.SdmxObject.ConceptSchemes
                        var conceptScheme = (from c in query.Structure.ConceptSchemes
                                             where c.Id == dimension.ConceptRef.MaintainableReference.MaintainableId
                                             && c.AgencyId == dimension.ConceptRef.MaintainableReference.AgencyId
                                             && c.Version == dimension.ConceptRef.MaintainableReference.Version
                                             select new
                                             {
                                                 c,
                                                 concept = (from d in c.Items
                                                            where d.Id == dimension.ConceptRef.FullId
                                                            select d).FirstOrDefault()
                                             }).FirstOrDefault();


                        _dsd.Dimension.Add(
                            new DimensionObject()
                            {
                                Concept_Id = (conceptScheme.concept!=null)?conceptScheme.concept.Id:string.Empty,
                                Concept_Name = TextTypeHelper.GetText(conceptScheme.concept.Names, PostDataArrived.Configuration.Locale),

                                ConceptScheme_Id = (conceptScheme.c != null) ? conceptScheme.c.Id : string.Empty,
                                ConceptScheme_Agency = (conceptScheme.c != null) ? conceptScheme.c.AgencyId : string.Empty,
                                ConceptScheme_Version = (conceptScheme.c != null) ? conceptScheme.c.Version : string.Empty,

                                Codelist_Id = (dimension.Representation != null && dimension.Representation.Representation != null) ? (dimension.Representation.Representation.MaintainableReference.HasMaintainableId()) ? dimension.Representation.Representation.MaintainableReference.MaintainableId : string.Empty : string.Empty,
                                Codelist_Agency = (dimension.Representation != null && dimension.Representation.Representation != null) ? (dimension.Representation.Representation.MaintainableReference.HasAgencyId()) ? dimension.Representation.Representation.MaintainableReference.AgencyId : string.Empty : string.Empty,
                                Codelist_Version = (dimension.Representation != null && dimension.Representation.Representation != null) ? (dimension.Representation.Representation.MaintainableReference.HasVersion()) ? dimension.Representation.Representation.MaintainableReference.Version : string.Empty : string.Empty,

                                DimensionType = (dimension.FrequencyDimension)?"Frequency":(dimension.TimeDimension)?"Time Period":(dimension.MeasureDimension)?"Measure":string.Empty,
                                TextFormat = (dimension.Representation != null && dimension.Representation.TextFormat != null && dimension.Representation.TextFormat.TextType != null) ? dimension.Representation.TextFormat.TextType.EnumType.ToString() : string.Empty,
                                
                            });
                    }

                    if (dsd.AttributeList != null)
                    {

                        _dsd.Attribute = new List<AttributeObject>();
                        foreach (var attribute in dsd.AttributeList.Attributes)
                        {
                            //var conceptScheme = (from c in sdmxObj.SdmxObject.ConceptSchemes
                            var conceptScheme = (from c in query.Structure.ConceptSchemes
                                                 where c.Id == attribute.ConceptRef.MaintainableReference.MaintainableId
                                                 && c.AgencyId == attribute.ConceptRef.MaintainableReference.AgencyId
                                                 && c.Version == attribute.ConceptRef.MaintainableReference.Version
                                                 select new
                                                 {
                                                     c,
                                                     concept = (from d in c.Items
                                                                where d.Id == attribute.ConceptRef.FullId
                                                                select d).FirstOrDefault()
                                                 }).FirstOrDefault();
                            _dsd.Attribute.Add(
                                new AttributeObject()
                                {
                                    AttachmentLevel = attribute.AttachmentLevel.ToString(),
                                    AssignmentStatus = attribute.AssignmentStatus.ToString(),
                                    AttributeType = (attribute.Mandatory) ? "Mandatory" : string.Empty,

                                    Concept_Id = (conceptScheme.concept != null) ? conceptScheme.concept.Id : string.Empty,
                                    Concept_Name = TextTypeHelper.GetText(conceptScheme.concept.Names, PostDataArrived.Configuration.Locale),

                                    ConceptScheme_Id = (conceptScheme.c != null) ? conceptScheme.c.Id : string.Empty,
                                    ConceptScheme_Agency = (conceptScheme.c != null) ? conceptScheme.c.AgencyId : string.Empty,
                                    ConceptScheme_Version = (conceptScheme.c != null) ? conceptScheme.c.Version : string.Empty,

                                    Codelist_Id = (attribute.Representation != null && attribute.Representation.Representation != null) ? (attribute.Representation.Representation.MaintainableReference.HasMaintainableId()) ? attribute.Representation.Representation.MaintainableReference.MaintainableId : string.Empty : string.Empty,
                                    Codelist_Agency = (attribute.Representation != null && attribute.Representation.Representation != null) ? (attribute.Representation.Representation.MaintainableReference.HasAgencyId()) ? attribute.Representation.Representation.MaintainableReference.AgencyId : string.Empty : string.Empty,
                                    Codelist_Version = (attribute.Representation != null && attribute.Representation.Representation != null) ? (attribute.Representation.Representation.MaintainableReference.HasVersion()) ? attribute.Representation.Representation.MaintainableReference.Version : string.Empty : string.Empty,

                                    TextFormat = (attribute.Representation != null && attribute.Representation.TextFormat != null && attribute.Representation.TextFormat.TextType != null) ? attribute.Representation.TextFormat.TextType.EnumType.ToString() : string.Empty,

                                });
                        }
                    }
                        _dsd.Measure = new List<MeasureObject>();
                        //foreach (var measure in dsd.MeasureList)
                        var measure = dsd.MeasureList.PrimaryMeasure;
                        {
                            //var conceptScheme = (from c in sdmxObj.SdmxObject.ConceptSchemes
                            var conceptScheme = (from c in query.Structure.ConceptSchemes
                                                 where c.Id == measure.ConceptRef.MaintainableReference.MaintainableId
                                                 && c.AgencyId == measure.ConceptRef.MaintainableReference.AgencyId
                                                 && c.Version == measure.ConceptRef.MaintainableReference.Version
                                                 select new
                                                 {
                                                     c,
                                                     concept = (from d in c.Items
                                                                where d.Id == measure.ConceptRef.FullId
                                                                select d).FirstOrDefault()
                                                 }).FirstOrDefault();
                            _dsd.Measure.Add(
                                new MeasureObject()
                                {
                                    Code = "N/A",
                                    Type ="Primary",
                                    MeasureDimension = "N/A",

                                    Concept_Id = (conceptScheme.concept != null) ? conceptScheme.concept.Id : string.Empty,
                                    Concept_Name = TextTypeHelper.GetText(conceptScheme.concept.Names, PostDataArrived.Configuration.Locale),

                                    ConceptScheme_Id = (conceptScheme.c != null) ? conceptScheme.c.Id : string.Empty,
                                    ConceptScheme_Agency = (conceptScheme.c != null) ? conceptScheme.c.AgencyId : string.Empty,
                                    ConceptScheme_Version = (conceptScheme.c != null) ? conceptScheme.c.Version : string.Empty,

                                    Codelist_Id = (measure.Representation != null && measure.Representation.Representation != null) ? (measure.Representation.Representation.MaintainableReference.HasMaintainableId()) ? measure.Representation.Representation.MaintainableReference.MaintainableId : string.Empty : string.Empty,
                                    Codelist_Agency = (measure.Representation != null && measure.Representation.Representation != null) ? (measure.Representation.Representation.MaintainableReference.HasAgencyId()) ? measure.Representation.Representation.MaintainableReference.AgencyId : string.Empty : string.Empty,
                                    Codelist_Version = (measure.Representation != null && measure.Representation.Representation != null) ? (measure.Representation.Representation.MaintainableReference.HasVersion()) ? measure.Representation.Representation.MaintainableReference.Version : string.Empty : string.Empty,

                                    TextFormat = (measure.Representation != null && measure.Representation.TextFormat != null && measure.Representation.TextFormat.TextType != null) ? measure.Representation.TextFormat.TextType.EnumType.ToString() : string.Empty,

                                });
                        }
                        
                    //}

                    return CS.ReturnForJQuery(_dsd);
                    
                    #endregion
                }
                else if (PostDataArrived.ArtefactType == "CODELIST")
                {
                    #region Return dettail CODELIST

                    CodelistObject cod = new CodelistObject();

                    var codelist =
                        (from c in sdmxObj.SdmxObject.Codelists
                         where c.Id.ToString() == PostDataArrived.Artefact.id.ToString()
                         && c.AgencyId.ToString() == PostDataArrived.Artefact.agency.ToString()
                         && c.Version.ToString() == PostDataArrived.Artefact.version.ToString()
                         select c).FirstOrDefault();

                    cod.Id = codelist.Id;
                    cod.Version = codelist.Version;
                    cod.Agency = codelist.AgencyId;
                    cod.Name = TextTypeHelper.GetText(codelist.Names, PostDataArrived.Configuration.Locale);
                    cod.Desc = TextTypeHelper.GetText(codelist.Descriptions, PostDataArrived.Configuration.Locale);
                    cod.Items = new List<ItemObject>();
                    foreach (var code in codelist.Items)
                    {
                        cod.Items.Add(new ItemObject()
                        {
                            Name = TextTypeHelper.GetText(code.Names, PostDataArrived.Configuration.Locale),
                            Desc = TextTypeHelper.GetText(code.Descriptions, PostDataArrived.Configuration.Locale),
                            Code = code.Id,
                            Parent = code.ParentCode
                        });
                    }

                    return CS.ReturnForJQuery(cod);

                    #endregion
                }
                else if (PostDataArrived.ArtefactType == "CONCEPTSCHEME")
                {
                    #region Return dettail CONCEPTSCHEME

                    ConceptObject conc = new ConceptObject();

                    var conceptScheme=
                        //(from c in sdmxObj.SdmxObject.ConceptSchemes 
                        (from c in query.Structure.ConceptSchemes                         
                        where c.Id.ToString()==PostDataArrived.Artefact.id.ToString() 
                        && c.AgencyId.ToString()==PostDataArrived.Artefact.agency.ToString() 
                        && c.Version.ToString()==PostDataArrived.Artefact.version.ToString()
                        select c).FirstOrDefault();

                    conc.Id = conceptScheme.Id;
                    conc.Version = conceptScheme.Version;
                    conc.Agency = conceptScheme.AgencyId;
                    conc.Name = TextTypeHelper.GetText(conceptScheme.Names, PostDataArrived.Configuration.Locale);
                    conc.Desc = TextTypeHelper.GetText(conceptScheme.Descriptions, PostDataArrived.Configuration.Locale);
                    conc.Items = new List<ItemObject>();
                    foreach (var concept in conceptScheme.Items) {
                        conc.Items.Add(new ItemObject() {
                            Name = TextTypeHelper.GetText(concept.Names, PostDataArrived.Configuration.Locale),
                            Desc = TextTypeHelper.GetText(concept.Descriptions, PostDataArrived.Configuration.Locale),
                            Code=concept.Id,
                        });
                    }

                    return CS.ReturnForJQuery(conc);

                    #endregion                
                }
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
            return CS.ReturnForJQuery(JSONConst.Error);
        }

        public ActionResult Get_DOTSTAT_StructureObject()
        {
            try
            {
                GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;


                SessionImplObject sdmxObj = null;
                DOTSTAT_Widget dotstatWidget = new DOTSTAT_Widget(PostDataArrived, sdmxObj);

                SessionImplObject ret = dotstatWidget.Get_DOTSTAT_CodemapAndLayout(connectionStringSetting);

                return CS.ReturnForJQuery(ret.SavedCodemap);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }

        private static IDataSetStore BuildDBDataSetStore(SessionQuery query, ConnectionStringSettings settings)
        {

            var info = new DBInfo(settings);
            string tempTable = "table_" + Path.GetRandomFileName().Replace(".", "_");
            query.AddToDataDisposeList(info);
            return new DataSetStoreDB(info, tempTable, query.KeyFamily,true,false);
        }

        private static IDataSetStore GetDataSetStore(SessionQuery query)
        {
            IDataSetStore store;
            long size = 0;

            string dataSet = query.GetSdmxMLDataSet(false);
            using (var datasetStream = System.IO.File.Open(dataSet, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                    if (datasetStream.CanSeek)
                    {
                        size = datasetStream.Length / 1024 / 1024;
                    }
                    else
                    {

                        size = (WebClientSettings.Instance.MaxResultObs == 0
                                    ? query.CurrentMax
                                    : Math.Min(WebClientSettings.Instance.MaxResultObs, query.CurrentMax)) / 10000;
                    }
                }
            
            if (size < 10)
            {
                var settings = Constants.InMemoryDBSettings;
                store = BuildDBDataSetStore(query, settings);
            }
            else
            {
                string sqliteFileName = string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.FileDBFileNameFormat,
                    query.GetSessionPrefix(),
                    Path.GetRandomFileName());
                string table = Path.Combine(
                    query.GetCacheFolder().FullName,
                    sqliteFileName);
                var settings = new ConnectionStringSettings(
                    Constants.ConnectionStringSettingsName, string.Format(CultureInfo.InvariantCulture, Constants.FileDBSettingsFormat, table), Constants.SystemDataSqlite);
                store = BuildDBDataSetStore(query, settings);
            }

            return store;
        }


        private string ModelJson(SessionQuery query)
        {
            var ser = new JavaScriptSerializer();
            string deserializedObject = string.Empty;

            var json = new StringBuilder();
            if (query == null)
            {
                json.Append("InvalidSession");
            }
            else if (query.DatasetModel == null)
            {
                json.Append("NoData");
            }
            else
            {

               var dict = query.DatasetModel.SliceKeyValues.Cast<DictionaryEntry>().ToDictionary(k => (string)k.Key, v => (string)v.Value).ToArray();
               var dictionary = dict.ToDictionary(x => x.Key, x => x.Value);
                var dataSetModel = new Dictionary<string, object>(StringComparer.Ordinal)
                    {
                         { "AllValidKeys", query.DatasetModel.AllValidKeys },
                         { "HorizontalKeys", query.DatasetModel.HorizontalKeys },
                         { "HorizontalVerticalKeyCount", query.DatasetModel.HorizontalVerticalKeyCount },
                         { "SliceKeyValidValues", query.DatasetModel.SliceKeyValidValues },
                         { "SliceKeyValues", query.DatasetModel.SliceKeyValues },
                         { "SliceKeys", query.DatasetModel.SliceKeys },
                         { "VerticalKeys", query.DatasetModel.VerticalKeys }
                    };



                var model = new Dictionary<string, object>(2)
                    {
                        { "model", dataSetModel }, { "codemap", query.GetComponentCodeDescriptionMap() } 
                    };
                try
                {
                    //ser.Serialize(model, json);
                    deserializedObject = new JavaScriptSerializer().Serialize(model);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return deserializedObject.ToString();
        }



    }
    



}

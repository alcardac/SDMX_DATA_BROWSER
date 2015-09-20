using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using ISTAT.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model;
using System.Configuration;
using System.Web.Script.Serialization;
using ISTAT.WebClient.WidgetEngine.Model.DataRender;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using ISTAT.WebClient.WidgetEngine.Model;
using ISTAT.WebClient.WidgetComplements.Model.JSObject.Input;
using ISTAT.WebClient.WidgetComplements.Model.JSObject.Output;

namespace ISTAT.WebClient.Controllers
{         

    public static class JSONConst {
        public readonly static string Error = "{\"error\" : true }";
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

        string SESSION_KEY = "StructureCache";

        public ActionResult ClearSession() {
            // Clear session on change ws
            Session[SESSION_KEY] = null;
            return CS.ReturnForJQuery(JSONConst.Success);
        }
        public ActionResult IsCachingDataSet()
        {
            GetDataObject PostDataArrived = CS.GetPostData<GetDataObject>(this.Request);
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
            
            if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                throw new Exception("ConnectionString not set");
            CacheWidget cache = new CacheWidget(connectionStringSetting.ConnectionString);

            if (PostDataArrived.WidgetId > 0 && UseWidgetCache)
            {
                if (cache.IsCachedWidget(PostDataArrived.WidgetId, System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Trim()))
                    return CS.ReturnForJQuery(JSONConst.Success);
            }

            return CS.ReturnForJQuery(JSONConst.Error);
        }

        public ActionResult GetTree()
        {
            try
            {
                // Get parameter
                GetTreeObject PostDataArrived = CS.GetPostData<GetTreeObject>(this.Request);
                PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                TreeWidget treeWidget = new TreeWidget(PostDataArrived, connectionStringSetting.ConnectionString);
                SessionImplObject ret = treeWidget.GetTree();

                // Clear session on change ws
                Session[SESSION_KEY] = null;

                return CS.ReturnForJQuery(ret.SavedTree);
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

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null 
                    || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                SessionImplObject ret = codemapWidget.GetCodemap(connectionStringSetting);

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

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null
                    || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                CodemapWidget codemapWidget = new CodemapWidget(PostDataArrived, sdmxObj);

                SessionImplObject ret = codemapWidget.GetSpecificCodemap(string.IsNullOrEmpty(PostDataArrived.Codelist) ? true : false, connectionStringSetting);
                
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

                SessionImplObject sdmxObj = null;
                CodemapWidget codemapWidget = new CodemapWidget(PostDataArrived, sdmxObj);
                var count=codemapWidget.GetCountObservation();

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

                // Check if a SessionImplObject is store in session
                SessionImplObject sdmxObj =
                    (Session[SESSION_KEY] != null) ? Session[SESSION_KEY] as SessionImplObject : new SessionImplObject();
                
                LayoutWidget layoutWidget = new LayoutWidget(PostDataArrived, sdmxObj);

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

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

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

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

                SessionImplObject sdmxObj = null;

                /*string SESSION_KEY = "StructureCache";
                var sdmxObj = (Session[SESSION_KEY] != null) ?
                    Session[SESSION_KEY] as Org.Sdmxsource.Sdmx.Api.Model.Objects.ISdmxObjects : null;
                */

                DataWidget dataWidget = new DataWidget(
                    PostDataArrived, sdmxObj,
                    (ConfigurationManager.AppSettings["ParseSDMXAttributes"].ToString().ToLower() == "true"));

                object DataStream = null;
                SessionImplObject ret = dataWidget.GetData(out DataStream);

                DataObjectForStreaming dOFS = DataStream as DataObjectForStreaming;
                if (dOFS != null && PostDataArrived.WidgetId > 0)
                    dOFS.WidgetID = PostDataArrived.WidgetId;

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

                DataWidget.StreamDataTable(
                    Session[CodeForStreaming],
                    textWriter,
                    (ConfigurationManager.AppSettings["ParseSDMXAttributes"].ToString().ToLower() == "true"),
                    cFrom, cTo);

                //Session[CodeForStreaming] = null;

                textWriter.Flush(); // added this line
                byte[] bytesInStream = memoryStream.ToArray(); // simpler way of converting to array
                memoryStream.Close();

                if (UseWidgetCache)
                {
                    ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
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
                    return new StreamResponseAction("HTML result out of range");
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
                    ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                    if (connectionStringSetting == null
                        || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                        throw new Exception("ConnectionString not set");

                    var _ret = codemapWidget.GetCodemap(connectionStringSetting).SdmxObject;
                    sdmxObj.SdmxObject.Merge(_ret);

                    StructureResponceObject _dsd = new StructureResponceObject();
                    var dsd = sdmxObj.SdmxObject.DataStructures.FirstOrDefault();
                    _dsd.Dsd = new DsdObject()
                    {
                        Name = TextTypeHelper.GetText(dsd.Names, PostDataArrived.Configuration.Locale),
                        Desc = TextTypeHelper.GetText(dsd.Descriptions, PostDataArrived.Configuration.Locale),
                        Id = dsd.Id,
                        Agency = dsd.AgencyId,
                        Version = dsd.Version,
                    };
                    _dsd.Concept = new List<ConceptObject>();
                    foreach (var concept in sdmxObj.SdmxObject.ConceptSchemes)
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
                    var dsd = sdmxObj.SdmxObject.DataStructures.FirstOrDefault();
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

                        var conceptScheme = (from c in sdmxObj.SdmxObject.ConceptSchemes
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
                            var conceptScheme = (from c in sdmxObj.SdmxObject.ConceptSchemes
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
                                    AttachmentLevel= attribute.AttachmentLevel.ToString(),
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
                        _dsd.Measure = new List<MeasureObject>();
                        //foreach (var measure in dsd.MeasureList)
                        var measure = dsd.MeasureList.PrimaryMeasure;
                        {
                            var conceptScheme = (from c in sdmxObj.SdmxObject.ConceptSchemes
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
                        
                    }

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
                        (from c in sdmxObj.SdmxObject.ConceptSchemes 
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

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                DOTSTAT_Widget dotstatWidget = new DOTSTAT_Widget(PostDataArrived, sdmxObj);

                SessionImplObject ret = dotstatWidget.Get_DOTSTAT_CodemapAndLayout(connectionStringSetting);

                return CS.ReturnForJQuery(ret.SavedCodemap);
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(JSONConst.Error);
            }
        }

    }
}

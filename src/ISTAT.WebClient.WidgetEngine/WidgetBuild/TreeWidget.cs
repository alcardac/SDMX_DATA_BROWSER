using Estat.Sdmxsource.Extension.Constant;
using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using ISTAT.WebClient.WidgetEngine.Builder.Tree;
using ISTAT.WebClient.WidgetEngine.Model;
using log4net;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Objects;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class TreeWidget
    {
        private GetTreeObject TreeObj { get; set; }
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TreeWidget));
        private const string ErrorOccured = "{\"error\" : true }";

        private const string CategoryIdFormat = "C_{0}";

        private string VirtualDataflowTypeEpAnn = "EP";
        private string VirtualDataflowTypeDescAnn = "DESC";
        private string VirtualDataflowTypeUrlAnn = "URLREF";
        
                                                     
        public string ConnectionString { get; set; }

        public TreeWidget(GetTreeObject treeObj, string connectionString)
        {
            TreeObj = treeObj;
            // Modifico direttamente l'attributo locale usato per identificare la cultura del web service
            //TreeObj.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            ConnectionString = connectionString;
        }

        public SessionImplObject GetTree()
        {
            var ser = new JavaScriptSerializer();
            string json;

            try
            {
                //string decimalCulture=TreeObj.Configuration.Locale;
                //TreeObj.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
                CacheTree ct = new CacheTree(ConnectionString, TreeObj.Configuration);

                string JsonTree = ct.GetCachedTree();
                if (!string.IsNullOrEmpty(JsonTree))
                    return new SessionImplObject() { SavedTree = JsonTree };

                ISdmxObjects SdmxOBJ = GetSdmxObject(TreeObj.Configuration);

                //TreeObj.Configuration.Locale = decimalCulture;

                List<JsTreeNode> nodelist = BuildJSTree(SdmxOBJ, System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
                if (nodelist == null || nodelist.Count == 0)
                {//Invio Errore
                    var x = new { error = true, dataflowError = true, message = ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources.Messages.no_results_found };
                    throw new Exception(ser.Serialize(x));
                }

                json = ser.Serialize(nodelist);
                
                ct.SaveCachedTree(json);

                return new SessionImplObject() { SavedTree = json, SdmxObject = SdmxOBJ };
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

        public string GetTreeforCache(string TwoLetterISO)
        {
            var ser = new JavaScriptSerializer();
            ISdmxObjects SdmxOBJ = GetSdmxObject(TreeObj.Configuration);


            List<JsTreeNode> nodelist = BuildJSTree(SdmxOBJ, TwoLetterISO);
            if (nodelist == null || nodelist.Count == 0)
                return null;

            return ser.Serialize(nodelist);

        }


        #region GetTree Objects from WebService

        private ISdmxObjects GetSdmxObject(EndpointSettings endpointSettings)
        {
            IGetSDMX GetSDMXObject = WebServiceSelector.GetSdmxImplementation(endpointSettings);
            return GetSDMXObject.RetrieveTree();
        }

        #endregion

        #region Costruzione dell'albero JS da un ISdmxObjects
        private List<JsTreeNode> BuildJSTree(ISdmxObjects SdmxOBJ,string TwoLetterISO)
        {
            var categorisedDataflowIndex = new Dictionary<string, IDataflowObject>();
            var uncategorisedDataflow = new List<IDataflowObject>();
            var nodeList = new List<JsTreeNode>();

            // for each dataflows control if has a categorization
            // if true put it in dataflow list or in uncategorizate list
            foreach (IDataflowObject d in SdmxOBJ.Dataflows)
            {
                if (!d.IsExternalReference.IsTrue 
                    && SdmxOBJ.Categorisations.Count(cat => !cat.IsExternalReference.IsTrue && cat.StructureReference.TargetReference.EnumType == d.StructureType.EnumType && MaintainableUtil<IMaintainableObject>.Match(d, cat.StructureReference)) == 0)
                    uncategorisedDataflow.Add(d);
                else
                    categorisedDataflowIndex.Add(Utils.MakeKey(d), d);
            }


            nodeList.AddRange(CreateCategorisedNodes(SdmxOBJ, categorisedDataflowIndex, TwoLetterISO));

            if (TreeObj.Configuration.UseUncategorysed)
            {
                var uncategorisedNode = new JsTreeNode();
                uncategorisedNode.SetRel("category-scheme");
                uncategorisedNode.type = "category-scheme";
                uncategorisedNode.SetId("uncategorised");
                uncategorisedNode.text = Messages.text_dataflows_uncategorized;
                foreach (IDataflowObject dataflow in uncategorisedDataflow)
                {
                    JsTreeNode node = CreateDataflowNode(dataflow, SdmxOBJ, TwoLetterISO);
                    if (node != null) uncategorisedNode.children.Add(node);
                }
                if (uncategorisedNode.children.Count > 0)
                {
                    nodeList.Add(uncategorisedNode);
                }
            }
            return nodeList;
        }


        private IEnumerable<JsTreeNode> CreateCategorisedNodes(
            ISdmxObjects sdmxObject,
            IDictionary<string, IDataflowObject> categorisedDataflowIndex, 
            string TwoLetterISO)
        {
            int categoryCount = 0;
            var categorySchemeNodes = new List<JsTreeNode>();
            var childToParent = new Dictionary<JsTreeNode, JsTreeNode>();
            var leafCategories = new Queue<JsTreeNode>();
            IEnumerable<ICategorySchemeObject> categories = sdmxObject.CategorySchemes;
            foreach (ICategorySchemeObject categoryScheme in categories)
            {
                JsTreeNode categorySchemeNode = CreateCategorySchemeNode(categoryScheme, TwoLetterISO);
                categorySchemeNodes.Add(categorySchemeNode);
                var remainingCategoryNodes = new Stack<JsTreeNode>();
                var remainingCategories = new Stack<ICategoryObject>();

                IList<ICategoryObject> categoriesWithAnnotation = new List<ICategoryObject>();
                IList<ICategoryObject> categoriesWithoutAnnotation = new List<ICategoryObject>();

                foreach (var category in categoryScheme.Items)
                {
                    if (category.Annotations.Count > 0 
                        && category.Annotations[0].FromAnnotation() == CustomAnnotationType.CategorySchemeNodeOrder)
                    {
                        categoriesWithAnnotation.Add(category);
                    }
                    else
                    {
                        categoriesWithoutAnnotation.Add(category);
                    }
                }

                IEnumerable<ICategoryObject> categoriesWithAnnotationOrderedBy = categoriesWithAnnotation.OrderBy(category => Convert.ToInt64(category.Annotations[0].ValueFromAnnotation()));

                IEnumerable<ICategoryObject> categoriesWithAndWithoutAnnotations = categoriesWithoutAnnotation.Concat(categoriesWithAnnotationOrderedBy);

                foreach (ICategoryObject c in categoriesWithAndWithoutAnnotations)
                {
                    JsTreeNode parent = CreateCategoryNode(c, ref categoryCount, TwoLetterISO);

                    categorySchemeNode.children.Add(parent);
                    remainingCategoryNodes.Push(parent);
                    remainingCategories.Push(c);
                    childToParent.Add(parent, categorySchemeNode);
                }

                while (remainingCategoryNodes.Count > 0)
                {
                    JsTreeNode currentNode = remainingCategoryNodes.Pop();
                    ICategoryObject currentCategory = remainingCategories.Pop();

                    IList<ICategoryObject> categoriesParentWithAnnotation = new List<ICategoryObject>();
                    IList<ICategoryObject> categoriesParentWithoutAnnotation = new List<ICategoryObject>();

                    foreach (var category in currentCategory.Items)
                    {
                        if (category.Annotations.Count > 0 
                            && category.Annotations[0].FromAnnotation() == CustomAnnotationType.CategorySchemeNodeOrder)
                        {
                            categoriesParentWithAnnotation.Add(category);
                        }
                        else
                        {
                            categoriesParentWithoutAnnotation.Add(category);
                        }
                    }

                    IEnumerable<ICategoryObject> categoriesParentWithAnnotationOrderedBy = categoriesParentWithAnnotation.OrderBy(category => Convert.ToInt64(category.Annotations[0].ValueFromAnnotation()));

                    IEnumerable<ICategoryObject> categoriesParentWithAndWithoutAnnotations = categoriesParentWithoutAnnotation.Concat(categoriesParentWithAnnotationOrderedBy);

                    foreach (ICategoryObject cc in categoriesParentWithAndWithoutAnnotations)
                    {
                        JsTreeNode childNode = CreateCategoryNode(cc, ref categoryCount, TwoLetterISO);
                        remainingCategoryNodes.Push(childNode);
                        remainingCategories.Push(cc);

                        currentNode.children.Add(childNode);
                        childToParent.Add(childNode, currentNode);
                    }

                    foreach (IMaintainableRefObject dataflowRef in GetDataFlows(currentCategory, sdmxObject.Categorisations, TwoLetterISO))
                    {
                        string key = Utils.MakeKey(dataflowRef);
                        IDataflowObject dataflow;
                        if (categorisedDataflowIndex.TryGetValue(key, out dataflow))
                        {
                            JsTreeNode dataflowNode = CreateDataflowNode(dataflow, sdmxObject, TwoLetterISO);
                            if (dataflowNode != null) currentNode.children.Add(dataflowNode);
                        }
                    }

                    if (currentNode.children.Count == 0)
                    {
                        leafCategories.Enqueue(currentNode);
                    }
                }
            }

            while (leafCategories.Count > 0)
            {
                JsTreeNode current = leafCategories.Dequeue();
                JsTreeNode parent;
                if (childToParent.TryGetValue(current, out parent))
                {
                    parent.children.Remove(current);
                    if (parent.children.Count == 0)
                    {
                        leafCategories.Enqueue(parent);
                    }
                }
                else
                {
                    categorySchemeNodes.Remove(current);
                }
            }

            return categorySchemeNodes;
        }

        private static IEnumerable<IMaintainableRefObject> GetDataFlows(ICategoryObject categoryObject, IEnumerable<ICategorisationObject> categorisations, string TwoLetterISO)
        {
            ISet<IMaintainableRefObject> returnSet = new HashSet<IMaintainableRefObject>();

            /* foreach */
            foreach (ICategorisationObject cat in categorisations)
            {
                if (cat.IsExternalReference.IsTrue)
                {
                    continue;
                }

                if (cat.CategoryReference.TargetReference.EnumType == categoryObject.StructureType.EnumType)
                {
                    var refId = cat.CategoryReference.IdentifiableIds.Last();

                    if (refId.Equals(categoryObject.Id))
                    {
                        returnSet.Add(cat.StructureReference.MaintainableReference);
                    }
                }
            }

            return returnSet;
        }

        private JsTreeNode CreateCategoryNode(
            ICategoryObject category,
            ref int categoryCount, 
            string TwoLetterISO)
        {
            var categoryNode = new JsTreeNode();

            categoryNode.SetId(
                string.Format(
                    CultureInfo.InvariantCulture,
                    CategoryIdFormat,
                    categoryCount.ToString("x", CultureInfo.InvariantCulture)));

            categoryCount++;
            SetupNode(categoryNode, category, TwoLetterISO);
            categoryNode.SetRel("category");
            categoryNode.type="category";

            //if (TreeObj.Configuration.UseVirtualDf 
            //    && category.HasAnnotationType(VirtualDataflowTypeAnn))
            //{
            //    var vrtDf = category.GetAnnotationsByType(VirtualDataflowTypeAnn);
            //    var value = TextTypeHelper.GetText(vrtDf.FirstOrDefault().Text,TwoLetterISO);

            //    string end_key = "@";
            //    string search_key = string.Empty;

            //    search_key = "@TITOLO=";
            //    string title = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    title = title.Substring(0,title.IndexOf(end_key));
                
            //    search_key = "@EP1=";
            //    string endpoint_1 = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    endpoint_1 = endpoint_1.Substring(0, endpoint_1.IndexOf(end_key));
                
            //    search_key = "@EP2=";
            //    string endpoint_2 = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    endpoint_2 = endpoint_2.Substring(0, endpoint_2.IndexOf(end_key));

            //    search_key = "@EPT=";
            //    string endpoint_type = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    endpoint_type = endpoint_type.Substring(0, endpoint_type.IndexOf(end_key));

            //    search_key = "@Id=";
            //    string Id = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    Id = Id.Substring(0, Id.IndexOf(end_key));
                
            //    search_key = "@Agency=";
            //    string Agency = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    Agency = Agency.Substring(0, Agency.IndexOf(end_key));
                
            //    search_key = "@Version=";
            //    string Version = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    Version = Version.Substring(0, Version.IndexOf(end_key));

            //    search_key = "@Source=";
            //    string Source = value.Substring(value.IndexOf(search_key) + search_key.Length);
            //    Source = Source.Substring(0, Source.IndexOf(end_key));
                
            //    categoryNode.children.Add(
            //        CreateDataflowNode(Id,
            //        Version,
            //        Agency,
            //        title,
            //        endpoint_2,
            //        endpoint_1,
            //        endpoint_type,
            //        Source,
            //        TreeObj.Configuration.Locale));

            //}

            return categoryNode;
        }

        /// <summary>
        /// Create a CategoryScheme Node
        /// </summary>
        /// <param name="categoryScheme">
        /// The SDMX Model category scheme object
        /// </param>
        /// <returns>
        /// The CategoryScheme Node
        /// </returns>
        private JsTreeNode CreateCategorySchemeNode(ICategorySchemeObject categoryScheme, string TwoLetterISO)
        {
            var categorySchemeNode = new JsTreeNode();

            // categorySchemeNode.data.attributes["rel"] = MakeKey(categoryScheme);
            categorySchemeNode.SetId(Utils.MakeKey(categoryScheme).Replace('.', '_'));

            // categorySchemeNode.data.icon = "folder";
            categorySchemeNode.SetRel("category-scheme");
            categorySchemeNode.type = "category-scheme";
            categorySchemeNode.AddClass("category-scheme");
            SetupNode(categorySchemeNode, categoryScheme, TwoLetterISO);
            return categorySchemeNode;
        }

        /// <summary>
        /// Create a Dataflow Node
        /// </summary>
        /// <param name="dataflow">
        /// The SDMX Model Dataflow object
        /// </param>
        /// <returns>
        /// The Dataflow Node
        /// </returns>
        private JsTreeNode CreateDataflowNode(
            IDataflowObject dataflow,
            ISdmxObjects SdmxOBJ, 
            string TwoLetterISO)
        {

            var dataflowNode = new JsTreeNode();

            if (dataflow.HasAnnotationType(VirtualDataflowTypeEpAnn))
            {

                // VIrtual DF
                var vrtDf = dataflow.GetAnnotationsByType(VirtualDataflowTypeEpAnn);
                var value = TextTypeHelper.GetText(vrtDf.FirstOrDefault().Text, TwoLetterISO);

                string end_key = "@";
                string search_key = string.Empty;

                search_key = "@EP1=";
                string endpoint_1 = value.Substring(value.IndexOf(search_key) + search_key.Length);
                endpoint_1 = endpoint_1.Substring(0, endpoint_1.IndexOf(end_key));

                search_key = "@EP2=";
                string endpoint_2 = value.Substring(value.IndexOf(search_key) + search_key.Length);
                endpoint_2 = endpoint_2.Substring(0, endpoint_2.IndexOf(end_key));

                search_key = "@EPT=";
                string endpoint_type = value.Substring(value.IndexOf(search_key) + search_key.Length);
                endpoint_type = endpoint_type.Substring(0, endpoint_type.IndexOf(end_key));

                search_key = "@SOURCE=";
                string dataflow_source = value.Substring(value.IndexOf(search_key) + search_key.Length);
                dataflow_source = dataflow_source.Substring(0, dataflow_source.IndexOf(end_key));
                
                List<string> valueDesc=new List<string>();
                if(dataflow.HasAnnotationType(VirtualDataflowTypeDescAnn)) {
                    var vrtDfDesc = dataflow.GetAnnotationsByType(VirtualDataflowTypeDescAnn);

                    foreach (var ann in vrtDfDesc)
                    {
                        valueDesc.Add(TextTypeHelper.GetText(ann.Text, TwoLetterISO));
                    }
                }

                List<DataflowMetaUrl> valueUrls = new List<DataflowMetaUrl>();
                if (dataflow.HasAnnotationType(VirtualDataflowTypeUrlAnn))
                {
                    var vrtDfUrls = dataflow.GetAnnotationsByType(VirtualDataflowTypeUrlAnn);

                    foreach (var ann in vrtDfUrls){
                        valueUrls.Add(new DataflowMetaUrl()
                        {
                            Title = TextTypeHelper.GetText(ann.Text, TwoLetterISO),
                            URL = ann.Title
                        });
                    }
                }

                dataflowNode=CreateDataflowNode(
                    dataflow.Id,
                    dataflow.Version,
                    dataflow.AgencyId,
                    TextTypeHelper.GetText(dataflow.Names, TwoLetterISO),
                    endpoint_1,
                    endpoint_2,
                    endpoint_type,
                    dataflow_source,
                    TreeObj.Configuration.DecimalSeparator,
                    valueDesc,
                    valueUrls);
            }
            else
            {
                // Normal DF
                dataflowNode.SetId(Utils.MakeKey(dataflow).Replace('.', '_').Replace('+', '-'));
                SetupNode(dataflowNode, dataflow, TwoLetterISO);

                IDataStructureObject dsd =
                    SdmxOBJ.DataStructures.FirstOrDefault(
                    dataStructure => dataflow.DataStructureRef.Equals(dataStructure.AsReference));

                if (dsd != null && dsd is ICrossSectionalDataStructureObject)
                {
                    dataflowNode.SetRel("xs-dataflow");
                    dataflowNode.type = "xs-dataflow";
                }
                else
                {
                    dataflowNode.SetRel("dataflow");
                    dataflowNode.type = "dataflow";
                }

                dataflowNode.li_attr.Add("title", TreeObj.Configuration.Title);

                dataflowNode.a_attr = new JSTreeMetadata
                {
                    DataflowID = dataflow.Id,
                    DataflowVersion = dataflow.Version,
                    DataflowAgency = dataflow.AgencyId,
                    DataflowUrl = TreeObj.Configuration.EndPoint,
                    DataflowUrlV20 = TreeObj.Configuration.EndPointV20,
                    DataflowUrlType = TreeObj.Configuration.EndPointType,
                    DataflowSource = TreeObj.Configuration.EndPointSource,
                    DataflowDecimalCulture = TreeObj.Configuration.DecimalSeparator,
                };
            }
            return dataflowNode;
        }

        private JsTreeNode CreateDataflowNode(
            string dataflow_id,
            string dataflow_version,
            string dataflow_agency,
            string dataflow_title,
            string dataflow_url_v20,
            string dataflow_url_v21,
            string dataflow_type,
            string dataflow_source,
            string dataflow_decimalCulture,
            List<string> dataflow_desc,
            List<DataflowMetaUrl> dataflow_url)
        {
            var dataflowNode = new JsTreeNode();

            dataflowNode.SetId(dataflow_id);
            dataflowNode.text = dataflow_title;
            dataflowNode.SetRel("virtual-dataflow");
            dataflowNode.type = "virtual-dataflow";
            dataflowNode.li_attr.Add("title", dataflow_title);
            dataflowNode.a_attr = new JSTreeMetadata
            {
                DataflowID = dataflow_id,
                DataflowVersion = dataflow_version,
                DataflowAgency = dataflow_agency,
                DataflowUrl = dataflow_url_v21,
                DataflowUrlV20 = dataflow_url_v20,
                DataflowUrlType = dataflow_type,
                DataflowSource = dataflow_source,
                DataflowDecimalCulture = dataflow_decimalCulture,
                DataflowName=dataflow_title,
                DataflowDesc = new JavaScriptSerializer().Serialize(dataflow_desc),
                DataflowUrls = new JavaScriptSerializer().Serialize(dataflow_url)
            };

            return dataflowNode;
        }

        private void SetupNode(JsTreeNode node, INameableObject artefact)
        {
            string entitle = artefact.Id;
            SetupNode(node, artefact, entitle, "{0}");
        }
        private void SetupNode(JsTreeNode node, INameableObject artefact, string TwoLetterISOLanguageName)
        {
            string entitle = artefact.Id;
            SetupNode(node, artefact, entitle, "{0}", TwoLetterISOLanguageName);
        }
        private void SetupNode(JsTreeNode node, INameableObject artefact, string defaultString, string format)
        {
            string result = TextTypeHelper.GetText(artefact.Names, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
            string title = string.Format(Thread.CurrentThread.CurrentUICulture, format, result.Length == 0 ? TextTypeHelper.GetText(artefact.Descriptions, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName) : result);

            if (string.IsNullOrEmpty(title)) title = defaultString;
            node.text = title;
        }
        private void SetupNode(JsTreeNode node, INameableObject artefact, string defaultString, string format,string TwoLetterISOLanguageName)
        {
            string result = TextTypeHelper.GetText(artefact.Names, TwoLetterISOLanguageName);
            string title = string.Format(format, result.Length == 0 ? TextTypeHelper.GetText(artefact.Descriptions, TwoLetterISOLanguageName) : result);

            if (string.IsNullOrEmpty(title)) title = defaultString;
            node.text = title;
        }

        #endregion


    }
}

// -----------------------------------------------------------------------
// <copyright file="DataflowTreeBuilder.cs" company="EUROSTAT">
//   Date Created : 2011-09-28
//   Copyright (c) 2009, 2015 by the European Commission, represented by Eurostat.   All rights reserved.
// 
// Licensed under the EUPL, Version 1.1 or – as soon they
// will be approved by the European Commission - subsequent
// versions of the EUPL (the "Licence");
// You may not use this work except in compliance with the
// Licence.
// You may obtain a copy of the Licence at:
// 
// https://joinup.ec.europa.eu/software/page/eupl 
// 
// Unless required by applicable law or agreed to in
// writing, software distributed under the Licence is
// distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the Licence for the specific language governing
// permissions and limitations under the Licence.
// </copyright>
// -----------------------------------------------------------------------
namespace ISTAT.WebClient.Tree
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Estat.Sdmxsource.Extension.Constant;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Util.Objects;

    using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
    using Estat.Nsi.Client;

    /// <summary>
    /// This class builds a Dataflow Tree with categorized and uncategorized categories
    /// </summary>
    public class DataflowTreeBuilder : JSTreeBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// Category HTML Id format
        /// </summary>
        private const string CategoryIdFormat = "C_{0}";

        /// <summary>
        /// The list of categories
        /// </summary>
        private readonly IEnumerable<ICategorySchemeObject> _categories;

        /// <summary>
        /// The list of dataflows
        /// </summary>
        private readonly ISet<IDataflowObject> _dataflows;

        /// <summary>
        /// The list of categorisations
        /// </summary>
        private readonly IEnumerable<ICategorisationObject> _categorisations;

        /// <summary>
        /// The list of dsds
        /// </summary>
        private readonly ISet<IDataStructureObject> _dataStructure;

        /// <summary>
        /// The previous culture
        /// </summary>
        private CultureInfo _prevCulture;

        /// <summary>
        /// The list of tree nodes
        /// </summary>
        private List<JsTreeNode> _tree;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataflowTreeBuilder"/> class. 
        /// Initialize a new instance of the DataflowTreeBuilder
        /// </summary>
        /// <param name="categories">
        /// The list of categories
        /// </param>
        /// <param name="dataflows">
        /// The list of dataflows
        /// </param>
        public DataflowTreeBuilder(IEnumerable<ICategorySchemeObject> categories, ISet<IDataflowObject> dataflows, IEnumerable<ICategorisationObject> categorisations, ISet<IDataStructureObject> dataStructure)
        {
            this._dataflows = dataflows;
            this._categories = categories;
            this._categorisations = categorisations;
            this._dataStructure = dataStructure;
            this._tree = this.BuildJSTree();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the list of tree nodes
        /// </summary>
        /// <returns>
        /// The list of tree nodes
        /// </returns>
        public List<JsTreeNode> GetJSTree()
        {
            if (!this._prevCulture.Equals(Thread.CurrentThread.CurrentUICulture))
            {
                this._tree = this.BuildJSTree();
            }

            return this._tree;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the categorised nodes, including the Categories and Dataflows
        /// </summary>
        /// <param name="categories">
        /// The list of CategorySchemes
        /// </param>
        /// <param name="categorisedDataflowIndex">
        /// A dictionary with a string with the format "Agency+Id+Version", <see cref="Utils.MakeKey(RefBean)"/> to DataflowBean map
        /// </param>
        /// <returns>
        /// The create categorised nodes.
        /// The tree with the categorized nodes
        /// </returns>
        private IEnumerable<JsTreeNode> CreateCategorisedNodes(
            IEnumerable<ICategorySchemeObject> categories, IDictionary<string, IDataflowObject> categorisedDataflowIndex)
        {
            int categoryCount = 0;
            var categorySchemeNodes = new List<JsTreeNode>();
            var childToParent = new Dictionary<JsTreeNode, JsTreeNode>();
            var leafCategories = new Queue<JsTreeNode>();
            foreach (ICategorySchemeObject categoryScheme in categories)
            {
                JsTreeNode categorySchemeNode = CreateCategorySchemeNode(categoryScheme);
                categorySchemeNodes.Add(categorySchemeNode);
                var remainingCategoryNodes = new Stack<JsTreeNode>();
                var remainingCategories = new Stack<ICategoryObject>();

                IList<ICategoryObject> categoriesWithAnnotation = new List<ICategoryObject>();
                IList<ICategoryObject> categoriesWithoutAnnotation = new List<ICategoryObject>();

                /*foreach (var category in categoryScheme.Items)
                {
                    if (category.Annotations.Count > 0 && category.Annotations[0].FromAnnotation() == CustomAnnotationType.CategorySchemeNodeOrder)
                    {
                        categoriesWithAnnotation.Add(category);
                    }
                    else
                    {
                        categoriesWithoutAnnotation.Add(category);
                    }
                }*/

                IEnumerable<ICategoryObject> categoriesWithAnnotationOrderedBy = categoriesWithAnnotation.OrderBy(category => Convert.ToInt64(category.Annotations[0].ValueFromAnnotation()));

                IEnumerable<ICategoryObject> categoriesWithAndWithoutAnnotations = categoriesWithoutAnnotation.Concat(categoriesWithAnnotationOrderedBy);

                foreach (ICategoryObject c in categoriesWithAndWithoutAnnotations)
                {
                    JsTreeNode parent = CreateCategoryNode(c, ref categoryCount);

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
                        if (category.Annotations.Count > 0 && category.Annotations[0].FromAnnotation() == CustomAnnotationType.CategorySchemeNodeOrder)
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
                        JsTreeNode childNode = CreateCategoryNode(cc, ref categoryCount);
                        remainingCategoryNodes.Push(childNode);
                        remainingCategories.Push(cc);

                        currentNode.children.Add(childNode);
                        childToParent.Add(childNode, currentNode);
                    }

                    foreach (IMaintainableRefObject dataflowRef in GetDataFlows(currentCategory, _categorisations))
                    {
                        string key = "";// Utils.MakeKey(dataflowRef);
                        IDataflowObject dataflow;
                        if (categorisedDataflowIndex.TryGetValue(key, out dataflow))
                        {
                            JsTreeNode dataflowNode = CreateDataflowNode(dataflow);
                            currentNode.children.Add(dataflowNode);
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

        /// <summary>
        /// Create a Category Node
        /// </summary>
        /// <param name="category">
        /// The SDMX Model category  object
        /// </param>
        /// <param name="categoryCount">
        /// The caregory counter. This methods updates it
        /// </param>
        /// <returns>
        /// The Category Node
        /// </returns>
        private static JsTreeNode CreateCategoryNode(ICategoryObject category, ref int categoryCount)
        {
            var categoryNode = new JsTreeNode();

            // categoryNode.data.attributes["rel"] = category.Id;
            // categoryNode.SetId(category.Id);
            categoryNode.SetId(
                string.Format(
                    CultureInfo.InvariantCulture,
                    CategoryIdFormat,
                    categoryCount.ToString("x", CultureInfo.InvariantCulture)));
            categoryCount++;
            SetupNode(categoryNode, category);
            categoryNode.SetRel("category");
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
        private static JsTreeNode CreateCategorySchemeNode(ICategorySchemeObject categoryScheme)
        {
            var categorySchemeNode = new JsTreeNode();

            // categorySchemeNode.data.attributes["rel"] = MakeKey(categoryScheme);
            categorySchemeNode.SetId(Utils.MakeKey(categoryScheme).Replace('.', '_'));

            // categorySchemeNode.data.icon = "folder";
            categorySchemeNode.SetRel("category-scheme");
            SetupNode(categorySchemeNode, categoryScheme);
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
        private JsTreeNode CreateDataflowNode(IDataflowObject dataflow)
        {
            var dataflowNode = new JsTreeNode();

            // dataflowNode.data.attributes.rel = MakeKey(dataflow);
            dataflowNode.SetId(Utils.MakeKey(dataflow).Replace('.', '_').Replace('+', '-'));
            SetupNode(dataflowNode, dataflow);
            IDataStructureObject dsd = NsiClientHelper.GetDsdFromDataflow(dataflow, _dataStructure);
            if (dsd != null && NsiClientHelper.DataflowDsdIsCrossSectional(dsd))
            {
                dataflowNode.SetRel("xs-dataflow");
            }
            else
            {
                dataflowNode.SetRel("dataflow");
            }
            dataflowNode.AddClass("dataflow-item");
            dataflowNode.SetLeaf(true);

            // dataflowNode.state = "closed";
            dataflowNode.metadata = new JSTreeMetadata
            {
                DataflowID = dataflow.Id,
                DataflowVersion = dataflow.Version,
                DataflowAgency = dataflow.AgencyId
            };

            // dataflowNode.metadata.dataflow_name = dataflow.PrimaryName;
            return dataflowNode;
        }

        /// <summary>
        /// Build a JQuery plugin JSTree JSON_DATA json string. It uses the <see cref="_dataflows"/> and <see cref="_categories"/>
        /// </summary>
        /// <returns>
        /// A list of nodes
        /// </returns>
        private List<JsTreeNode> BuildJSTree()
        {
            this._prevCulture = Thread.CurrentThread.CurrentUICulture;
            var categorisedDataflowIndex = new Dictionary<string, IDataflowObject>();
            var uncategorisedDataflow = new List<IDataflowObject>();
            var nodeList = new List<JsTreeNode>();

            foreach (IDataflowObject d in this._dataflows)
            {

                if (GetCategorisations(d, _categorisations).Count == 0)
                {
                    uncategorisedDataflow.Add(d);
                }
                else
                {
                    string key = Utils.MakeKey(d);
                    categorisedDataflowIndex.Add(key, d);
                }
            }


            nodeList.AddRange(CreateCategorisedNodes(this._categories, categorisedDataflowIndex));
            var uncategorisedNode = new JsTreeNode();

            //////var data = new Data();
            uncategorisedNode.SetRel("category-scheme");
            uncategorisedNode.SetId("uncategorised");

            ////data.title = Messages.text_dataflows_uncategorized;
            ////uncategorisedNode.data.Add(data);
            uncategorisedNode.data = Messages.text_dataflows_uncategorized;
            foreach (IDataflowObject dataflow in uncategorisedDataflow)
            {
                uncategorisedNode.children.Add(CreateDataflowNode(dataflow));
            }

            if (uncategorisedNode.children.Count > 0)
            {
                nodeList.Add(uncategorisedNode);
            }

            return nodeList;
        }


        private static ISet<ICategorisationObject> GetCategorisations(
          IMaintainableObject maintainable, IEnumerable<ICategorisationObject> categorisations)
        {
            ISet<ICategorisationObject> returnSet = new HashSet<ICategorisationObject>();
            if (maintainable.IsExternalReference.IsTrue)
            {
                return returnSet;
            }

            /* foreach */
            foreach (ICategorisationObject cat in categorisations)
            {
                if (cat.IsExternalReference.IsTrue)
                {
                    continue;
                }

                if (cat.StructureReference.TargetReference.EnumType == maintainable.StructureType.EnumType)
                {
                    if (MaintainableUtil<IMaintainableObject>.Match(maintainable, cat.StructureReference))
                    {
                        returnSet.Add(cat);
                    }
                }
            }

            return returnSet;
        }

        private static IEnumerable<IMaintainableRefObject> GetDataFlows(
     ICategoryObject categoryObject, IEnumerable<ICategorisationObject> categorisations)
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

        #endregion
    }
}
// -----------------------------------------------------------------------
// <copyright file="SessionQuery.cs" company="EUROSTAT">
//   Date Created : 2010-11-11
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
//namespace ISTAT.WebClient.Controllers
namespace ISTAT.WebClient.WidgetComplements.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;

    //using Estat.Nsi.Client.Properties;
    //using Estat.Nsi.Client.Renderer;
    //using ISTAT.WebClient.WidgetEngine.Model.DataRender;
    //using Estat.Nsi.Client.Web.Properties;
    //using Estat.Nsi.Client.Web.Tree;
    using ISTAT.WebClient.WidgetComplements.Model.Tree;
    using ISTAT.WebClient.WidgetComplements.Model.NSIWC;
    using ISTAT.WebClient.WidgetComplements.Model.DataRenderNSI;
    //using ISTAT.WebClient.WidgetEngine.Model.DataReader;
    using ISTAT.WebClient.WidgetComplements.Model.DataReaderNSI;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    using log4net;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Util.Objects.Container;
    //using ISTAT.WebClient.WidgetEngine.Model.DataReader;
    using ISTAT.WebClient.WidgetComplements.Model.Properties;
    using ISTAT.WebClient.WidgetComplements.Model.DataRender;


    /// <summary>
    /// The session query
    /// </summary>
    /// <remarks>
    /// Properties and only properties will be serialized to json.
    /// </remarks>
    public class SessionQuery
    {
        #region Constants and Fields

        /// <summary>
        /// Contains the CodeTree for each coded component
        /// </summary>
        public readonly Dictionary<IComponent, CodeTreeBuilder> _codeTreeMap =
            new Dictionary<IComponent, CodeTreeBuilder>();

        /// <summary>
        /// Contains the artefact cache
        /// </summary>
        public readonly ComponentArtefactCache< ICodelistObject,ICode> _codelistCache =
            new ComponentArtefactCache<ICodelistObject, ICode> ();

        /// <summary>
        /// Map between component id and a map between code value and code description.
        /// </summary>
        public readonly ComponentCodeDescriptionDictionary _componentCodesDescriptionMap =
            new ComponentCodeDescriptionDictionary();

        /// <summary>
        /// A map between the component and the index inside the DSD. The Time Dimension goes after the last dimension
        /// </summary>
        public readonly Dictionary<IComponent, int> _componentIndex = new Dictionary<IComponent, int>();

        /// <summary>
        /// A map between the a DSD component and Concept {id} to the concept.
        /// </summary>
        public readonly ComponentArtefactCache<IConceptSchemeObject, IConceptObject> _conceptMap = new ComponentArtefactCache<IConceptSchemeObject,IConceptObject>();

        /// <summary>
        /// List of data related disposable items
        /// They will be Disposed when <see cref="Reset"/> or <see cref="Clear"/> are called
        /// </summary>
        public readonly List<IDisposable> _dataDisposeList = new List<IDisposable>();

        /// <summary>
        /// A map between a unique string identifacation of a dataflow against a DataflowBean
        /// </summary>
        public readonly Dictionary<string, IDataflowObject> _dataflowMap = new Dictionary<string, IDataflowObject>();

        /// <summary>
        /// The list in the same order as the dimensions in the DSD plus the Time Dimension at the end containing the Query Component for each component.
        /// </summary>
        public readonly List<QueryComponent> _queryComponentIndex = new List<QueryComponent>();

        /// <summary>
        /// Session cache folder.
        /// </summary>
        public DirectoryInfo _cacheFolder;

        /// <summary>
        /// The current selected criteria component tab
        /// </summary>
        public int _criteriaComponentIdx;

        /// <summary>
        /// The current culture
        /// </summary>
        public CultureInfo _currentCulture;

        /// <summary>
        /// The layout
        /// </summary>
        public IDataSetModel _dataSetModel;

        /// <summary>
        /// Store the SDMX-ML dataset stream from NSI CLient executeQuery
        /// </summary>
        public string _dataSetSDMXML;

        /// <summary>
        /// The selected dataflow, <code>null</code> if no selection has been made yet.
        /// </summary>
        public IDataflowObject _dataflow;

        /// <summary>
        /// List of tree nodes containing the categories and dataflows
        /// </summary>
        public DataflowTreeBuilder _dataflowTreeList;

        /// <summary>
        /// The keyfamily of the selected dataflow, if structure is not null.
        /// </summary>
        public IDataStructureObject _keyFamily;

        /// <summary>
        /// The maximum number of observations
        /// </summary>
        public int _maximumObservations;

        /// <summary>
        /// The session prefix.
        /// </summary>
        public string _sessionPrefix = Guid.NewGuid().ToString();

        /// <summary>
        /// The _store.
        /// </summary>
        public IDataSetStore _store;

        /// <summary>
        ///  The bean with key family's data and concepts. It should not include any data about code lists.
        /// </summary>
        public ISdmxObjects _structure;

        /// <summary>
        /// The currently selected work page
        /// </summary>
        public int _workPageIdx;

        /// <summary>
        /// The endpoint type
        /// </summary>
        public EndpointType _endpointType;

        public static readonly ILog Logger = LogManager.GetLogger(typeof(SessionQuery));

        public ISet<IDataStructureObject> _dsds = new HashSet<IDataStructureObject>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current component tab
        /// </summary>
        public int CriteriaComponentIdx
        {
            get
            {
                return this._criteriaComponentIdx;
            }
        }

        /// <summary>
        /// Gets or sets the current culture
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get
            {
                return this._currentCulture;
            }

            set
            {
                this._currentCulture = value;
                if (this._currentCulture != null)
                {
                    Thread.CurrentThread.CurrentUICulture = this._currentCulture;
                    this.RebuildCodeDescriptionMap();
                    if (this._dataSetModel != null)
                    {
                        this._dataSetModel.ReloadValidKeys();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the current max number of observations.
        /// </summary>
        public int CurrentMax { get; set; }

        /// <summary>
        /// Gets or sets the current dataflow
        /// </summary>
        public IDataflowObject Dataflow
        {
            get
            {
                return this._dataflow;
            }

            set
            {
                this._dataflow = value;
            }
        }

        /// <summary>
        /// Gets or sets the DataSet Model
        /// </summary>
        public IDataSetModel DatasetModel
        {
            get
            {
                return this._dataSetModel;
            }

            set
            {
                this._dataSetModel = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataflow is set
        /// </summary>
        public bool IsDataflowSet
        {
            get
            {
                return this._dataflow != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the criteria is empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                bool notEmpty = false;
                for (int i = 0, j = this._queryComponentIndex.Count; i < j && !notEmpty; i++)
                {
                    notEmpty = this._queryComponentIndex[i] != null;
                }

                return !notEmpty;
            }
        }

        /// <summary>
        /// Gets the keyFamily
        /// </summary>
        public IDataStructureObject KeyFamily
        {
            get
            {
                return this._keyFamily;
            }
        }

        /// <summary>
        /// Gets the EndPointType
        /// </summary>
        public EndpointType EndPointType
        {
            get
            {
                return this._endpointType;
            }
            set
            {
                this._endpointType = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of observations
        /// </summary>
        public int MaximumObservations
        {
            get
            {
                return this._maximumObservations;
            }

            set
            {
                this._maximumObservations = value;
                this.CurrentMax = this._maximumObservations;
            }
        }

        /// <summary>
        /// Gets or sets the Structure containing the KeyFamily used by the current <see cref="Dataflow"/> and the concepts used by this KeyFamily
        /// </summary>
        public ISdmxObjects Structure
        {
            get
            {
                return this._structure;
            }

            set
            {
                this._structure = value;
                if (this._structure.DataStructures.Count > 0)
                {
                    this._keyFamily = this._structure.DataStructures.First();
                    this.InitializeConceptMap(this._structure);
                    this.InitializeComponentList(this.KeyFamily);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Dsds
        /// </summary>
        public ISet<IDataStructureObject>  Dsds
        {
            
            get
            {
                return _dsds;
            }
            set
            {
                _dsds = value;
            }
        }


        /// <summary>
        /// Gets the current main tab
        /// </summary>
        public int WorkPageIdx
        {
            get
            {
                return this._workPageIdx;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add disposable item to the list of data related disposable items.
        /// They will be Disposed when <see cref="Reset"/> or <see cref="Clear"/> or <see cref="ClearData"/>are called
        /// </summary>
        /// <param name="item">
        /// The <see cref="IDisposable"/> item
        /// </param>
        public void AddToDataDisposeList(IDisposable item)
        {
            this._dataDisposeList.Add(item);
        }

        /// <summary>
        /// Clear the existing criteria and data
        /// </summary>
        public void Clear()
        {
            this._codelistCache.Clear();
            this._codeTreeMap.Clear();
            this._componentCodesDescriptionMap.Clear();
            this.RemoveAllComponent();
            this.ClearData();
        }

        /// <summary>
        /// Clear the artefact cache for components after the specified component
        /// </summary>
        /// <param name="component">
        /// The component
        /// </param>
        public void ClearCodeListAfter(IComponent component)
        {
            int pos = this._componentIndex[component];
            for (int i = pos + 1, j = this._keyFamily.DimensionList.Dimensions.Count; i < j; i++)
            {
                IComponent dim = this._keyFamily.DimensionList.Dimensions[i];
                if (dim.HasCodedRepresentation() && dim.Representation.Representation.MaintainableReference.MaintainableId != null)
                {
                    this._codelistCache.Remove(dim);
                    this._codeTreeMap.Remove(dim);
                }
            }

            if (this._keyFamily.TimeDimension != null)
            {
                this._codelistCache.Remove(this._keyFamily.TimeDimension);
            }
        }

        /// <summary>
        /// Clear data
        /// </summary>
        public void ClearData()
        {
            this.CloseSdmxMLDataSet();
            this._dataSetModel = null;
            if (this._store != null)
            {
                this._store.Dispose();
                this._store = null;
            }

            for (int i = 0, j = this._dataDisposeList.Count; i < j; i++)
            {
                this._dataDisposeList[i].Dispose();
            }

            this._dataDisposeList.Clear();
            
            try
            {
                // close any files
                /*DA FARE FABIO NUOVO
                _cacheFolder = new DirectoryInfo(Utils.GetAppPath()); 
                FileInfo[] files = this._cacheFolder.GetFiles(this._sessionPrefix + "*.*");
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }*/
            }
            catch (IOException e)
            {
                Logger.Warn(e.Message, e);
            }
            catch (SecurityException e)
            {
                Logger.Warn(e.Message, e);
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Warn(e.Message, e);
            }
        }

        /// <summary>
        /// Close the dataset freeing memory
        /// </summary>
        public void CloseSdmxMLDataSet()
        {
            if (this._dataSetSDMXML == null)
            {
                return;
            }
            //DA FARE FABIO
            //NsiClientHelper.TryToDelete(this._dataSetSDMXML);
            this._dataSetSDMXML = null;
        }

        /// <summary>
        /// Get the <see cref="DirectoryInfo"/> instance of the cache folder
        /// </summary>
        /// <returns>
        /// The <see cref="DirectoryInfo"/> instance of the cache folder
        /// </returns>
        public DirectoryInfo GetCacheFolder()
        {
            return this._cacheFolder;
        }

        /// <summary>
        /// Get the code list for the specified component
        /// </summary>
        /// <param name="component">
        /// The coded component
        /// </param>
        /// <returns>
        /// The artefact or null if it is not in the <see cref="_codelistCache"/>
        /// </returns>
        public ICodelistObject GetCachedCodelist(IComponent component)
        {
            ICodelistObject codelist = this._codelistCache.GetArtefact(component);
            return codelist;
        }

        /// <summary>
        /// Get a concept from the <see cref="_conceptMap"/> from the specified component
        /// </summary>
        /// <param name="component">
        /// The component
        /// </param>
        /// <returns>
        /// The concept of the specified component or null
        /// </returns>
        public IConceptObject GetCachedConcept(IComponent component)
        {
            return this._conceptMap.GetArtefactItemObject(component);
        }

        /// <summary>
        /// Get the dataflow from <see cref="_dataflowMap"/> based on the specified identification
        /// </summary>
        /// <param name="id">
        /// The dataflow id
        /// </param>
        /// <param name="agency">
        /// The dataflow agency
        /// </param>
        /// <param name="version">
        /// The dataflow version
        /// </param>
        /// <returns>
        /// The cached dataflow; otherwise null
        /// </returns>
        public IDataflowObject GetCachedDataflow(string id, string agency, string version)
        {
            string key = Utils.MakeKey(id, agency, version);
            IDataflowObject dataflow;
            return this._dataflowMap.TryGetValue(key, out dataflow) ? dataflow : null;
        }

        /// <summary>
        /// Get the specified <paramref name="component"/> code tree. 
        /// </summary>
        /// <param name="component">
        /// The SDMX <see cref="ComponentBean"/>
        /// </param>
        /// <returns>
        /// The <see cref="CodeTreeBuilder"/> of the specified <paramref name="component"/>
        /// </returns>
        public CodeTreeBuilder GetCodeTree(IComponent component)
        {
            CodeTreeBuilder codeTree;
            return this._codeTreeMap.TryGetValue(component, out codeTree) ? codeTree : null;
        }

        /// <summary>
        /// Get the Component -&lt; Code Value -&lt; Code Description map
        /// </summary>
        /// <returns>
        /// The map between components codes and description
        /// </returns>
        public ComponentCodeDescriptionDictionary GetComponentCodeDescriptionMap()
        {
            return this._componentCodesDescriptionMap;
        }

        ///////// <summary>
        ///////// Get the index of the specified dimension or time dimension
        ///////// </summary>
        ///////// <param name="component">
        ///////// The dimension or time dimension
        ///////// </param>
        ///////// <returns>
        ///////// The index of the dimension or time dimension or -1 if it is not found
        ///////// </returns>
        //////public int GetComponentIndex(ComponentBean component)
        //////{
        //////    int idx;
        //////    if (!this._componentIndex.TryGetValue(component, out idx))
        //////    {
        //////        idx = -1;
        //////    }

        //////    return idx;
        //////}

        /// <summary>
        /// Gets the current component is retrieved from <see cref="CriteriaComponentIdx"/>
        /// </summary>
        /// <returns>
        /// The current component is retrieved from <see cref="CriteriaComponentIdx"/>
        /// </returns>
        public CodeTreeBuilder GetCurrentCodeTree()
        {
            CodeTreeBuilder codeTree = null;
            if (this._criteriaComponentIdx > -1)
            {
                foreach (KeyValuePair<IComponent, int> kv in this._componentIndex)
                {
                    if (kv.Value == this._criteriaComponentIdx)
                    {
                        codeTree = this.GetCodeTree(kv.Key);
                        break;
                    }
                }
            }

            return codeTree;
        }

        /// <summary>
        /// Get cached dataflow tree
        /// </summary>
        /// <returns>
        /// The list of <see cref="JsTreeNode"/> from cache
        /// </returns>
        public List<JsTreeNode> GetDataflowTree()
        {
            return this._dataflowTreeList.GetJSTree();
        }

        /// <summary>
        /// Gets the full structure with codelists
        /// </summary>
        /// <returns>
        /// The full structure with Codelists
        /// </returns>
        public ISdmxObjects GetFullStructure()
        {
            if (this._structure == null)
            {
                return null;
            }

            var fullStructure = new SdmxObjectsImpl();
            foreach (var structure in this._structure.DataStructures)
            {
                fullStructure.AddDataStructure(structure);
            }
            foreach (var conceptScheme in this._structure.ConceptSchemes)
            {
                fullStructure.AddConceptScheme(conceptScheme);
            }
            foreach (var mergedList in this._codelistCache.GetMergedItemScheme())
            {
                fullStructure.AddCodelist(mergedList);
            }
          
            return fullStructure;
        }

        /// <summary>
        /// Get the QueryComponent for the specified component
        /// </summary>
        /// <param name="component">
        /// The DSD Component
        /// </param>
        /// <returns>
        /// The <c>QueryComponent</c> or null if there was no criteria set
        /// </returns>
        public QueryComponent GetQueryComponent(IComponent component)
        {
            int pos = this._componentIndex[component];
            return this._queryComponentIndex[pos];
        }

        /// <summary>
        /// Gets the current collection of <see cref="QueryComponent"/>
        /// </summary>
        /// <returns>
        /// The current collection of <see cref="QueryComponent"/>
        /// </returns>
        public ICollection<QueryComponent> GetQueryComponents()
        {
            return this._queryComponentIndex;
        }

        /// <summary>
        /// Get the SDMXL-ML Dataset returned by NSIClient executeQuery
        /// </summary>
        /// <param name="rewind">
        /// If set to true and the stream supports seeking go to position 0. Else do nothing
        /// </param>
        /// <returns>
        /// The stream of the SDMX-ML Dataset
        /// </returns>
        public string GetSdmxMLDataSet(bool rewind)
        {
            if (this._dataSetSDMXML == null)
            {
                return null;
            }

            //if (rewind && this._dataSetSDMXML.CanSeek)
            //{
            //    this._dataSetSDMXML.Position = 0;
            //}

            return this._dataSetSDMXML;
        }

        /// <summary>
        /// Gets the session prefix.
        /// </summary>
        /// <returns>
        /// The session prefix.
        /// </returns>
        public string GetSessionPrefix()
        {
            return this._sessionPrefix;
        }

        /// <summary>
        /// Check if the dataflow tree is cached
        /// </summary>
        /// <returns>
        /// The has dataflow tree.
        /// </returns>
        public bool HasDataflowTree()
        {
            return this._dataflowTreeList != null;
        }

        /// <summary>
        /// Initialize the <see cref="_conceptMap"/> from ConceptSchemes contained in the specified structure
        /// </summary>
        /// <param name="structure">
        /// The structure containing the ConceptSchemes
        /// </param>
        public void InitializeConceptMap(ISdmxObjects structure)
        {
            this._conceptMap.Clear();
            var concepKeyToComponent = new Dictionary<string, IComponent>();
            if (this._keyFamily.DimensionList != null)
            {
                foreach (IDimension component in this._keyFamily.DimensionList.Dimensions)
                {
                    concepKeyToComponent.Add(Utils.MakeKeyForConcept(component), component);
                }
            }

            //if (this._keyFamily.TimeDimension != null)
            //{
            //    concepKeyToComponent.Add(
            //        Utils.MakeKeyForConcept(this._keyFamily.TimeDimension), this._keyFamily.TimeDimension);
            //}

            foreach (IConceptSchemeObject conceptScheme in structure.ConceptSchemes)
            {
                foreach (IConceptObject concept in conceptScheme.Items)
                {
                    string key = Utils.MakeKey(concept, conceptScheme);
                    IComponent component;
                    if (concepKeyToComponent.TryGetValue(key, out component))
                    {
                        this._conceptMap.UpdateItemObject(component, concept);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the <see cref="_dataflowMap"/> from Dataflows contained in the specified structure
        /// </summary>
        /// <param name="dataflows">
        /// The list of dataflows
        /// </param>
        public void InitializeDataflowMap(ISet<IDataflowObject> dataflows)
        {
            this._dataflowMap.Clear();
            foreach (IDataflowObject dataflow in dataflows)
            {
                string key = Utils.MakeKey(dataflow);
                if (!this._dataflowMap.ContainsKey(key))
                {
                    this._dataflowMap.Add(key, dataflow);
                }
            }
        }

        /// <summary>
        /// Checks if there are any edited components after the specified component.
        /// </summary>
        /// <param name="component">
        /// The component to check
        /// </param>
        /// <returns>
        /// True if there aren't any edited components after the specified component else false
        /// </returns>
        public bool IsLastEdited(IComponent component)
        {
            bool ret = true;
            int pos = this._componentIndex[component];
            for (int i = pos + 1, j = this._queryComponentIndex.Count; i < j && ret; i++)
            {
                if (this._queryComponentIndex[i] != null)
                {
                    ret = false;
                }
            }

            return ret;
        }

        /// <summary>
        /// Remove all component criteria
        /// </summary>
        public void RemoveAllComponent()
        {
            for (int i = 0, j = this._queryComponentIndex.Count; i < j; i++)
            {
                this._queryComponentIndex[i] = null;
            }
        }

        /// <summary>
        /// Remove all component criteria starting from the specified component and clear the data
        /// </summary>
        /// <param name="component">
        /// The component to remove
        /// </param>
        public void RemoveComponent(IComponent component)
        {
            int pos = this._componentIndex[component];
            CodeTreeBuilder tree;
            if (this._codeTreeMap.TryGetValue(component, out tree))
            {
                tree.CheckCodes(null);
            }

            for (int i = pos, j = this._queryComponentIndex.Count; i < j; i++)
            {
                this._queryComponentIndex[i] = null;
            }

            this.ClearData();
        }

        /// <summary>
        /// Reset everything
        /// </summary>
        public void Reset()
        {
            this._dataflow = null;
            this._structure = null;
            this._keyFamily = null;
            this._dataflowTreeList = null;
            this._componentIndex.Clear();
            this._conceptMap.Clear();
            this._criteriaComponentIdx = 0;
            this._workPageIdx = 0;
            this.Clear();
            this._sessionPrefix = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Reset dsds
        /// </summary>
        public void ResetDsds()
        {
              this._dsds.Clear();
        }

        /// <summary>
        /// Saves a component which does not have an associated artefact.
        /// </summary>
        /// <param name="conceptRef">
        /// the concept which identifies the component
        /// </param>
        /// <param name="value">
        /// the value
        /// </param>
        public void SaveComponent(string conceptRef, string value)
        {
            IDimension dimension = Utils.GetComponentByName(this._keyFamily, conceptRef);
            IConceptObject concept = this.GetCachedConcept(dimension);
            this.RemoveComponent(dimension);
            var qc = new QueryComponent(dimension, concept, value);

            this.UpdateComponentIndex(dimension, qc);
        }

        /// <summary>
        /// Saves a component which has an associated artefact (all of them should have).
        /// </summary>
        /// <param name="dimension">
        /// the dimemnsion
        /// </param>
        /// <param name="codes">
        /// the codes
        /// </param>
        public void SaveComponent(IDimension dimension, List<ICode> codes)
        {
            IConceptObject concept = this.GetCachedConcept(dimension);
            this.RemoveComponent(dimension);
            var qc = new QueryComponent(dimension, concept, codes);

            this._codeTreeMap[dimension].CheckCodes(codes);

            this.UpdateComponentIndex(dimension, qc);
        }

        /// <summary>
        /// Set the current selected tab
        /// </summary>
        /// <param name="component">
        /// The component.
        /// </param>
        public void SelectComponentTab(IComponent component)
        {
            int idx;
            this._componentIndex.TryGetValue(component, out idx);
            this._criteriaComponentIdx = idx;
        }

        /// <summary>
        /// Set the criteria page as the current working page
        /// </summary>
        public void SelectCriteriaPage()
        {
            this._workPageIdx = 0;
        }

        /// <summary>
        /// Set the results page as the current working page
        /// </summary>
        public void SelectResultsPage()
        {
            this._workPageIdx = 1;
        }

        /// <summary>
        /// Set the cache folder 
        /// </summary>
        /// <param name="folder">
        /// The path of the folder
        /// </param>
        public void SetCacheFolder(string folder)
        {
            this.SetCacheFolder(new DirectoryInfo(folder));
        }

        /// <summary>
        /// Set the cache folder 
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> instance of the cache folder
        /// </param>
        public void SetCacheFolder(DirectoryInfo directoryInfo)
        {
            this._cacheFolder = directoryInfo;
        }

        /// <summary>
        /// Cache the dataflow tree
        /// </summary>
        /// <param name="categories">
        /// The SDMX category schemes
        /// </param>
        /// <param name="dataflows">
        /// The SDMX dataflow schemes
        /// </param>
        /// /// <param name="categorisations">
        /// The SDMX categorisation
        /// </param>
        public void SetDataflowTree(IEnumerable<ICategorySchemeObject> categories, ISet<IDataflowObject> dataflows, IEnumerable<ICategorisationObject> categorisations, ISet<IDataStructureObject> dataStructure)
        {
            this._dataflowTreeList = new DataflowTreeBuilder(categories, dataflows,categorisations, dataStructure);
        }

        /// <summary>
        /// Set the SDMXL-ML Dataset returned by NSIClient executeQuery
        /// </summary>
        /// <param name="sdmxmlDataSet">
        /// The stream of the SDMX-ML Dataset
        /// </param>
        public void SetSdmxMLDataSet(string sdmxmlDataSet)
        {
            if (sdmxmlDataSet == null)
            {
                throw new ArgumentNullException("sdmxmlDataSet");
            }

            this._dataSetSDMXML = sdmxmlDataSet;
        }

        /// <summary>
        /// Setter for the time dimension
        /// </summary>
        /// <param name="startYear">
        /// The start year
        /// </param>
        /// <param name="endYear">
        /// The end year
        /// </param>
        /// <param name="startDate">
        /// The start date
        /// </param>
        /// <param name="endDate">
        /// The end date
        /// </param>
        public void SetTimeComponent(string startDate, string endDate)
        {

            IDimension dimension = this.KeyFamily.TimeDimension;
            IConceptObject concept = this.GetCachedConcept(dimension);
            if (dimension == null)
            {
                //DA FARE FABIO
                //throw new NsiClientException(Resources.ExceptionNoTimeDimension);
            }
        
                this.RemoveComponent(dimension);
                if (!string.IsNullOrEmpty(startDate))
                {
                    var qc = new QueryComponent(dimension, concept, startDate, endDate);
                    this.UpdateComponentIndex(dimension, qc);
                }
          
        }

        /// <summary>
        /// Update the codelist cache with the specified codelist and component
        /// </summary>
        /// <param name="codelist">
        /// The codelist
        /// </param>
        /// <param name="component">
        /// The component
        /// </param>
        public void UpdateCodelistMap(ICodelistObject codelist, IComponent component)
        {
            this._codelistCache.Update(component, codelist);
            this._codeTreeMap[component] = new CodeTreeBuilder(codelist);
            if (component.HasCodedRepresentation() && !string.IsNullOrEmpty(component.Representation.Representation.MaintainableReference.MaintainableId))
            {
                this.BuildCodeDescriptionMap(component);
            }
        }

        /// <summary>
        /// Update the <see cref="_queryComponentIndex"/> for the specified component with the specified <c>QueryComponent</c>
        /// </summary>
        /// <param name="component">
        /// The ComponentBean
        /// </param>
        /// <param name="queryComponent">
        /// The QueryComponent, can be null
        /// </param>
        public void UpdateComponentIndex(IComponent component, QueryComponent queryComponent)
        {
            int pos = this._componentIndex[component];
            this._queryComponentIndex[pos] = queryComponent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set data store.
        /// </summary>
        /// <param name="store">
        /// The store.
        /// </param>
        internal void SetDataStore(IDataSetStore store)
        {
            if (this._store != null)
            {
                this._store.Dispose();
            }

            this._store = store;
        }

        /// <summary>
        /// Build the code - description map for the specified component and add it to <see cref="_componentCodesDescriptionMap"/>.
        /// </summary>
        /// <param name="dim">
        /// The component
        /// </param>
        public void BuildCodeDescriptionMap(IComponent dim)
        {
            Dictionary<string, string> codeDescriptionMap;
            if (!this._componentCodesDescriptionMap.TryGetValue(dim.Id, out codeDescriptionMap))
            {
                codeDescriptionMap = new Dictionary<string, string>(StringComparer.Ordinal);
                this._componentCodesDescriptionMap.Add(dim.Id, codeDescriptionMap);
            }
            else
            {
                codeDescriptionMap.Clear();
            }

            ICodelistObject codelist = this.GetCachedCodelist(dim);
            string lang = this.CurrentCulture.TwoLetterISOLanguageName;
            foreach (ICode code in codelist.Items)
            {
                string desc =  TextTypeHelper.GetText(code.Names,lang);
                if (string.IsNullOrEmpty(desc))
                {
                    desc = code.Id;
                }

                codeDescriptionMap.Add(code.Id, desc);
            }
        }

        /// <summary>
        /// Initialize the <see cref="_componentIndex"/> and <see cref="_queryComponentIndex"/> fields for the specified KeyFamilyBean
        /// </summary>
        /// <param name="kf">
        /// The Keyfamily
        /// </param>
        public void InitializeComponentList(IDataStructureObject kf)
        {
            this._componentIndex.Clear();
            this._queryComponentIndex.Clear();
            int x = 0;

            if (kf.DimensionList != null)
            {
                foreach (IDimension comp in kf.DimensionList.Dimensions)
                {
                    this._componentIndex.Add(comp, x);
                    this._queryComponentIndex.Add(null);
                    x++;
                }
            }

            //if (kf.TimeDimension != null)
            //{
            //    this._componentIndex.Add(kf.TimeDimension, x);
            //    this._queryComponentIndex.Add(null);
            //}
        }

        /// <summary>
        /// Rebuild the <see cref="_componentCodesDescriptionMap"/> for all keys.
        /// </summary>
        public void RebuildCodeDescriptionMap()
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> pair in this._componentCodesDescriptionMap)
            {
                IComponent dim = Utils.GetComponentByName(this.KeyFamily, pair.Key);
                this.BuildCodeDescriptionMap(dim);
            }
        }

        #endregion
    }
}
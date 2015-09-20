namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Text;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using System.Linq;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    using ISTAT.WebClient.WidgetEngine.Model.DataReader;
    using ISTAT.WebClient.WidgetComplements.Model;

    /// <summary>
    /// Abstract dataset model
    /// </summary>
    public abstract class AbstractDataSetModel
    {
        #region Constants and Fields

        private readonly List<string> _infoKeys = new List<string>();

        /// <summary>
        /// Contains all dimensions from a keyfamily with concept id as key and concept primary name as value
        /// </summary>
        private readonly Dictionary<string, string> _allValidKeys =
            new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// The concept map
        /// </summary>
        private readonly Dictionary<string, IConceptObject> _conceptMap =
            new Dictionary<string, IConceptObject>(StringComparer.Ordinal);

        /// <summary>
        /// The Horizontal key collection
        /// </summary>
        private readonly List<string> _horizontalKeys = new List<string>();

        /// <summary>
        /// The count of values for X and Y axis keys
        /// </summary>
        private readonly Dictionary<string, long> _horizontalVerticalKeyCount =
            new Dictionary<string, long>(StringComparer.Ordinal);

        /// <summary>
        /// Holds the current code display mode of a dimension
        /// </summary>
        private readonly Dictionary<string, DisplayModeStatus> _keyDisplayMode =
            new Dictionary<string, DisplayModeStatus>(StringComparer.Ordinal);

        /// <summary>
        /// The key family
        /// </summary>
        private readonly IDataStructureObject _keyFamily;
        private readonly IDataflowObject _dataFlow;

        /// <summary>
        /// A map between the a slice key and it's valid values
        /// </summary>
        private readonly ComponentValueDictionary _sliceKeyValidValues = new ComponentValueDictionary();

        /// <summary>
        /// The Slice key collection
        /// </summary>
        private readonly OrderedDictionary _sliceKeys = new OrderedDictionary(StringComparer.Ordinal);

        /// <summary>
        /// The IDataSetStore that will be used to by implementations that use IDataSetStore
        /// </summary>
        private readonly IDataSetStore _store;

        /// <summary>
        /// The SDMX ML structure
        /// </summary>
        private readonly ISdmxObjects _structure;

        /// <summary>
        /// Holds a set of all keys. It is used to check for duplicates.
        /// The value is not used.
        /// </summary>
        private readonly Dictionary<string, object> _uniqueKeyIndex =
            new Dictionary<string, object>(StringComparer.Ordinal);

        /// <summary>
        /// The Vertical key collection
        /// </summary>
        private readonly List<string> _verticalKeys = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDataSetModel"/> class. 
        /// </summary>
        /// <param name="structure">
        /// The SDMX-ML structure
        /// </param>
        /// <param name="store">
        /// The <see cref="IDataSetStore"/> containing the dataset data
        /// </param>
        protected AbstractDataSetModel(ISdmxObjects structure, IDataSetStore store)
        {
            if (structure == null)
            {
                throw new ArgumentNullException("structure");
            }

            this._store = store;
            this._structure = structure;

            // The structure must define exactly one key family...
            if ((this._structure.DataStructures == null) || (this._structure.DataStructures.Count != 1))
            {
                throw new Exception(
                    "Invalid structure bean, it does not contain the definition for exactly one key family!");
            }
            this._dataFlow = this._structure.Dataflows.First();
            this._keyFamily = this._structure.DataStructures.First();
        }

        #endregion

        #region Public Properties

        public IList<string> InfoKeys
        {
            get
            {
                return this._infoKeys;
            }
        }

        public bool BlockAxisX { get; set; }
        public bool BlockAxisY { get; set; }
        public bool UseDefaultQuery { get; set; }

        /// <summary>
        /// Gets a map of all DSD Dimensions and possibly Time Dimensions ConceptRef to Concept Name
        /// </summary>
        public Dictionary<string, string> AllValidKeys
        {
            get
            {
                return this._allValidKeys;
            }
        }

        /// <summary>
        /// Gets the Horizontal keys
        /// </summary>
        public IList<string> HorizontalKeys
        {
            get
            {
                return this._horizontalKeys;
            }
        }

        /// <summary>
        /// Gets the count of values for X and Y axis keys
        /// </summary>
        public IDictionary<string, long> HorizontalVerticalKeyCount
        {
            get
            {
                return this._horizontalVerticalKeyCount;
            }
        }

        /// <summary>
        /// Gets the SDMX DSD (i.e. KeyFamily)
        /// </summary>
        public IDataStructureObject KeyFamily
        {
            get
            {
                return this._keyFamily;
            }
        }

        /// <summary>
        /// Gets the map between a slice key and it's valid values
        /// </summary>
        public ComponentValueDictionary SliceKeyValidValues
        {
            get
            {
                return this._sliceKeyValidValues;
            }
        }

        /// <summary>
        /// Gets the slice keys
        /// </summary>
        public OrderedDictionary SliceKeyValues
        {
            get
            {
                return this._sliceKeys;
            }
        }

        /// <summary>
        /// Gets the slice keys
        /// </summary>
        public ICollection SliceKeys
        {
            get
            {
                return this._sliceKeys.Keys;
            }
        }

        /// <summary>
        /// Gets the vertical keys
        /// </summary>
        public IList<string> VerticalKeys
        {
            get
            {
                return this._verticalKeys;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the IDataSetStore that will be used to by implementations that use IDataSetStore
        /// </summary>
        protected internal IDataSetStore Store
        {
            get
            {
                return this._store;
            }
        }

        /// <summary>
        /// Gets the SDMX ML structure
        /// </summary>
        protected internal ISdmxObjects Structure
        {
            get
            {
                return this._structure;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Display mode for a specific code
        /// </summary>
        /// <param name="dimension">
        /// The dimension that the code belongs to
        /// </param>
        /// <param name="currentValue">
        /// The code value (code ID)
        /// </param>
        /// <returns>
        /// The display mode of the specified code
        /// </returns>
        public DisplayMode GetDisplayMode(string dimension, string currentValue)
        {
            DisplayModeStatus status;
            return this._keyDisplayMode.TryGetValue(dimension, out status)
                       ? status.GetDisplayMode(currentValue)
                       : DisplayModeStatus.DefaultMode;
        }

        /// <summary>
        /// Initialize the <see cref="AbstractDataSetModel"/> instance
        /// </summary>
        public void Initialize()
        {
            // populate the concept.id to concept.name map and the list of slice keys
            Utils.PopulateConceptMap(this._structure, this._conceptMap);

            var slice = new List<string>(this._keyFamily.DimensionList.Dimensions.Count + 1);
            foreach (IDimension dimension in this._keyFamily.DimensionList.Dimensions)
            {
                slice.Add(dimension.Id);
            }
            List<string> sliceAttribute=null;
            if (this._keyFamily.AttributeList!=null)
            {
                sliceAttribute = new List<string>(this._keyFamily.AttributeList.Attributes.Count + 1);
                foreach (IAttributeObject attribute in this._keyFamily.AttributeList.Attributes)
                {
                    sliceAttribute.Add(attribute.Id);
                }
            }


            // populate all valid keys
            this.ReloadValidKeys();

            this.ApplyDefaultSettings(slice);
            this.SetupCurrentView(slice, this._horizontalKeys, this._verticalKeys);
            this.SetupSliceKeys(slice);
            this.SetupKeyXyCount();

            this._infoKeys.Clear();
            if (sliceAttribute!=null)
                this._infoKeys.AddRange(sliceAttribute);

        }

        /// <summary>
        /// Reload the <see cref="AllValidKeys"/> in order to get the current localized description
        /// </summary>
        public void ReloadValidKeys()
        {
            lock (this._allValidKeys)
            {
                this._allValidKeys.Clear();
                foreach (IDimension dimension in this._keyFamily.DimensionList.Dimensions)
                {
                    this._allValidKeys.Add(
                        dimension.Id,
                        Utils.GetLocalizedName(Utils.GetCachedConcept(dimension, this._conceptMap)));
                }
                if (this._keyFamily.AttributeList!=null)
                    foreach (IAttributeObject attribute in this._keyFamily.AttributeList.Attributes)
                    {
                        this._allValidKeys.Add(
                            attribute.Id,
                            Utils.GetLocalizedName(Utils.GetCachedConcept(attribute, this._conceptMap)));
                    }
            }
        }

        /// <summary>
        /// Reset all dimension display mode
        /// </summary>
        public void ResetDisplayMode()
        {
            this._keyDisplayMode.Clear();
        }

        /// <summary>
        /// Toggles the dimension wide display mode
        /// </summary>
        /// <param name="key">
        /// The key (dimension)
        /// </param>
        public void ToggleKeyDisplayValue(string key)
        {
            DisplayModeStatus status;
            if (!this._keyDisplayMode.TryGetValue(key, out status))
            {
                status = new DisplayModeStatus();
                this._keyDisplayMode.Add(key, status);
            }

            status.ToggleDisplayMode();
        }

        /// <summary>
        /// Toggles a specific code display mode
        /// </summary>
        /// <param name="key">
        /// The key (dimension)
        /// </param>
        /// <param name="value">
        /// The code value
        /// </param>
        public void ToggleKeyDisplayValue(string key, string value)
        {
            DisplayModeStatus status;
            if (!this._keyDisplayMode.TryGetValue(key, out status))
            {
                status = new DisplayModeStatus();
                this._keyDisplayMode.Add(key, status);
            }

            status.ToggleDisplayMode(value);
        }

        /// <summary>
        /// Updates the axis for all keys.
        /// </summary>
        /// <param name="slice">
        /// the slice keys
        /// </param>
        /// <param name="horizontal">
        /// the horizontal keys
        /// </param>
        /// <param name="vertical">
        /// the vertical keys
        /// </param>
        public void UpdateAxis(List<string> slice, List<string> horizontal, List<string> vertical)
        {
            this.SetupCurrentView(slice, horizontal, vertical);

            this._horizontalKeys.Clear();
            this._horizontalKeys.AddRange(horizontal);

            this._verticalKeys.Clear();
            this._verticalKeys.AddRange(vertical);

            this.SetupSliceKeys(slice);
            this.SetupKeyXyCount();
        }

        /// <summary>
        /// Updates the value for a slice key.
        /// </summary>
        /// <param name="key">
        /// the key
        /// </param>
        /// <param name="value">
        /// the new value
        /// </param>
        public void UpdateSliceKeyValue(string key, string value)
        {
            if (!this._sliceKeys.Contains(key))
            {
                throw new Exception("Unable to find slice key '" + key + "'");
            }

            this._sliceKeys[key] = value;
            this.SetupSliceKeys(new ArrayList(this._sliceKeys.Keys));
            this.SetupKeyXyCount();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds an SQL "order by" with the specified <paramref name="keys"/>
        /// </summary>
        /// <param name="keys">
        /// The collection of keys (i.e. dimensions)
        /// </param>
        /// <returns>
        /// The build sort order.
        /// </returns>
        protected static string BuildSortOrder(IList<string> keys)
        {
            var sb = new StringBuilder();
            foreach (string key in keys)
            {
                sb.Append(key);
                sb.Append(" ASC,");
            }

            sb.Length--;
            return sb.ToString();
        }

        /// <summary>
        /// Append the given key = value to the filter of the current slice view
        /// </summary>
        /// <param name="key">
        /// The key name
        /// </param>
        /// <param name="value">
        /// The key value
        /// </param>
        protected abstract void AppendFilter(string key, string value);

        /// <summary>
        /// Apply default settings to the data set model
        /// </summary>
        /// <param name="slice">
        /// All dimensions. The list will modified. Everything that is left is considered to be slice axis dimensions
        /// </param>
        protected void ApplyDefaultSettings(IList<string> slice)
        {
            //Dictionary<string, LayoutAxis> defaultAxis = LayoutManager.Instance.GetDefaultAxis(this._dataFlow);

            //this.BlockAxisX = LayoutManager.Instance.IsBlockAxisX(this._dataFlow);
            //this.BlockAxisY = LayoutManager.Instance.IsBlockAxisY(this._dataFlow);
            //this.UseDefaultQuery = LayoutManager.Instance.UseDefaultQuery(this._dataFlow);
            //if (defaultAxis != null)
            //{
            //    var toDelete = new Stack<int>(slice.Count);

            //    for (int i = 0, j = slice.Count; i < j; i++)
            //    {
            //        LayoutAxis axis;
            //        string concept = slice[i];
            //        if (defaultAxis.TryGetValue(concept, out axis) && axis != LayoutAxis.Slice)
            //        {
            //            switch (axis)
            //            {
            //                case LayoutAxis.Horizontal:

            //                    this._horizontalKeys.Add(concept);
            //                    break;
            //                case LayoutAxis.Vertical:

            //                    this._verticalKeys.Add(concept);
            //                    break;
            //            }

            //            toDelete.Push(i);
            //        }
            //    }

            //    while (toDelete.Count > 0)
            //    {
            //        int i = toDelete.Pop();
            //        slice.RemoveAt(i);
            //    }
            //}
        }

        /// <summary>
        /// Clear the filter of the current slice view
        /// </summary>
        protected abstract void ClearFilter();

        /// <summary>
        /// Get the distinct count of the specified key
        /// </summary>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <returns>
        /// The distinct count
        /// </returns>
        protected abstract long GetCount(string key);

        /// <summary>
        /// Get the set of distinct values of the specified key
        /// </summary>
        /// <param name="key">
        /// The key name
        /// </param>
        /// <returns>
        /// A Set of distinct values
        /// </returns>
        protected abstract IDictionary<string, object> GetDistinctValues(string key);

        /// <summary>
        /// Reset the display mode of the specified key (i.e. dimension)
        /// </summary>
        /// <param name="dimension">
        /// The key name
        /// </param>
        protected void ResetKeyDisplayMode(string dimension)
        {
            this._keyDisplayMode.Remove(dimension);
        }

        /// <summary>
        /// Set the sort order of the current slice view
        /// </summary>
        /// <param name="sortOrder">
        /// The sort order. The syntax should follow the SQL ORDER BY clauses.
        /// </param>
        protected abstract void SetSort(IList<string> sortOrder);

        /// <summary>
        /// Setup the current view. Reset the row filter, validate the keys and build the sort order
        /// </summary>
        /// <param name="slice">
        /// the slice keys
        /// </param>
        /// <param name="horizontal">
        /// the horizontal keys
        /// </param>
        /// <param name="vertical">
        /// the vertical keys
        /// </param>
        protected void SetupCurrentView(List<string> slice, List<string> horizontal, List<string> vertical)
        {
            // sort the current view
            var sort = new List<string>();
            sort.AddRange(slice); // first the slice, if any
            sort.AddRange(horizontal); // then the horrizontal, if any
            sort.AddRange(vertical); // then the vertical, if any 
            this.ValidateKeys(sort);
            this.ClearFilter();
            this.SetSort(sort);
            this.SetupKeyXyCount();
        }

        /// <summary>
        /// Add the count for each key to <see cref="_horizontalVerticalKeyCount"/>
        /// </summary>
        /// <param name="keys">
        /// The keys
        /// </param>
        protected void SetupKeyCount(ICollection<string> keys)
        {
            foreach (var key in keys)
            {
                this._horizontalVerticalKeyCount.Add(key, this.GetCount(key));
            }
        }

        /// <summary>
        /// Setup the slice key maps with the current slice values and the valid values
        /// </summary>
        /// <param name="keys">
        /// The list of keys to setup as slice keys
        /// </param>
        protected void SetupSliceKeys(ICollection keys)
        {
            var oldSliceKeys = new OrderedDictionary();
            foreach (DictionaryEntry e in this._sliceKeys)
            {
                oldSliceKeys.Add(e.Key, e.Value);
            }

            this._sliceKeys.Clear();

            this._sliceKeyValidValues.Clear();
            this.ClearFilter();

            // TODO move (part?) of the following to ApplyFilter for SRA-142
            foreach (string key in keys)
            {
                IDictionary<string, object> values = this.GetDistinctValues(key);
                if (values.Count == 0)
                {
                    throw new Exception(ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources.Messages.no_result_found);
                }

                var value = oldSliceKeys[key] as string;
                if (value == null || !values.ContainsKey(value))
                {
                    IEnumerator<string> lookup = values.Keys.GetEnumerator();
                    if (lookup.MoveNext())
                    {
                        value = lookup.Current;
                    }

                    lookup.Dispose();
                }

                this._sliceKeyValidValues.Add(key, new List<string>(values.Keys));
                this._sliceKeys.Add(key, value);
                if (values.Count > 1)
                {
                    this.AppendFilter(key, value);
                }
            }
        }

        /// <summary>
        /// Validates a set of keys against the dimension set
        /// </summary>
        /// <param name="newKeys">
        /// the keys to validate
        /// </param>
        protected void ValidateKeys(IEnumerable<string> newKeys)
        {
            this._uniqueKeyIndex.Clear();
            foreach (string newKey in newKeys)
            {
                if (!this._allValidKeys.ContainsKey(newKey) || this._uniqueKeyIndex.ContainsKey(newKey))
                {
                    throw new Exception("Invalid/duplicated key: " + newKey);
                }

                this._uniqueKeyIndex.Add(newKey, null);
            }
        }

        /// <summary>
        /// Setup the <see cref="_horizontalVerticalKeyCount"/>
        /// </summary>
        private void SetupKeyXyCount()
        {
            this._horizontalVerticalKeyCount.Clear();
            this.SetupKeyCount(this._horizontalKeys);
            this.SetupKeyCount(this._verticalKeys);
        }

        #endregion
    }
}
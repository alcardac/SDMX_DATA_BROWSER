namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using ISTAT.WebClient.WidgetEngine.Model.DataReader;


    /// <summary>
    /// An implementation of the IDataSetModel inteface that uses the <see cref="IDataSetStore"/>
    /// </summary>
    public class DataSetModelStore : AbstractDataSetModel, IDataSetModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetModelStore"/> class. 
        /// Initialize a new instance of the <see cref="DataSetModelStore"/>
        /// </summary>
        /// <param name="structure">
        /// The SDMX structure file
        /// </param>
        /// <param name="store">
        /// The <see cref="IDataSetStore"/>
        /// </param>
        public DataSetModelStore(ISdmxObjects structure, IDataSetStore store)
            : base(structure, store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the first observation if there is any slice data. 
        /// </summary>
        public string FirstObservation
        {
            get
            {
                using (IDataReader reader = this.GetReader(true))
                {
                    if (reader.Read())
                    {
                        int idx = reader.GetOrdinal(this.KeyFamily.PrimaryMeasure.Id);
                        return reader.GetString(idx);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the a data reader for the all data unsorted.
        /// </summary>
        /// <value>
        ///   A DataReader with all the data
        /// </value>
        public IDataReader UnsortedReader
        {
            get
            {
                return this.Store.CreateDataReaderAllUnsorted();
            }
        }

        public IDictionary<string, object> DatasetAttribute
        {
            get
            {
                using (IDataReader reader = this.GetReader(true))
                {
                    if (reader.Read())
                    {
                        Dictionary<string, object> _attr = new Dictionary<string, object>();

                        foreach (var attr in this.KeyFamily.DatasetAttributes)
                        {
                            var value = reader.GetValue(reader.GetOrdinal(attr.Id));
                            if (!string.IsNullOrEmpty(value.ToString())) _attr.Add(attr.Id, value);
                        }

                        return _attr;
                    }
                }

                return null;
            }
        }
        public IDictionary<string, object> ObservationAttribute
        {
            get
            {
                using (IDataReader reader = this.GetReader(true))
                {
                    if (reader.Read())
                    {
                        Dictionary<string, object> _attr = new Dictionary<string, object>();

                        foreach (var attr in this.KeyFamily.ObservationAttributes)
                        {
                            var value = reader.GetValue(reader.GetOrdinal(attr.Id));
                            if (!string.IsNullOrEmpty(value.ToString())) _attr.Add(attr.Id, value);
                        }

                        return _attr;
                    }
                }

                return null;
            }
        }
        public IDictionary<string, object> GroupAttribute
        {
            get
            {
                using (IDataReader reader = this.GetReader(true))
                {
                    if (reader.Read())
                    {
                        Dictionary<string, object> _attr = new Dictionary<string, object>();

                        foreach (var attr in this.KeyFamily.GroupAttributes)
                        {
                            var value = reader.GetValue(reader.GetOrdinal(attr.Id));
                            if (!string.IsNullOrEmpty(value.ToString())) _attr.Add(attr.Id, value);
                        }

                        return _attr;
                    }
                }

                return null;
            }
        }
        public IDictionary<string, object> DimensionGroupAttribute
        {
            get
            {
                using (IDataReader reader = this.GetReader(true))
                {
                    if (reader.Read())
                    {
                        Dictionary<string, object> _attr = new Dictionary<string, object>();

                        foreach (var attr in this.KeyFamily.DimensionGroupAttributes)
                        {
                            var value = reader.GetValue(reader.GetOrdinal(attr.Id));
                            if (!string.IsNullOrEmpty(value.ToString())) _attr.Add(attr.Id, value);
                        }

                        return _attr;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a DataReader of the sorted data ( Z-Axis dimensions first). 
        /// </summary>
        /// <param name="slice">
        /// If set to true it should return 
        /// </param>
        /// <returns>
        /// the <see cref="IDataReader"/>
        /// </returns>
        public IDataReader GetReader(bool slice)
        {
            return this.Store.CreateDataReader(!slice);
        }

        /// <summary>
        /// Get the count of rows from either the current slice or all data
        /// </summary>
        /// <param name="fromSlice">
        /// Whether to check from slice or not
        /// </param>
        /// <returns>
        /// the count of rows from either the current slice or all data
        /// </returns>
        public int GetRowCount(bool fromSlice)
        {
            return this.Store.Count(!fromSlice);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Append the given key = value to the filter of the current slice view
        /// </summary>
        /// <param name="key">
        /// The key name
        /// </param>
        /// <param name="value">
        /// The key value
        /// </param>
        protected override void AppendFilter(string key, string value)
        {
            this.Store.AddCriteria(new DataCriteria() { component = key, values = new List<string>() { value } });
        }

        /// <summary>
        /// Clear the filter of the current slice view
        /// </summary>
        protected override void ClearFilter()
        {
            this.Store.SetCriteria(null);
        }

        /// <summary>
        /// Get the distinct count of the specified key
        /// </summary>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <returns>
        /// The distinct count
        /// </returns>
        protected override long GetCount(string key)
        {
            return this.Store.Count(key);
        }

        /// <summary>
        /// Get the set of distinct values of the specified key
        /// </summary>
        /// <param name="key">
        /// The key name
        /// </param>
        /// <returns>
        /// A Set of distinct values
        /// </returns>
        protected override IDictionary<string, object> GetDistinctValues(string key)
        {
            return this.Store.SelectDistinct<string>(key);
        }

        /// <summary>
        /// Set the sort order of the current slice view
        /// </summary>
        /// <param name="sortOrder">
        /// The sort order. The syntax should follow the SQL ORDER BY clauses.
        /// </param>
        protected override void SetSort(IList<string> sortOrder)
        {
            this.Store.SetSort(sortOrder);
        }

        #endregion
    }
}
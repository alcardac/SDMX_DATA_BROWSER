// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetStoreDataView.cs" company="Eurostat">
//   Date Created : 2011-04-05
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This is an implementation if the <see cref="IDataSetStore" />  that stores the SDMX-ML dataset inside a <see cref="DataTable" /> and a <see cref="DataView" />.
//   The <see cref="DataView" /> is used to get filtered and/or sorted data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetEngine.Model.DataReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Xml;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

    /// <summary>
    /// This is an implementation if the <see cref="IDataSetStore"/>  that stores the SDMX-ML dataset inside a <see cref="DataTable"/> and a <see cref="DataView"/>.
    /// The <see cref="DataView"/> is used to get filtered and/or sorted data.
    /// </summary>
    /// <remarks>
    /// This is implementation is not recommeded for big SDMX-ML datasets (&gt; 5-10 thousand observations) as <see cref="DataTable"/> and <see cref="DataView"/> consume a lot of memory and are very slow
    /// </remarks>
    public class DataSetStoreDataView : IDataSetStore
    {
        #region Constants and Fields

        /// <summary>
        /// Holds the current view of the <see cref="_dataTable"/>
        /// </summary>
        private DataView _currentView;

        /// <summary>
        /// Holds the table with all data
        /// </summary>
        private DataTable _dataTable;

        /// <summary>
        /// The measure dimension Column. If none exists it will be set to null
        /// </summary>
        private DataColumn _measureColumn;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetStoreDataView"/> class. 
        /// </summary>
        /// <param name="keyFamily">
        /// The <see cref="KeyFamilyBean"/>
        /// </param>
        public DataSetStoreDataView(IDataStructureObject keyFamily)
        {
            this._dataTable = new DataTable("obs") { Locale = CultureInfo.InvariantCulture };
            if (keyFamily == null)
            {
                throw new ArgumentNullException("keyFamily");
            }

            this.InitTable(keyFamily);
            this._currentView = new DataView(this._dataTable);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DataSetStoreDataView"/> class. 
        /// </summary>
        ~DataSetStoreDataView()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the measure dimension concept ref
        /// </summary>
        public string MeasureColumn
        {
            get
            {
                return this._measureColumn != null ? this._measureColumn.ColumnName : null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SDMX Attributes should be parsed. Default is false
        /// </summary>
        public bool ParseSdmxAttributes { get { return true; } set { } }

        #endregion

        #region Public Methods

        public List<string> GetAllColumns()
        { return null;}
        
        /// <summary>
        /// Add (Append) to criteria the given key value pair 
        /// </summary>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <param name="value">
        /// The key's value
        /// </param>
        public void AddCriteria(string key, string value)
        {
            string and = string.Empty;
            if (!string.IsNullOrEmpty(this._currentView.RowFilter))
            {
                and = this._currentView.RowFilter + " and ";
            }

            this._currentView.RowFilter = and + key + "='" + value + "'";
        }

        /// <summary>
        /// Add row to dataset store. Each row should contain the values added with <see cref="AddToStore(XmlReader)"/> or <see cref="AddToStore(string,string)"/>. It should be called after each iteration after all <c>AddToStore</c> overloads
        /// Must be called after <see cref="BeginDataSetImport"/>  and before <see cref="Commit"/>
        /// In this implementation it adds a new row to the data table
        /// </summary>
        public void AddRow()
        {
            DataRow row = this._dataTable.NewRow();
            this._dataTable.Rows.Add(row);
        }

        /// <summary>
        /// Add the current XML attributes to sdmx dataset store. 
        /// Only XML attributes that exist in the dataset store are imported and it is determined by <see cref="ExistsColumn"/>
        /// Must be called after <see cref="BeginDataSetImport"/> and before <see cref="Commit"/>
        /// In this implementation it sets the default values for corresponding columns
        /// </summary>
        /// <param name="reader">
        /// The XMLReader instance to read the XML attributes from
        /// </param>
        public void AddToStore(XmlReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                string name = reader.LocalName;
                this.AddToStore(name, reader.Value);
            }
        }

        /// <summary>
        /// Add the specified key and value to sdmx dataset store. 
        /// The key must by a valid column else it is ignored and it is determined by <see cref="ExistsColumn"/> 
        /// Must be called after <see cref="BeginDataSetImport"/>  and before <see cref="Commit"/>
        /// In this implementation it sets the default values for corresponding columns
        /// </summary>
        /// <param name="key">
        /// The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        public void AddToStore(string key, string value)
        {
            if (this.ExistsColumn(key))
            {
                this._dataTable.Columns[key].DefaultValue = value;
            }
        }

        /// <summary>
        /// Not used in this implementation
        /// </summary>
        public void BeginDataSetImport()
        {
        }

        /// <summary>
        /// Commit the added rows with <see cref="AddRow"/>. Must be called after all calls of <see cref="AddRow"/>
        /// in this implementation the datatable accepts all changes
        /// </summary>
        public void Commit()
        {
            this._dataTable.AcceptChanges();
        }

        /// <summary>
        /// Get the distinct count of the specified key
        /// Criteria added either by <see cref="AddCriteria"/> or <see cref="SetCriteria"/> are considered
        /// </summary>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <returns>
        /// The distinct count
        /// </returns>
        public long Count(string key)
        {
            IDictionary<object, object> ht = new Dictionary<object, object>();
            int idx = this._currentView.Table.Columns[key].Ordinal;

            for (int i = 0, j = this._currentView.Count; i < j; i++)
            {
                DataRowView dr = this._currentView[i];
                object val = dr[idx];
                if (val != null && !Convert.IsDBNull(val) && (!ht.ContainsKey(val)))
                {
                    ht.Add(val, null);
                }
            }

            return ht.Count;
        }

        /// <summary>
        /// Get count of the records
        /// </summary>
        /// <param name="all">
        /// If set to true it will ignore criteria else it will use them
        /// </param>
        /// <returns>
        /// The number of records
        /// </returns>
        public int Count(bool all)
        {
            return !all ? this._currentView.Count : this._dataTable.Rows.Count;
        }

        /// <summary>
        /// Create a sorted <see cref="IDataReader"/>
        /// </summary>
        /// <param name="all">
        /// If set to true it will ignore criteria else it will use them
        /// </param>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        public IDataReader CreateDataReader(bool all)
        {
            return this.GetReader(!all);
        }

        /// <summary>
        /// Create a unsorted <see cref="IDataReader"/>. It ignores sort and criteria
        /// </summary>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        public IDataReader CreateDataReaderAllUnsorted()
        {
            return this._dataTable.CreateDataReader();
        }

        /// <summary>
        /// Dispose any open DataView and DataTable
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Check if the given name exists as a column in SDMX dataset.
        /// </summary>
        /// <param name="name">
        /// The name of the key i.e. the concept ref of the component, the xml attribute name
        /// </param>
        /// <returns>
        /// True if it exists else false
        /// </returns>
        public bool ExistsColumn(string name)
        {
            return this._dataTable.Columns.Contains(name);
        }

        /// <summary>
        /// Reset all criteria and sort
        /// </summary>
        public void ResetCriteriaSort()
        {
            this._currentView.RowFilter = null;
            this._currentView.Sort = null;
        }



        /// <summary>
        /// Perform a select distinct on the specified key. 
        /// Criteria added either by <see cref="AddCriteria"/> or <see cref="SetCriteria"/> are considered
        /// </summary>
        /// <typeparam name="T">
        /// The type of key values. In most cases it should be <see cref="System.String"/>
        /// </typeparam>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <returns>
        /// A Set of unique values
        /// </returns>
        public IDictionary<T, object> SelectDistinct<T>(string key)
        {
            IDictionary<T, object> ht = new Dictionary<T, object>();
            int idx = this._currentView.Table.Columns[key].Ordinal;

            for (int i = 0, j = this._currentView.Count; i < j; i++)
            {
                DataRowView dr = this._currentView[i];
                object val = dr[idx];
                if (val != null && !Convert.IsDBNull(val))
                {
                    var tv = (T)val;
                    if (!ht.ContainsKey(tv))
                    {
                        ht.Add(tv, null);
                    }
                }
            }

            return new SortedDictionary<T, object>(ht);
        }

        /// <summary>
        /// Set criteria to the given map of key and value.
        /// </summary>
        /// <param name="criteria">
        /// A map of key and value.  Set to 
        /// <code>
        /// null
        /// </code>
        /// or empty map to reset all criteria
        /// </param>
        /// 
        public void SetCriteria(List<DataCriteria> Criterias)
        { 
        }

        public void SetCriteria(Dictionary<string, string> criteria)
        {
            if (criteria == null)
            {
                this._currentView.RowFilter = null;
                return;
            }

            var rowFilter = new StringBuilder();
            int lastClause = 0;
            foreach (KeyValuePair<string, string> kv in criteria)
            {
                rowFilter.AppendFormat("{0}='{1}'", kv.Key, kv.Value);
                lastClause = rowFilter.Length;
                rowFilter.Append(" and ");
            }

            rowFilter.Length = lastClause;
            this._currentView.RowFilter = rowFilter.ToString();
        }


        /// <summary>
        /// Set sort order, i.e. order by, to the specified list. All entries will be added as ascenting and order of the list matters.
        /// </summary>
        /// <param name="sort">
        /// The pre-ordered list of keys to include when sorting
        /// </param>
        public void SetSort(IList<string> sort)
        {
            this._currentView.Sort = BuildSortOrder(sort);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the sort order <see cref="string"/> using the specified <paramref name="keys"/>
        /// </summary>
        /// <param name="keys">
        /// The collection of keys
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
        /// Dispose any open DataView and DataTable
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether to dispose managed resources or just native
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._currentView != null)
                {
                    this._currentView.Dispose();
                    this._currentView = null;
                }

                if (this._dataTable != null)
                {
                    this._dataTable.Dispose();
                    this._dataTable = null;
                }
            }
        }

        /// <summary>
        /// Get a DataReader of the sorted data ( Z-Axis dimensions first). 
        /// </summary>
        /// <param name="slice">
        /// If set to true it should return 
        /// </param>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        private IDataReader GetReader(bool slice)
        {
            if (slice)
            {
                return new DataViewReader(this._currentView);
            }

            var all = new DataView(this._dataTable) { Sort = this._currentView.Sort };
            return new DataViewReader(all);
        }

        /// <summary>
        /// Initialize table
        /// </summary>
        /// <param name="dsd">
        /// The keyfamily DSD
        /// </param>
        private void InitTable(IDataStructureObject dsd)
        {
            this._dataTable.CaseSensitive = true;
            var pks = new List<DataColumn>();
            DataColumn c;
            this._measureColumn = null;
            foreach (IDimension comp in dsd.DimensionList.Dimensions)
            {
                c = new DataColumn(comp.Id);
                if (comp.MeasureDimension)
                {
                    this._measureColumn = c;
                }

                c.DataType = typeof(string);
                this._dataTable.Columns.Add(c);
                pks.Add(c);
            }

            this._dataTable.PrimaryKey = pks.ToArray();
            c = new DataColumn(dsd.PrimaryMeasure.Id);
            c.DataType = typeof(string);
            this._dataTable.Columns.Add(c);



            if (this.ParseSdmxAttributes)
            {
                foreach (IAttributeObject comp in dsd.Attributes)
                {
                    c = new DataColumn(comp.Id) { DataType = typeof(string) };
                    this._dataTable.Columns.Add(c);
                }
            }
        }

        #endregion
    }
}
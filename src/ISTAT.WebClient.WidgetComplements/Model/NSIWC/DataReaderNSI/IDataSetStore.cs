// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSetStore.cs" company="Eurostat">
//   Date Created : 2011-04-03
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Interface to implementations that store SDMX DataSets
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.DataReaderNSI
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Xml;
    using ISTAT.WebClient.WidgetComplements.NSIWC;

    /// <summary>
    /// Interface to implementations that store SDMX DataSets
    /// </summary>
    public interface IDataSetStore : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets the measure dimension concept ref
        /// </summary>
        string MeasureColumn { get; }

        /// <summary>
        /// Gets or sets a value indicating whether SDMX Attributes should be parsed. Default is false
        /// </summary>
        bool ParseSdmxAttributes { get; set; }



        #endregion

        #region Public Methods

        /// <summary>
        /// Add (Append) to criteria the given key value pair 
        /// </summary>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <param name="value">
        /// The key's value
        /// </param>
        //void AddCriteria(DataCriteria criterio);

        /// <summary>
        /// Add row to dataset store. Each row should contain the values added with <see cref="AddToStore(XmlReader)"/> or <see cref="AddToStore(string,string)"/>. It should be called after each iteration after all <c>AddToStore</c> overloads
        /// Must be called after <see cref="BeginDataSetImport"/>  and before <see cref="Commit"/>
        /// </summary>
        void AddRow();

        /// <summary>
        /// Add the current XML attributes to sdmx dataset store. 
        /// Only XML attributes that exist in the dataset store are imported and it is determined by <see cref="ExistsColumn"/>
        /// Must be called after <see cref="BeginDataSetImport"/> and before <see cref="Commit"/>
        /// </summary>
        /// <param name="reader">
        /// The XMLReader instance to read the XML attributes from
        /// </param>
        void AddToStore(XmlReader reader);

        /// <summary>
        /// Add the specified key and value to sdmx dataset store. 
        /// The key must by a valid column else it is ignored and it is determined by <see cref="ExistsColumn"/> 
        /// Must be called after <see cref="BeginDataSetImport"/>  and before <see cref="Commit"/>
        /// </summary>
        /// <param name="key">
        /// The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        void AddToStore(string key, string value);

        /// <summary>
        /// Begin SDMX-ML DataSet import
        /// </summary>
        void BeginDataSetImport();

        /// <summary>
        /// Commit the added rows with <see cref="AddRow"/>. Must be called after all calls of <see cref="AddRow"/>
        /// </summary>
        void Commit();

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
        long Count(string key);

        /// <summary>
        /// Get count of the records
        /// </summary>
        /// <param name="all">
        /// If set to true it will ignore criteria else it will use them
        /// </param>
        /// <returns>
        /// The number of records
        /// </returns>
        int Count(bool all);

        /// <summary>
        /// Create a sorted <see cref="IDataReader"/>
        /// </summary>
        /// <param name="all">
        /// If set to true it will ignore criteria else it will use them
        /// </param>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        IDataReader CreateDataReader(bool all);

        /// <summary>
        /// Create a unsorted <see cref="IDataReader"/>. It ignores sort and criteria
        /// </summary>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        IDataReader CreateDataReaderAllUnsorted();

        /// <summary>
        /// Check if the given name exists as a column in SDMX dataset.
        /// </summary>
        /// <param name="name">
        /// The name of the key i.e. the concept ref of the component, the xml attribute name
        /// </param>
        /// <returns>
        /// True if it exists else false
        /// </returns>
        bool ExistsColumn(string name);

        /// <summary>
        /// Return all columns
        /// </summary>
        /// <returns>Return all columns</returns>
        List<string> GetAllColumns();

        /// <summary>
        /// Reset all criteria and sort
        /// </summary>
        void ResetCriteriaSort();

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
        IDictionary<T, object> SelectDistinct<T>(string key);

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
        void SetCriteria(List<DataCriteria> Criterias);

        /// <summary>
        /// Set sort order, i.e. order by, to the specified list. All entries will be added as ascenting and order of the list matters.
        /// </summary>
        /// <param name="sort">
        /// The pre-ordered list of keys to include when sorting
        /// </param>
        void SetSort(IList<string> sort);

        #endregion



       
    }
}
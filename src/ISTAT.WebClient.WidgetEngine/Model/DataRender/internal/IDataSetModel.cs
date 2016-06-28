namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Data;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;


    /// <summary>
    /// Defines the methods which must be implemented by a model.
    /// </summary>
    /// <remarks>
    /// <p>A key is actually a SDMX concept (or a key family component).</p>
    /// <p>Each key can be assigned to an axis (from a three dimensional cartesian system): X for horizontal, Y for vertical
    /// and Z for current slice. The Z axis is used for defining the current view port (or the slice), which represents the
    /// current visible subset from entire dataset.</p>
    /// </remarks>
    public interface IDataSetModel
    {
        #region Public Properties

        /// <summary>
        ///  Gets all valid keys from this model. A map between key and localized concept description
        /// </summary>
        Dictionary<string, string> AllValidKeys { get; }

        bool BlockAxisX { get; set; }
        bool BlockAxisY { get; set; }
        bool UseDefaultQuery { get; set; }

        /// <summary>
        /// Gets the first value which matches the specified filter.
        /// </summary>
        string FirstObservation { get; }

        /// <summary>
        /// Gets all keys from horizontal dimension (x axis), used for creating the header of resulted table.
        /// </summary>
        IList<string> HorizontalKeys { get; }


        IList<string> InfoKeys { get; }
        IDictionary<string, object> DatasetAttribute { get; }
        IDictionary<string, object> DimensionGroupAttribute { get; }
        IDictionary<string, object> GroupAttribute { get; }
        IDictionary<string, object> ObservationAttribute { get; }


        /// <summary>
        /// Gets the count of values for X and Y axis keys
        /// </summary>
        IDictionary<string, long> HorizontalVerticalKeyCount { get; }

        /// <summary>
        /// Gets the current key family
        /// </summary>
        IDataStructureObject KeyFamily { get; }

        /// <summary>
        /// Gets all valid values which can be used for a slice key.
        /// </summary>
        ComponentValueDictionary SliceKeyValidValues { get; }

        /// <summary>
        /// Gets all keys used to define the slice (z axis) of the data which must be rendered.
        /// </summary>
        OrderedDictionary SliceKeyValues { get; }

        //StringDictionary SliceKeyValues { get; }

        /// <summary>
        /// Gets all keys used to define the slice (z axis) of the data which must be rendered.
        /// </summary>
        ICollection SliceKeys { get; }

        /// <summary>
        /// Gets the unsorted data (with the order of the SDMX-ML data file.
        /// </summary>
        /// <value>
        ///   The IDataReader for all data
        /// </value>
        IDataReader UnsortedReader { get; }

        /// <summary>
        /// Gets all keys from vertical dimension (y axis), used for creating the first columns of resulted table.
        /// </summary>
        IList<string> VerticalKeys { get; }

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
        DisplayMode GetDisplayMode(string dimension, string currentValue);

        /// <summary>
        /// Get the IDataReader of all data or the current slice
        /// </summary>
        /// <param name="slice">
        /// If true then return the IDataReader for the current slide
        /// </param>
        /// <returns>
        /// The IDataReader of either all data or the current slice
        /// </returns>
        IDataReader GetReader(bool slice);

        /// <summary>
        /// Get the row count of all data or the current slide
        /// </summary>
        /// <param name="fromSlice">
        /// If true then return the row count for the current slide
        /// </param>
        /// <returns>
        /// The row count of either all data or the current slice
        /// </returns>
        int GetRowCount(bool fromSlice);

        /// <summary>
        /// Initialize the <see cref="IDataSetModel"/> instance
        /// </summary>
        void Initialize();

        void Initialize(List<DataCriteria> Criterias);

        /// <summary>
        /// Reload the <see cref="AllValidKeys"/> in order to get the current localized description
        /// </summary>
        void ReloadValidKeys();

        /// <summary>
        /// Reset all dimension display mode
        /// </summary>
        void ResetDisplayMode();

        /// <summary>
        /// Toggles the dimension wide display mode
        /// </summary>
        /// <param name="key">
        /// The key (dimension)
        /// </param>
        void ToggleKeyDisplayValue(string key);

        /// <summary>
        /// Toggles a specific code display mode
        /// </summary>
        /// <param name="key">
        /// The key (dimension)
        /// </param>
        /// <param name="value">
        /// The code value
        /// </param>
        void ToggleKeyDisplayValue(string key, string value);

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
        /// <remarks>
        /// After this function is called, all other model's data (like the valid values for slice keys) may change.
        /// </remarks>
        void UpdateAxis(List<string> slice, List<string> horizontal, List<string> vertical);

        void UpdateAxis(List<string> slice, List<string> horizontal, List<string> vertical, List<DataCriteria> Criterias);

        /// <summary>
        /// Updates the value for a slice key.
        /// </summary>
        /// <param name="key">
        /// the key
        /// </param>
        /// <param name="value">
        /// the new value
        /// </param>
        /// <remarks>
        /// After this function is called, all other model's data (like the valid values for slice keys) may change.
        /// </remarks>
        void UpdateSliceKeyValue(string key, string value);

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.DataRender;
using ISTAT.WebClient.WidgetComplements.Model.Enum;
using ISTAT.WebClient.WidgetComplements.Model.Exceptions;

namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    public class HtmlRenderer2: IDataSetRenderer
    {
        #region Constants and Fields

        /// <summary>
        /// The 0 value in string to use as value on html attributes
        /// </summary>
        private const string ZeroValue = "0";

        /// <summary>
        /// Map between component id and a map between code value and code description.
        /// </summary>
        private readonly ComponentCodeDescriptionDictionary _componentCodesDescriptionMap;
        public string DecimalSeparator;

        /// <summary>
        /// Controls whether enhanced output should be output. The enhanched output is related to javascript events and tooltips for displaying code and desscription. 
        /// <see cref="TableCell.SetupDisplayText(string , string , string , DisplayMode ,bool )"/>
        /// </summary>
        private readonly bool _enhancedOutput;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlRenderer"/> class. 
        /// </summary>
        /// <param name="componentCodeDescriptionMap">
        /// The map between component and the map of code and description
        /// </param>
        /// <param name="enhanced">
        /// Whether to use enhnanced output which includes java script and additional attributes
        /// </param>
        public HtmlRenderer2(ComponentCodeDescriptionDictionary componentCodeDescriptionMap, bool enhanced, string decimalSeparator = ".")
        {
            this.DecimalSeparator = decimalSeparator;
            this._componentCodesDescriptionMap = componentCodeDescriptionMap;
            this._enhancedOutput = enhanced;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets the mime type which identifies renderer's result.
        /// </summary>
        public virtual string MimeType
        {
            get
            {
                return "text/html";
            }
        }

        /// <summary>
        /// Gets the standard file extension used for the files where to save the content returned by renderer.
        /// </summary>
        public virtual string StandardFileExtension
        {
            get
            {
                return "html";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Render to the specified writer the current slide as a html table
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public virtual void Render(IDataSetModel model, TextWriter os)
        {
            var s = new RendererState(model);
            Table table = this.CreateTableModel(s);

            // Table table = this.createTableModel(model);
            table.Write(os);
        }

        /// <summary>
        /// A wrapper around <see cref="RenderAllTables(Estat.Nsi.Client.Renderer.IDataSetModel,System.IO.TextWriter)"/>
        /// </summary>
        /// <param name="model">
        /// The <see cref="IDataSetModel"/>
        /// </param>
        /// <param name="os">
        /// The output stream
        /// </param>
        public virtual void RenderAllTables(IDataSetModel model, Stream os)
        {
            var writer = new StreamWriter(os);
            this.RenderAllTables(model, writer);
            writer.Flush();
        }

        /// <summary>
        /// Render all data divided into slices to html tables
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public virtual void RenderAllTables(IDataSetModel model, TextWriter os)
        {
            this.WriteEmbeddedHtml(os, "html_start.ftl");
            this.WriteTableModels(model, os);
            this.WriteEmbeddedHtml(os, "html_finish.ftl");
        }


        #endregion

        #region Methods

        /// <summary>
        /// Check if all keys are slice keys
        /// </summary>
        /// <param name="model">
        /// The model to check
        /// </param>
        /// <returns>
        /// The all keys are slice.
        /// </returns>
        protected static bool AllKeysAreSlice(IDataSetModel model)
        {
            return model.VerticalKeys.Count == 0 && model.HorizontalKeys.Count == 0;
        }

        /// <summary>
        /// Create a <see cref="Table"/> object that will be used for slice tables
        /// </summary>
        /// <returns>
        /// A slice Table
        /// </returns>
        protected static Table CreateSliceTable()
        {
            var table = new Table();
            table.AddClass(HtmlClasses.HorizontalVerticalKeys);
            table.AddAttribute(HtmlConstants.CellPadding, ZeroValue);
            table.AddAttribute(HtmlConstants.CellSpacing, ZeroValue);
            return table;
        }

        /// <summary>
        /// Build a key for the specified dimensions with their values included at the row
        /// </summary>
        /// <param name="dims">
        /// The dimensions to include in the key
        /// </param>
        /// <param name="row">
        /// The row that contains the data
        /// </param>
        /// <returns>
        /// A key that identifies this dimension set
        /// </returns>
        protected static string MakeKey(IList<string> dims, IDataRecord row)
        {
            var sb = new StringBuilder();

            foreach (string dim in dims)
            {
                var val = row[dim] as string;
                sb.AppendFormat("{0}:{1}+", dim, val);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Build a key for the specified dimensions with their values included at the row
        /// </summary>
        /// <param name="dims">
        /// The dimensions to include in the key
        /// </param>
        /// <param name="row">
        /// The row that contains the data
        /// </param>
        /// <returns>
        /// A key that identifies this dimension set
        /// </returns>
        protected static string MakeKey(ICollection dims, IDataRecord row)
        {
            var sb = new StringBuilder();
            foreach (string dim in dims)
            {
                var val = row[dim] as string;
                sb.AppendFormat("{0}:{1}+", dim, val);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Populate the Slice table
        /// </summary>
        /// <param name="state">
        /// The State object that contains the data that will be used to populate the slice table
        /// </param>
        protected static void PopulateTable(RendererState state)
        {
            state.Table.Add(state.HorizontalOrderedRows);
            if (state.VerticalKeyTitles.Children.Count > 0 && state.VerticalOrderedRows.Count > 0)
            {
                int dataLength = state.VerticalOrderedRows[0].Children.Count - state.VerticalKeyTitles.Children.Count;
                var empty = new TableCell();
                empty.SetColSpan(dataLength);
                state.VerticalKeyTitles.AddElement(empty);
                state.Table.AddElement(state.VerticalKeyTitles);
            }

            state.VerticalOrderedRows.Sort(new VerticalKeyComparer(state.Model.VerticalKeys.Count));
            SetupVerticalRowSpan(state);
            state.Table.Add(state.VerticalOrderedRows);
        }

        /// <summary>
        /// Create a slice header table that will contain the slice key titles and values
        /// </summary>
        /// <param name="state">
        /// The current state
        /// </param>
        /// <returns>
        /// The slice header table
        /// </returns>
        protected Table CreateSliceHeaderTable(RendererState state)
        {
            var table = new Table();
            table.AddClass(HtmlClasses.SliceKeys);
            table.AddAttribute(HtmlConstants.CellPadding, ZeroValue);
            table.AddAttribute(HtmlConstants.CellSpacing, ZeroValue);
            foreach (string key in state.Model.SliceKeys)
            {
                var row = new TableRow();
                table.AddElement(row);

                var value = state.InputRow[key] as string;
                value = value ?? " ";
                var title = new TableCell(state.Model.AllValidKeys[key]);
                title.AddClass(HtmlClasses.SliceKeyTitle);
                row.AddElement(title);

                string text = this.GetLocalizedName(key, value);
                var val = new TableCell();

                // Implemented like in java to show description only
                val.SetupDisplayText(value, text, key, DisplayMode.Description, false);
                val.AddClass(HtmlClasses.SliceKeyValue);
                row.AddElement(val);
            }

            return table;
        }

        /// <summary>
        /// Initialize the horizontal and vertical key titles
        /// </summary>
        /// <param name="state">
        /// The current <see cref="RendererState"/>
        /// </param>
        protected void InitTitles(RendererState state)
        {
            state.VerticalKeyTitles = this.CreateVerticalTitles(state.Model);

            // There will be exactly one row for each horizontal dimension (for both title & possible values)...
            this.CreateHorizontalTitles(state.Model, state.HorizontalKeyRows, state.HorizontalOrderedRows);
        }

        /// <summary>
        /// Parse a data row and populate the <see cref="RendererState.CurrentTableRow"/> with the X, Y axis key values and the data
        /// </summary>
        /// <param name="state">
        /// The current state
        /// </param>
        protected virtual void ParseDataRow(RendererState state)
        {
            // check if something has changed
            state.HorizontalCurrentKeySet = MakeKey(state.Model.HorizontalKeys, state.InputRow);
            if (!string.Equals(state.HorizontalCurrentKeySet, state.HorizontalOldKeySet))
            {
                state.CurrentHorizontalKeySetColumn++;
                state.HorizontalOldKeySet = state.HorizontalCurrentKeySet;

                foreach (TableRow row in state.VerticalKeySetIndex.Values)
                {
                    row.AddElement(new TableCell(" "));
                }

                foreach (string dim in state.Model.HorizontalKeys)
                {
                    TableRow horizontalKeyRow = state.HorizontalKeyRows[dim];
                    var currentValue = state.InputRow[dim] as string;
                    this.AddHorrizontalKeyValues(
                        state.VerticalPreviousValuesMap, dim, horizontalKeyRow, currentValue, state.Model);
                }
            }

            state.VerticalCurrentKeySet = MakeKey(state.Model.VerticalKeys, state.InputRow);
            TableRow currentRow;
            if (!state.VerticalKeySetIndex.TryGetValue(state.VerticalCurrentKeySet, out currentRow))
            {
                state.CurrentTableRow = new TableRow();
                if (state.Model.VerticalKeys.Count == 0 && state.Model.HorizontalKeys.Count > 0)
                {
                    state.CurrentTableRow.AddElement(new TableCell(" "));
                }

                state.VerticalKeySetIndex.Add(state.VerticalCurrentKeySet, state.CurrentTableRow);
                state.VerticalOrderedRows.Add(state.CurrentTableRow);

                foreach (string dim in state.Model.VerticalKeys)
                {
                    var currentValue = state.InputRow[dim] as string;
                    this.AddVerticalKeyValues2(
                        state.HorizontalPreviousValuesMap, dim, state.CurrentTableRow, currentValue, state.Model);
                }

                int currentVerticalKeyValueCount = state.CurrentTableRow.Children.Count;
                state.VerticalKeyValueCount.Add(state.VerticalCurrentKeySet, currentVerticalKeyValueCount);

                state.CurrentVerticalKeyValueCount = currentVerticalKeyValueCount;

                for (int i = 0; i < state.CurrentHorizontalKeySetColumn; i++)
                {
                    state.CurrentTableRow.AddElement(new TableCell(" "));
                }
            }
            else
            {
                state.CurrentTableRow = currentRow;
                state.CurrentVerticalKeyValueCount = state.VerticalKeyValueCount[state.VerticalCurrentKeySet];
            }

            var val = state.InputRow[state.Model.KeyFamily.PrimaryMeasure.Id] as string;
            if (string.IsNullOrEmpty(val))
            {
                val = " ";
            }

            int column = state.CurrentHorizontalKeySetColumn + state.CurrentVerticalKeyValueCount;
            var measure = new TableCell(ReplaceDecimalSeparator(val));
            measure.AddClass(HtmlClasses.Measure);
            state.CurrentTableRow.AddAt(measure, column);
        }

        private string ReplaceDecimalSeparator(string currentValue)
        {
            //Double dbl;
            //Double.TryParse(currentValue,out dbl);

            //if (dbl != 0)
            if (DecimalSeparator == ",")
                return currentValue.Replace('.', ',');

            return currentValue.Replace(',', '.');

        }

        /// <summary>
        /// Read and Write the HTML from a embeded resource
        /// </summary>
        /// <param name="writer">
        /// The output to write to
        /// </param>
        /// <param name="fileName">
        /// The name of the embdeded resource. Usually it is the name of the filename
        /// </param>
        protected void WriteEmbeddedHtml(TextWriter writer, string fileName)
        {
            string resourceName = string.Format(
                CultureInfo.InvariantCulture, "{0}.{1}", this.GetType().Namespace, fileName);
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (resourceStream != null)
            {
                using (var reader = new StreamReader(resourceStream))
                {
                    var buffer = new char[4096];

                    while (!reader.EndOfStream)
                    {
                        int len = reader.Read(buffer, 0, 4096);
                        writer.Write(buffer, 0, len);
                    }
                }
            }
        }

        /// <summary>
        /// Write all data splitted in slices to the specified writer
        /// </summary>
        /// <param name="model">
        /// The model to write
        /// </param>
        /// <param name="writer">
        /// The writer to use
        /// </param>
        protected virtual void WriteTableModels(IDataSetModel model, TextWriter writer)
        {
            IDataReader allData = model.GetReader(false);
            Table containerTable = CreateContainerTable();
            string oldKeySet = null;
            var s = new RendererState(model);

            while (allData.Read())
            {
                s.InputRow = allData;
                string currentKeySet = MakeKey(model.SliceKeys, allData);

                if (!currentKeySet.Equals(oldKeySet))
                {
                    oldKeySet = currentKeySet;
                    if (s.Table != null)
                    {
                        PopulateTable(s);
                        containerTable.Write(writer);
                        s.Reset();
                    }

                    containerTable = CreateContainerTable();
                    Table sliceHeaderTable = this.CreateSliceHeaderTable(s);
                    AddSliceHeading(containerTable, sliceHeaderTable);
                    s.Table = CreateSliceTable();
                    AddSliceTable(containerTable, s.Table);

                    this.InitTitles(s);
                }

                this.ParseDataRow(s);
            }

            allData.Close();
            s.InputRow = null;
            if (s.Table != null)
            {
                PopulateTable(s);
                containerTable.Write(writer);
                s.Reset();
            }
        }

        /// <summary>
        /// Add specified Slice heading table to container table
        /// </summary>
        /// <param name="containerTable">
        /// The container table
        /// </param>
        /// <param name="sliceKeysTable">
        /// The slice header table
        /// </param>
        private static void AddSliceHeading(Table containerTable, Table sliceKeysTable)
        {
            containerTable.AddTable(0, 0, sliceKeysTable);
        }

        /// <summary>
        /// Add <paramref name="horizontalVerticalKeys"/> to <paramref name="containerTable"/>
        /// </summary>
        /// <param name="containerTable">
        /// The container <see cref="Table"/>
        /// </param>
        /// <param name="horizontalVerticalKeys">
        /// The slice <see cref="Table"/>
        /// </param>
        private static void AddSliceTable(Table containerTable, Table horizontalVerticalKeys)
        {
            containerTable.AddTable(1, 0, horizontalVerticalKeys);
        }

        /// <summary>
        /// Create the container table. The container table holds the slice header and slice data tables.
        /// </summary>
        /// <returns>
        /// The table that holds the slice header and data
        /// </returns>
        private static Table CreateContainerTable()
        {
            var containerTable = new Table();
            containerTable.AddClass(HtmlClasses.Container);
            containerTable.AddAttribute(HtmlConstants.CellPadding, ZeroValue);
            containerTable.AddAttribute(HtmlConstants.CellSpacing, ZeroValue);
            var sliceRow = new TableRow();
            containerTable.AddElement(sliceRow);
            var skeys = new TableCell();
            skeys.AddClass(HtmlClasses.SliceKeys);
            sliceRow.AddElement(skeys);

            var hvkeys = new TableRow(new TableCell());
            hvkeys.AddClass(HtmlClasses.HorizontalVerticalKeys);
            containerTable.AddElement(hvkeys);

            return containerTable;
        }

        /// <summary>
        /// Populate a single value table. This is used when all keys are slice keys.
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="table">
        /// The html table to populate
        /// </param>
        private static void GetSingleValue(IDataSetModel model, HtmlEntityParent<TableRow> table)
        {
            if (model.GetRowCount(true) > 1)
            {
                throw new DataSetRendererException(
                    "There are no horizontal & vertical keys, but the number of " + "values from model is "
                    + model.GetRowCount(true) + " instead of 1");
            }

            table.AddElement(
                model.GetRowCount(true) == 0
                    ? new TableRow(new TableCell(" "))
                    : new TableRow(new TableCell(model.FirstObservation)));
        }

        /// <summary>
        /// Set vertical row span in <see cref="RendererState.VerticalOrderedRows"/>.
        /// </summary>
        /// <param name="s">
        /// The <see cref="RendererState"/> containing the vertical ordered rows
        /// </param>
        private static void SetupVerticalRowSpan(RendererState s)
        {
            int keyCount = s.Model.VerticalKeys.Count - 1;
            if (keyCount < 1)
            {
                // we don't care about colspan if there is only one or less vertical keys
                return;
            }

            // The TD per TR that will be deleted
            var toDeleteTableCells = new Dictionary<TableRow, List<TableCell>>();

            // the previous row values
            var previousValues = new string[keyCount];
            var firstTableCell = new TableCell[keyCount];
            for (int v = 0; v < s.VerticalOrderedRows.Count; v++)
            {
                // for each TR row
                var row = s.VerticalOrderedRows[v];
                for (int i = 0; i < keyCount; i++)
                {
                    // for each vertical key column
                    var cell = row.Children[i];
                    if (!string.Equals(cell.Text, previousValues[i]))
                    {
                        // if the current key value doesn't equal the previous row value
                        previousValues[i] = cell.Text; // set the previous value to the current
                        for (int x = i + 1; x < keyCount; x++)
                        {
                            // reset any previous values from keys that follow the current
                            previousValues[x] = null;
                        }

                        firstTableCell[i] = cell;
                    }
                    else
                    {
                        firstTableCell[i].RowSpan++;
                        List<TableCell> deleteList;
                        if (!toDeleteTableCells.TryGetValue(row, out deleteList))
                        {
                            deleteList = new List<TableCell>();
                            toDeleteTableCells.Add(row, deleteList);
                        }

                        deleteList.Add(cell);
                    }
                }
            }

            foreach (KeyValuePair<TableRow, List<TableCell>> entry in toDeleteTableCells)
            {
                foreach (TableCell cell in entry.Value)
                {
                    entry.Key.Children.Remove(cell);
                }
            }
        }

        /// <summary>
        /// Add horrizontal key values to the specified output Row
        /// </summary>
        /// <param name="horizontalPreviousValues">
        /// A map of {key,  previous value of key} 
        /// </param>
        /// <param name="dimension">
        /// The current key
        /// </param>
        /// <param name="outputRow">
        /// The row to add the key values
        /// </param>
        /// <param name="currentValue">
        /// The current value
        /// </param>
        /// <param name="model">
        /// The dataset model
        /// </param>
        private void AddHorrizontalKeyValues(
            IDictionary<string, TableCell> horizontalPreviousValues, 
            string dimension, 
            TableRow outputRow, 
            string currentValue, 
            IDataSetModel model)
        {
            TableCell oldValue;
            if (horizontalPreviousValues.TryGetValue(dimension, out oldValue) && oldValue != null
                && string.Equals(currentValue, oldValue.SdmxValue))
            {
                oldValue.ColumnSpan++;
            }
            else
            {
                var tableCell = new TableCell();
                string text = this.GetLocalizedName(dimension, currentValue);
                tableCell.SetupDisplayText(
                    currentValue, text, dimension, model.GetDisplayMode(dimension, currentValue), this._enhancedOutput);
                tableCell.AddClass(HtmlClasses.HorizontalKeyValue);
                outputRow.AddCell(tableCell);
                horizontalPreviousValues[dimension] = tableCell;
            }
        }

        /// <summary>
        /// Add vertical key values to the specified output Row
        /// </summary>
        /// <param name="verticalPreviousValues">
        /// A map of {key,  previous value of key} 
        /// </param>
        /// <param name="dimension">
        /// The current key
        /// </param>
        /// <param name="outputRow">
        /// The row to add the key values
        /// </param>
        /// <param name="currentValue">
        /// The current value
        /// </param>
        /// <param name="model">
        /// The dataset model
        /// </param>
        private void AddVerticalKeyValues2(
            IDictionary<string, TableCell> verticalPreviousValues, 
            string dimension, 
            TableRow outputRow, 
            string currentValue, 
            IDataSetModel model)
        {
            var tableCell = new TableCell { Text = currentValue };
            tableCell.AddClass(HtmlClasses.VerticalKeyValue);
            string text = this.GetLocalizedName(dimension, currentValue);
            tableCell.SetupDisplayText(
                currentValue, text, dimension, model.GetDisplayMode(dimension, currentValue), this._enhancedOutput);
            outputRow.AddCell(tableCell);
            verticalPreviousValues[dimension] = tableCell;
        }

        /// <summary>
        /// Create and setup the titles for horizontal dimensions
        /// </summary>
        /// <param name="model">
        /// The DataSetModel to use
        /// </param>
        /// <param name="horizontalRows">
        /// The horizontal map to add the titles to
        /// </param>
        /// <param name="horizontalOrderedRows">
        /// The list of rows
        /// </param>
        private void CreateHorizontalTitles(
            IDataSetModel model, 
            IDictionary<string, TableRow> horizontalRows, 
            ICollection<TableRow> horizontalOrderedRows)
        {
            foreach (string dim in model.HorizontalKeys)
            {
                var row = new TableRow();
                row.AddClass(HtmlClasses.HorizontalKeys);

                // First cell is the title, which spawns over all columns used for vertical keys...
                TableCell tableCell = this.CreateTitle(model.AllValidKeys[dim], dim, "hkeytitle");
                tableCell.SetColSpan(Math.Max(1, model.VerticalKeys.Count));
                row.AddCell(tableCell);
                horizontalRows.Add(dim, row);
                horizontalOrderedRows.Add(row);
            }
        }

        /// <summary>
        /// Create a single slice table for the specified Layout.
        /// </summary>
        /// <param name="state">
        /// The current state
        /// </param>
        /// <returns>
        /// The slice <see cref="Table"/> 
        /// </returns>
        private Table CreateTableModel(RendererState state)
        {
            state.Table = CreateSliceTable();

            if (AllKeysAreSlice(state.Model))
            {
                GetSingleValue(state.Model, state.Table);
                return state.Table;
            }

            this.InitTitles(state);
            IDataReader reader = state.Model.GetReader(true);
            while (reader.Read())
            {
                state.InputRow = reader;
                this.ParseDataRow(state);
            }

            reader.Close();
            PopulateTable(state);
            return state.Table;
        }

        /// <summary>
        /// Create a dimension (i.e. key) title
        /// </summary>
        /// <param name="title">
        /// The title
        /// </param>
        /// <param name="dim">
        /// The SDMX dimension id
        /// </param>
        /// <param name="clazz">
        /// the HTML class of the title
        /// </param>
        /// <returns>
        /// The <see cref="TableCell"/> with the <paramref name="title"/> as text
        /// </returns>
        private TableCell CreateTitle(string title, string dim, string clazz)
        {
            var tableCell = new TableCell(title);
            tableCell.AddClass(clazz);
            if (this._componentCodesDescriptionMap.ContainsKey(dim) && this._enhancedOutput)
            {
                tableCell.SetupDisplayText(dim);
            }

            return tableCell;
        }

        /// <summary>
        /// Create and setup the titles for vertical dimensions
        /// </summary>
        /// <param name="model">
        /// The DataSetModel to use
        /// </param>
        /// <returns>
        /// The row with the titles
        /// </returns>
        private TableRow CreateVerticalTitles(IDataSetModel model)
        {
            var verticalTitles = new TableRow();
            verticalTitles.AddClass(HtmlClasses.VerticalKeys);
            foreach (string dim in model.VerticalKeys)
            {
                TableCell tableCell = this.CreateTitle(model.AllValidKeys[dim], dim, HtmlClasses.VerticalKeyTitle);

                // new TD();
                verticalTitles.AddElement(tableCell);
            }

            return verticalTitles;
        }

        /// <summary>
        /// Get localized description for the specified code value of the specified dimension codelist
        /// </summary>
        /// <param name="dimension">
        /// The dimension
        /// </param>
        /// <param name="code">
        /// The code value
        /// </param>
        /// <returns>
        /// A localized description
        /// </returns>
        private string GetLocalizedName(string dimension, string code)
        {
            string ret = null;
            Dictionary<string, string> codeDescMap;
            if (this._componentCodesDescriptionMap.TryGetValue(dimension, out codeDescMap))
            {
                codeDescMap.TryGetValue(code, out ret);
            }

            return ret;
        }

        #endregion

        /// <summary>
        /// This class holds the stats of the Html Renderer.
        /// </summary>
        protected class RendererState
        {
            // TODO Clean this mess
            #region Constants and Fields

            /// <summary>
            /// a map between the horizontal key and the row that contains title and key values
            /// </summary>
            private readonly Dictionary<string, TableRow> _horizontalKeyRows;

            /// <summary>
            /// Gets or sets the list of rows containing the horizontal key and key values. 
            /// </summary>
            private readonly HtmlEntityCollection<TableRow> _horizontalOrderedRows =
                new HtmlEntityCollection<TableRow>();

            /// <summary>
            /// A map betwen a horizontal key and it's previous value. Used for colspan
            /// </summary>
            private readonly IDictionary<string, TableCell> _horizontalPreviousValuesMap =
                new Dictionary<string, TableCell>();

            /// <summary>
            /// Map between vertical key value-Set to their TR 
            /// </summary>
            private readonly IDictionary<string, TableRow> _verticalKeySetIndex = new Dictionary<string, TableRow>();

            /// <summary>
            /// A map between the vertical keySet and the true number of TDs used by key values for that row.
            /// </summary>
            private readonly IDictionary<string, int> _verticalKeyValueCount = new Dictionary<string, int>();

            /// <summary>
            /// A list of rows with vertical key values (if any) and data
            /// </summary>
            private readonly HtmlEntityCollection<TableRow> _verticalOrderedRows = new HtmlEntityCollection<TableRow>();

            /// <summary>
            /// A map betwen a vertical key and it's previous value. Used for rowspan
            /// </summary>
            private readonly IDictionary<string, TableCell> _verticalPreviousValuesMap =
                new Dictionary<string, TableCell>();

            /// <summary>
            /// The current data/slice row
            /// </summary>
            private IDataReader _inputRow;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RendererState"/> class. 
            /// </summary>
            /// <param name="model">
            /// The current <see cref="IDataSetModel"/>
            /// </param>
            public RendererState(IDataSetModel model)
            {
                this.Model = model;
                this.CurrentHorizontalKeySetColumn = -1;
                this._horizontalKeyRows = new Dictionary<string, TableRow>(StringComparer.Ordinal);
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the current position of the current column 
            /// </summary>
            public int CurrentHorizontalKeySetColumn { get; set; }

            /// <summary>
            /// Gets or sets the current row
            /// </summary>
            public TableRow CurrentTableRow { get; set; }

            /// <summary>
            /// Gets or sets the current vertical key value count
            /// </summary>
            public int CurrentVerticalKeyValueCount { get; set; }

            /// <summary>
            /// Gets or sets the current horizontal key set (the values of all horizontal dimensions)
            /// </summary>
            public string HorizontalCurrentKeySet { get; set; }

            /// <summary>
            /// Gets a map between the horizontal key and the row that contains title and key values
            /// </summary>
            public Dictionary<string, TableRow> HorizontalKeyRows
            {
                get
                {
                    return this._horizontalKeyRows;
                }
            }

            /// <summary>
            /// Gets or sets the previous horizontal key set (the values of all horizontal dimensions)
            /// </summary>
            public string HorizontalOldKeySet { get; set; }

            /// <summary>
            /// Gets the list of rows containing the horizontal key and key values. 
            /// </summary>
            public HtmlEntityCollection<TableRow> HorizontalOrderedRows
            {
                get
                {
                    return this._horizontalOrderedRows;
                }
            }

            /// <summary>
            /// Gets the map betwen a horizontal key and it's previous value. Used for colspan
            /// </summary>
            public IDictionary<string, TableCell> HorizontalPreviousValuesMap
            {
                get
                {
                    return this._horizontalPreviousValuesMap;
                }
            }

            /// <summary>
            /// Gets or sets the current data/slice row
            /// </summary>
            public IDataReader InputRow
            {
                get
                {
                    return this._inputRow;
                }

                set
                {
                    this._inputRow = value;
                }
            }

            /// <summary>
            /// Gets or sets the current model
            /// </summary>
            public IDataSetModel Model { get; set; }

            /// <summary>
            /// Gets or sets the current table
            /// </summary>
            public Table Table { get; set; }

            /// <summary>
            /// Gets or sets the current vertical key set (the values of all vertical dimensions)
            /// </summary>
            public string VerticalCurrentKeySet { get; set; }

            /// <summary>
            /// Gets the map between vertical key value-Set to their TR 
            /// </summary>
            public IDictionary<string, TableRow> VerticalKeySetIndex
            {
                get
                {
                    return this._verticalKeySetIndex;
                }
            }

            /// <summary>
            /// Gets or sets the row containing the vertical titles
            /// </summary>
            public TableRow VerticalKeyTitles { get; set; }

            /// <summary>
            /// Gets the map between the vertical keySet and the true number of TDs used by key values for that row.
            /// </summary>
            public IDictionary<string, int> VerticalKeyValueCount
            {
                get
                {
                    return this._verticalKeyValueCount;
                }
            }

            /// <summary>
            /// Gets the list of rows with vertical key values (if any) and data
            /// </summary>
            public HtmlEntityCollection<TableRow> VerticalOrderedRows
            {
                get
                {
                    return this._verticalOrderedRows;
                }
            }

            /// <summary>
            /// Gets the map betwen a vertical key and it's previous value. Used for rowspan
            /// </summary>
            public IDictionary<string, TableCell> VerticalPreviousValuesMap
            {
                get
                {
                    return this._verticalPreviousValuesMap;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Reset all fields
            /// </summary>
            public void Reset()
            {
                this._verticalKeySetIndex.Clear();
                this._verticalOrderedRows.Clear();
                this.HorizontalKeyRows.Clear();
                this.HorizontalOldKeySet = null;
                this.VerticalCurrentKeySet = null;
                this.HorizontalCurrentKeySet = null;
                this._horizontalPreviousValuesMap.Clear();
                this._verticalPreviousValuesMap.Clear();

                // inputRow = null;
                this.Table = null;
                this.CurrentTableRow = null;
                this.VerticalKeyTitles = null;
                this._horizontalOrderedRows.Clear();
                this._verticalKeyValueCount.Clear();
                this.CurrentHorizontalKeySetColumn = -1;
                this.CurrentVerticalKeyValueCount = 0;
            }

            #endregion
        }

    }
}

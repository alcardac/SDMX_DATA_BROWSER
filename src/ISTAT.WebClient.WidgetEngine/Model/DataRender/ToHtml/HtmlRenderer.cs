namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    using ISTAT.WebClient.WidgetComplements.Model.DataRender;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Render a IDataSetModel object to html.
    /// </summary>
    public class HtmlRenderer : IDataSetRenderer
    {
        private int _uniqID;

        #region Constants and Fields

        /// <summary>
        /// The 0 value in string to use as value on html attributes
        /// </summary>
        private const string ZeroValue = "0";

        /// <summary>
        /// Map between component id and a map between code value and code description.
        /// </summary>
        private readonly ComponentCodeDescriptionDictionary _componentCodesDescriptionMap;

        /// <summary>
        /// Controls whether enhanced output should be output. The enhanched output is related to javascript events and tooltips for displaying code and desscription. 
        /// <see cref="TableCell.SetupDisplayText(string , string , string , DisplayMode ,bool )"/>
        /// </summary>
        private readonly bool _enhancedOutput;

        private CultureInfo cFrom;
        private CultureInfo cTo;

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
        public HtmlRenderer(
            ComponentCodeDescriptionDictionary componentCodeDescriptionMap,
            bool enhanced,
            bool useAttr, CultureInfo cFrom, CultureInfo cTo)
        {
            this._componentCodesDescriptionMap = componentCodeDescriptionMap;
            this._enhancedOutput = enhanced;
            this._uniqID = 0;
            _useSdmxAttr = useAttr;

            this.cFrom = cFrom;
            this.cTo = cTo;
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

        private bool _useSdmxAttr = false;
        public bool useSdmxAttr { get { return _useSdmxAttr; } set { value = useSdmxAttr; } }

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
        protected virtual void ParseDataRow(RendererState state, object vc = null, object vt = null)
        {
            #region check if something has changed
            state.HorizontalCurrentKeySet = MakeKey(state.Model.HorizontalKeys, state.InputRow);
            if (!string.Equals(state.HorizontalCurrentKeySet, state.HorizontalOldKeySet))
            {
                state.CurrentHorizontalKeySetColumn++;
                state.HorizontalOldKeySet = state.HorizontalCurrentKeySet;

                foreach (TableRow row in state.VerticalKeySetIndex.Values)
                {


                    TableCell emptyMeasure = new TableCell("-");
                    emptyMeasure.AddClass(HtmlClasses.NotMeasure);
                    row.AddElement(emptyMeasure);
                }

                foreach (string dim in state.Model.HorizontalKeys)
                {
                    TableRow horizontalKeyRow = state.HorizontalKeyRows[dim];
                    var currentValue = state.InputRow[dim] as string;

                    this.AddHorrizontalKeyValues(
                        state.VerticalPreviousValuesMap,
                        dim,
                        horizontalKeyRow,
                        currentValue,
                        state.Model,
                        state.InputRow);

                }
            }
            #endregion

            TableRow currentRow;

            state.VerticalCurrentKeySet = MakeKey(state.Model.VerticalKeys, state.InputRow);
            if (!state.VerticalKeySetIndex.TryGetValue(state.VerticalCurrentKeySet, out currentRow))
            {
                state.CurrentTableRow = new TableRow();
                if (state.Model.VerticalKeys.Count == 0 && state.Model.HorizontalKeys.Count > 0)
                {
                    TableCell emptyMeasure = new TableCell("-");
                    emptyMeasure.AddClass(HtmlClasses.NotMeasure);
                    state.CurrentTableRow.AddElement(emptyMeasure);
                }

                state.VerticalKeySetIndex.Add(state.VerticalCurrentKeySet, state.CurrentTableRow);
                state.VerticalOrderedRows.Add(state.CurrentTableRow);

                foreach (string dim in state.Model.VerticalKeys)
                {
                    var currentValue = state.InputRow[dim] as string;
                    this.AddVerticalKeyValues(
                        state.HorizontalPreviousValuesMap,
                        dim,
                        state.CurrentTableRow,
                        currentValue,
                        state.Model,
                        state.InputRow);
                }

                int currentVerticalKeyValueCount = state.CurrentTableRow.Children.Count;
                state.VerticalKeyValueCount.Add(state.VerticalCurrentKeySet, currentVerticalKeyValueCount);

                state.CurrentVerticalKeyValueCount = currentVerticalKeyValueCount;

                for (int i = 0; i < state.CurrentHorizontalKeySetColumn; i++)
                {
                    TableCell emptyMeasure = new TableCell("-");
                    emptyMeasure.AddClass(HtmlClasses.NotMeasure);
                    state.CurrentTableRow.AddElement(emptyMeasure);
                }
            }
            else
            {
                state.CurrentTableRow = currentRow;
                state.CurrentVerticalKeyValueCount = state.VerticalKeyValueCount[state.VerticalCurrentKeySet];
            }

            //var time = state.InputRow[state.Model.KeyFamily.TimeDimension.Id] as string;

            var val = state.InputRow[state.Model.KeyFamily.PrimaryMeasure.Id] as string;
            if (string.IsNullOrEmpty(val) 
                || val == "NaN")
            {
                val = string.Empty;
            }
            else {
                decimal decVal = 0;
                if (decimal.TryParse(val.ToString(), NumberStyles.AllowDecimalPoint, cFrom, out decVal))
                {
                    val = decVal.ToString("F1", cTo);
                }
            }


            TableCell data_cell;
            if (string.IsNullOrEmpty(val))
            {
                data_cell = new TableCell("-");
                data_cell.AddClass(HtmlClasses.NotMeasure);

            }else{

                this._uniqID++;
                Table tb = new Table();
                TableRow _rw = new TableRow();
                // Add table info extra at level observation                 
                Table tb_extra =
                    (this._useSdmxAttr) ?
                    (!string.IsNullOrEmpty(val)) ?
                        CreateObservationAttribute(state.Model, state.InputRow) : null : null;
                if (tb_extra != null)
                {
                    tb_extra.AddAttribute("ID", this._uniqID + "_info_extra_dialog");
                    tb_extra.AddClass(HtmlClasses.ExtraInfoTable);

                    tb.AddRow(new TableRow(new TableCell(tb_extra)));

                    var tb_btn_extra = new TableCell();
                    tb_btn_extra.AddClass(HtmlClasses.ExtraInfoWrapper);
                    tb_btn_extra.AddAttribute("ID", this._uniqID + "_info");

                    _rw.AddCell(tb_btn_extra);
                }
                /*
                decimal decVal_vc = 0;
                if (vc != null && decimal.TryParse(vc.ToString(), NumberStyles.AllowDecimalPoint, cFrom, out decVal_vc))
                {
                    vc = decVal_vc.ToString("F1", cTo);
                }
                decimal decVal_vt = 0;
                if (vt != null && decimal.TryParse(vt.ToString(), NumberStyles.AllowDecimalPoint, cFrom, out decVal_vt))
                {
                    vt = decVal_vt.ToString("F1", cTo);
                }
                */
                TableCell measure = new TableCell((string.IsNullOrEmpty(val)) ? "-" : val);
                measure.AddClass((string.IsNullOrEmpty(val)) ? HtmlClasses.NotMeasure : HtmlClasses.Measure);
                measure.AddAttribute("data-v", (string.IsNullOrEmpty(val)) ? "-" : val);
                measure.AddAttribute("data-vc", (vc == null) ? "?" : vc.ToString());
                measure.AddAttribute("data-vt", (vt == null) ? "?" : vt.ToString());

                _rw.AddCell(measure);

                tb.AddRow(_rw);


                data_cell = new TableCell(tb);
                data_cell.AddClass(HtmlClasses.ExtraInfoTableValue);

            }

            int column = state.CurrentHorizontalKeySetColumn + state.CurrentVerticalKeyValueCount;
            state.CurrentTableRow.AddAt(data_cell, column);
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
                throw new Exception(
                    "There are no horizontal & vertical keys, but the number of " + "values from model is "
                    + model.GetRowCount(true) + " instead of 1");
            }

            table.AddElement(
                model.GetRowCount(true) == 0
                    ? new TableRow(new TableCell(" "))
                    : new TableRow(new TableCell(float.Parse(model.FirstObservation.ToString().Replace('.', ',')).ToString())));
        }

        /// <summary>
        /// Set vertical row span in <see cref="RendererState.VerticalOrderedRows"/>.
        /// </summary>
        /// <param name="s">
        /// The <see cref="RendererState"/> containing the vertical ordered rows
        /// </param>
        private static void SetupVerticalRowSpan(RendererState s)
        {

            try
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
                        TableCell cell = row.Children[i];

                        string cell_value = string.Empty;

                        cell_value = HtmlRenderer.GetSdmxValue(cell);

                        if (cell != null && !string.Equals(cell_value, previousValues[i]))
                        {
                            // if the current key value doesn't equal the previous row value
                            previousValues[i] = cell_value; // set the previous value to the current
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
            catch(Exception ex) { 
            
            }
        }

        private static string GetSdmxValue(Table cell)
        {
            if (cell.Children.Count > 0) 
                return GetSdmxValue(cell.Children[0]);
            return null;
        }
        private static string GetSdmxValue(TableRow cell)
        {
            if (cell.Children.Count > 0) return GetSdmxValue(cell.Children[0]);
            return null;
        }
        private static string GetSdmxValue(TableCell cell)
        {
            if (cell.Children.Count > 0) return GetSdmxValue(cell.Children[0]);
            return cell.SdmxValue;
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
            IDataSetModel model,
            IDataReader currentRow)
        {
            TableCell oldCell;
            horizontalPreviousValues.TryGetValue(dimension, out oldCell);

            string oldValue = string.Empty;

            if (oldCell != null &&
                oldCell.Children.Count > 0 &&
                oldCell.Children[0].Children.Count > 0 &&
                oldCell.Children[0].Children[0].Children.Count > 1)
                oldValue = oldCell.Children[0].Children[0].Children[0].SdmxValue;
            else if (oldCell != null)
                oldValue = oldCell.SdmxValue;

            if (oldCell != null && string.Equals(currentValue, oldValue))
            {
                oldCell.ColumnSpan++;
            }
            else
            {

                this._uniqID++;
                Table tb = new Table();
                TableRow _rw = new TableRow();
                Table tb_extra = (this._useSdmxAttr) ? CreateDimensionAttributeTable(dimension, model, currentRow) : null;
                if (tb_extra != null)
                {
                    tb_extra.AddAttribute("ID", this._uniqID + "_dim_info_extra_dialog");
                    tb_extra.AddClass(HtmlClasses.ExtraInfoTable);

                    tb.AddRow(new TableRow(new TableCell(tb_extra)));

                    var tb_btn_extra = new TableCell();
                    tb_btn_extra.AddClass(HtmlClasses.ExtraInfoWrapper);
                    tb_btn_extra.AddAttribute("ID", this._uniqID + "_dim_info");

                    _rw.AddCell(tb_btn_extra);
                }

                string text = this.GetLocalizedName(dimension, currentValue);
                var tb_dim = new TableCell();
                tb_dim.AddClass(HtmlClasses.HorizontalKeyValue);
                tb_dim.SetupDisplayText(currentValue, text, dimension, model.GetDisplayMode(dimension, currentValue), this._enhancedOutput);

                if (dimension == "TIME_PERIOD") tb_dim.AddAttribute("style", "white-space:nowrap");

                _rw.AddCell(tb_dim);

                tb.AddRow(_rw);
                TableCell tb_cell = new TableCell(tb);
                tb_cell.SdmxValue = currentValue;
                outputRow.AddCell(tb_cell);
                horizontalPreviousValues[dimension] = tb_cell;

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
        private void AddVerticalKeyValues(
            IDictionary<string, TableCell> verticalPreviousValues,
            string dimension,
            TableRow outputRow,
            string currentValue,
            IDataSetModel model,
            IDataReader currentRow)
        {
            this._uniqID++;
            Table tb = new Table();
            TableRow _rw = new TableRow();
            Table tb_extra = (this._useSdmxAttr) ? CreateDimensionAttributeTable(dimension, model, currentRow) : null;
            if (tb_extra != null)
            {
                tb_extra.AddAttribute("ID", this._uniqID + "_dim_info_extra_dialog");
                tb_extra.AddClass(HtmlClasses.ExtraInfoTable);

                tb.AddRow(new TableRow(new TableCell(tb_extra)));

                var tb_btn_extra = new TableCell();
                tb_btn_extra.AddClass(HtmlClasses.ExtraInfoWrapper);
                tb_btn_extra.AddAttribute("ID", this._uniqID + "_dim_info");

                _rw.AddCell(tb_btn_extra);
            }

            string text = this.GetLocalizedName(dimension, currentValue);
            var tb_dim = new TableCell();
            tb_dim.AddClass(HtmlClasses.VerticalKeyValue);

            if (dimension == "TIME_PERIOD") tb_dim.AddAttribute("style", "white-space:nowrap");

            tb_dim.SetupDisplayText(currentValue, text, dimension, model.GetDisplayMode(dimension, currentValue), this._enhancedOutput);

            _rw.AddCell(tb_dim);
            tb.AddRow(_rw);
            TableCell tb_cell = new TableCell(tb);

            outputRow.AddCell(tb_cell);
            verticalPreviousValues[dimension] = tb_cell;

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

            var v = new Dictionary<string, decimal>();

            reader = state.Model.GetReader(true);
            while (reader.Read())
            {
                state.InputRow = reader;

                #region Calcolo variazioni

                decimal obs = 0;
                object vt = null;
                object vc = null;

                var obs_val = reader.GetValue(reader.GetOrdinal("OBS_VALUE"));
                var time_p = reader.GetValue(reader.GetOrdinal("TIME_PERIOD"));

                bool is_obs_value = false;
                try
                {
                    obs = Convert.ToDecimal(obs_val.ToString(), cFrom);
                    is_obs_value = true;
                }
                catch {
                    is_obs_value = false;
                }
                if (is_obs_value)
                {
                        
                    int anno = 0;
                    int period = 0;

                    bool _errTimePeriod = false;

                    bool _annual = !((string)time_p).Contains("-");
                    bool _quater = false;
                    bool _seme = false;

                    #region ESTRAGGO ANNO E PERIOD
                    if (_annual)
                    {
                        _errTimePeriod = !(int.TryParse(((string)time_p), out anno));
                    }
                    else
                    {
                        _errTimePeriod = !(int.TryParse(((string)time_p).Split('-')[0], out anno));

                        string _p = ((string)time_p).Split('-')[1];

                        if (_quater = _p.StartsWith("Q")) _p = _p.Substring(1);
                        if (_seme = _p.StartsWith("S")) _p = _p.Substring(1);

                        _errTimePeriod = !(int.TryParse(_p, out period));
                    }
                    #endregion

                    if (!_errTimePeriod)
                    {
                        string serieKeyStr = string.Empty;
                        foreach (var dim in state.Model.KeyFamily.DimensionList.Dimensions)
                        {
                            serieKeyStr += (string)((serieKeyStr != string.Empty) ? "+" : string.Empty);
                            serieKeyStr += (dim.Id != state.Model.KeyFamily.TimeDimension.Id) ? reader[dim.Id] : string.Empty;
                        }

                        string vi_k = string.Empty;
                        string vf_k = string.Empty;

                        // Calcolo variazione congiunturale

                        vf_k = serieKeyStr + anno + "_" + (period);
                        if (!_annual)
                            if (period == 1)
                                if (_seme)
                                    vi_k = serieKeyStr + (anno - 1) + "_2";
                                else if (_quater)
                                    vi_k = serieKeyStr + (anno - 1) + "_4";
                                else
                                    vi_k = serieKeyStr + (anno - 1) + "_12";
                            else
                                vi_k = serieKeyStr + anno + "_" + (period - 1);
                        else vi_k = serieKeyStr + (anno - 1) + "_" + (period);

                        var vi = (v.ContainsKey(vi_k.ToString())) ? (object)v[vi_k] : null;

                        try
                        {
                            decimal _vi = Convert.ToDecimal(vi, cFrom);
                            if (_vi == 0) vc = null;
                            else vc = Math.Round((((obs - _vi) / _vi) * 100), 1);
                        }
                        catch
                        {
                            vc = null;
                        }


                        // Calcolo variazione tendenziale
                        vi_k = serieKeyStr + (anno - 1) + "_" + (period);
                        vf_k = serieKeyStr + anno + "_" + (period);
                        vi = (v.ContainsKey(vi_k.ToString())) ? (object)v[vi_k] : null;

                        try
                        {
                            decimal _vi = Convert.ToDecimal(vi, cFrom);
                            if (_vi == 0) vt = null;
                            else vt = Math.Round((((obs - _vi) / _vi) * 100), 1);
                        }
                        catch { vt = null; }

                        v.Add(vf_k, obs);
                    }
                }
                #endregion

                this.ParseDataRow(state, vc, vt);

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

        private Table CreateObservationAttribute(IDataSetModel model, IDataReader currentRow)
        {

            Table tb_extra = new Table();
            /*
            TableCell cell_title = new TableCell("Observation Attribute");
            cell_title.AddClass(HtmlClasses.ExtraInfoTableLabelTitle);
            cell_title.ColumnSpan = 2;
            tb_extra.AddRow(new TableRow(cell_title));
            */
            bool hasReference = false;

            foreach (var attr in model.KeyFamily.ObservationAttributes)
            {
                //var attr_ = model.KeyFamily.GetObservationAttribute(attr);
                var code = currentRow[attr.Id].ToString().ToLower();
                if (!string.IsNullOrEmpty(code))
                {

                    string codesDescription = string.Empty;
                    if (this._componentCodesDescriptionMap.ContainsKey(attr.Id))
                    {
                        this._componentCodesDescriptionMap[attr.Id].TryGetValue(code.ToUpper(), out codesDescription);
                        if (string.IsNullOrEmpty(codesDescription))
                            this._componentCodesDescriptionMap[attr.Id].TryGetValue(code.ToLower(), out codesDescription);
                    }

                    TableRow tr = CreateAttributeRow(
                        model.AllValidKeys[attr.Id],
                        code,
                        codesDescription, true);

                    tb_extra.AddRow(tr);
                    hasReference = true;
                }
            }
            return (hasReference) ? tb_extra : null;
        }
        private Table CreateDimensionAttributeTable(string dimension, IDataSetModel model, IDataReader currentRow)
        {
            // table container
            Table tb_extra = new Table();

            // Get Concept name of dimension group attribute
            bool hasReference = false;

            #region GroupAttributes
            foreach (var attr in model.KeyFamily.GroupAttributes)
            {
                IList<string> dim_ref = model.KeyFamily.GetGroup(attr.AttachmentGroup).DimensionRefs;
                // if current dimensions has reference of attr get the current value
                if (dim_ref.Contains(dimension))
                {
                    var code = currentRow[attr.Id] as string;
                    if (!string.IsNullOrEmpty(code))
                    {

                        #region Group Attribute Header
                        /*
                        // cell title
                        TableCell cell_title = new TableCell();
                        cell_title.AddClass(HtmlClasses.ExtraInfoTableLabelTitle);
                        cell_title.ColumnSpan = 2;
                        // cell dimension reference
                        TableCell cell_subtitle = new TableCell();
                        cell_subtitle.AddClass(HtmlClasses.ExtraInfoTableLabelSubTitle);
                        cell_subtitle.ColumnSpan = 2;

                        cell_title.Text = "Group Attribute";
                        cell_subtitle.Text = attr.AttachmentGroup;

                        tb_extra.AddRow(new TableRow(cell_title));
                        tb_extra.AddRow(new TableRow(cell_subtitle));
                        */
                        #endregion

                        string codesDescription = string.Empty;
                        if (this._componentCodesDescriptionMap.ContainsKey(attr.Id))
                        {
                            this._componentCodesDescriptionMap[attr.Id].TryGetValue(code.ToUpper(), out codesDescription);
                            if (string.IsNullOrEmpty(codesDescription))
                                this._componentCodesDescriptionMap[attr.Id].TryGetValue(code.ToLower(), out codesDescription);
                        }

                        tb_extra.AddRow(
                            CreateAttributeRow(
                                model.AllValidKeys[attr.Id],
                                code,
                                codesDescription, true));
                        hasReference = true;
                    }
                }
            }
            #endregion

            #region DimensionGroupAttributes
            foreach (var attr in model.KeyFamily.DimensionGroupAttributes)
            {
                IList<string> dim_ref = attr.DimensionReferences;
                // if current dimensions has reference of attr get the current value
                if (dim_ref.Contains(dimension))
                {

                    var code = currentRow[attr.Id] as string;
                    if (!string.IsNullOrEmpty(code))
                    {
                        #region Dimension Group Attributes Header
                        /*
                        // cell title
                        TableCell cell_title = new TableCell();
                        cell_title.AddClass(HtmlClasses.ExtraInfoTableLabelTitle);
                        cell_title.ColumnSpan = 2;
                        if (dim_ref.Count == (model.KeyFamily.DimensionList.Dimensions.Count - 1))
                        {
                            cell_title.Text = "Series Attribute";
                        }
                        else if (dim_ref.Count == 1)
                        {
                            cell_title.Text = "Dimension Attribute";
                        }
                        else
                        {
                            cell_title.Text = "Dimension Group Attribute";
                        }
                        tb_extra.AddRow(new TableRow(cell_title));
                        */
                        #endregion

                        string codesDescription = string.Empty;
                        if (this._componentCodesDescriptionMap.ContainsKey(attr.Id))
                        {
                            this._componentCodesDescriptionMap[attr.Id].TryGetValue(code.ToUpper(), out codesDescription);
                            if (string.IsNullOrEmpty(codesDescription))
                                this._componentCodesDescriptionMap[attr.Id].TryGetValue(code.ToLower(), out codesDescription);
                        }
                        tb_extra.AddRow(
                            CreateAttributeRow(
                                model.AllValidKeys[attr.Id],
                                code,
                                codesDescription, true));
                        hasReference = true;
                    }
                }
            }
            #endregion

            return (hasReference) ? tb_extra : null;

        }
        private TableRow CreateAttributeRow(string concept, string code, string codeDesc, bool highlighted = false)
        {

            TableRow tr = new TableRow();
            if (highlighted) tr.AddClass(HtmlClasses.ExtraInfoTableRowHighlighted);

            TableCell cell_concept = new TableCell(concept);
            cell_concept.AddClass(HtmlClasses.ExtraInfoTableLabelConcept);

            TableCell cell_concept_value = new TableCell(string.Format("[{0}] - {1}", code, codeDesc));
            cell_concept_value.AddClass(HtmlClasses.ExtraInfoTableLabelConceptValue);

            tr.AddCell(cell_concept);
            tr.AddCell(cell_concept_value);

            return tr;

        }

        #endregion

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
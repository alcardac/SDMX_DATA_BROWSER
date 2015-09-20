using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.DataRender;

namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    public class CsvLayoutRenderer : HtmlRenderer2
    {
        #region Constants and Fields

        /// <summary>
        /// The separator used in the CSV file
        /// </summary>
        private readonly string _separator = ";";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvLayoutRenderer"/> class. 
        /// Initialize a new instance of the CSVLayoutRenderer class using the default separator
        /// </summary>
        /// <param name="componentCodeDescriptionMap">
        /// The component Code Description Map.
        /// </param>
        public CsvLayoutRenderer(ComponentCodeDescriptionDictionary componentCodeDescriptionMap)
            : base(componentCodeDescriptionMap, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvLayoutRenderer"/> class. 
        /// Initialize a new instance of the CSVLayoutRenderer class with a specific separator
        /// </summary>
        /// <param name="separator">
        /// The separator.
        /// </param>
        /// <param name="componentCodeDescriptionMap">
        /// The component Code Description Map.
        /// </param>
        public CsvLayoutRenderer(string separator, ComponentCodeDescriptionDictionary componentCodeDescriptionMap)
            : this(componentCodeDescriptionMap)
        {
            if (!string.IsNullOrEmpty(separator))
            {
                this._separator = separator;
            }
        }

        //public CsvLayoutRenderer(string separator, ComponentCodeDescriptionDictionary componentCodeDescriptionMap, bool useAttr)
        //    : base(componentCodeDescriptionMap, false)
        //{
        //}

        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets the mime type which identifies renderer's result.
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "text/csv";
            }
        }

        /// <summary>
        /// Gets the standard file extension used for the files where to save the content returned by renderer.
        /// </summary>
        public override string StandardFileExtension
        {
            get
            {
                return "csv";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Render to the specified writer the specified model as a csv table
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public override void RenderAllTables(IDataSetModel model, TextWriter os)
        {
            this.WriteTableModels(model, os);
        }

        /// <summary>
        /// Render to the specified writer the specified model as a csv table
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public override void RenderAllTables(IDataSetModel model, Stream os)
        {
            var writer = new StreamWriter(os, Encoding.UTF8);
            this.RenderAllTables(model, writer);
            writer.Flush();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Write all data splitted in slices to the specified writer
        /// </summary>
        /// <param name="model">
        /// The model to write
        /// </param>
        /// <param name="writer">
        /// The writer to use
        /// </param>
        protected override void WriteTableModels(IDataSetModel model, TextWriter writer)
        {
            IDataReader allData = model.GetReader(false);

            string oldKeySetSlice = null;
            var s = new RendererState(model);

            while (allData.Read())
            {
                s.InputRow = allData;
                string currentKeySet = MakeKey(model.SliceKeys, allData);

                if (!currentKeySet.Equals(oldKeySetSlice))
                {
                    oldKeySetSlice = currentKeySet;
                    this.CommitTable(s, writer);
                    Table sliceHeaderTable = this.CreateSliceHeaderTable(s);
                    sliceHeaderTable.WriteCsv(writer, this._separator);
                    s.Table = CreateSliceTable();

                    this.InitTitles(s);
                }

                this.ParseDataRow(s);
            }

            allData.Close();
            s.InputRow = null;
            this.CommitTable(s, writer);
        }

        /// <summary>
        /// Commit the current table and reset state
        /// </summary>
        /// <param name="s">
        /// The state
        /// </param>
        /// <param name="writer">
        /// The writer to commit the current table
        /// </param>
        private void CommitTable(RendererState s, TextWriter writer)
        {
            if (s.Table != null)
            {
                PopulateTable(s);
                s.Table.WriteLineCsv(writer, this._separator);
                s.Reset();
            }
        }

        #endregion
    }
}

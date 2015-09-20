using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.DataRender;
using ISTAT.WebClient.WidgetComplements.Model.Log;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;

namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    public class PdfRenderer : HtmlRenderer2
    {
        #region Constants and Fields

        /// <summary>
        /// Used for converting Postscript points to millimeters. (From Java nsi client)
        /// </summary>
        private const double PostscriptPointToMM = 0.352777778;

        /// <summary>
        /// The border color
        /// </summary>
        private static readonly BaseColor _borderColor = new BaseColor(0x33, 0x99, 0xCC);

        /// <summary>
        /// The default background color
        /// </summary>
        private static readonly BaseColor _defaultBackgroundColor = BaseColor.WHITE;

        /// <summary>
        /// The dimension (i.e. key) value Background Color
        /// </summary>
        private static readonly BaseColor _keyValueBackgroundColor = new BaseColor(0xDF, 0xEF, 0xFC);

        /// <summary>
        /// This object is used to lock multiple threads from populating the pageSizes
        /// </summary>
        private static readonly object _pageSizeLock = new object();

        /// <summary>
        /// Hold the list of supported page sizes
        /// </summary>
        private static readonly Dictionary<string, string> _pageSizes = new Dictionary<string, string>();

        /// <summary>
        /// This field hold the output page size
        /// </summary>
        private readonly Rectangle _pageSize = PageSize.A4;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRenderer"/> class. 
        /// Initialize a new instance of the PDFRenderer class with the specified page type and orientation
        /// </summary>
        /// <param name="pageType">
        /// A String containing the page size name. E.g. A4, letter
        /// </param>
        /// <param name="landscape">
        /// If true the page orientation will be landscape, else potrait
        /// </param>
        /// <param name="componentCodeDescriptionMap">
        /// The map between component code value and description
        /// </param>
        public PdfRenderer(
            string pageType, bool landscape, ComponentCodeDescriptionDictionary componentCodeDescriptionMap)
            : base(componentCodeDescriptionMap, false)
        {
            Rectangle page = PageSize.GetRectangle(pageType);
            if (page != null)
            {
                this._pageSize = landscape ? page.Rotate() : page;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of supported page sizes. Currently everything that is supported by iText
        /// </summary>
        /// <value>
        ///   The list of all supported page size names
        /// </value>
        public static IDictionary<string, string> SupportedPageSizes
        {
            get
            {
                lock (_pageSizeLock)
                {
                    if (_pageSizes.Count == 0)
                    {
                        FieldInfo[] fi = typeof(PageSize).GetFields();

                        for (int i = 0; i < fi.Length; i++)
                        {
                            var value = (Rectangle)fi[i].GetValue(null);
                            string name = fi[i].Name;
                            if (name[0] == '_')
                            {
                                i = fi.Length;
                            }

                            string text = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} ({1}mm x {2}mm)",
                                name,
                                (int)(value.Width * PostscriptPointToMM),
                                (int)(value.Height * PostscriptPointToMM));
                            _pageSizes.Add(name, text);
                        }
                    }
                }

                return _pageSizes;
            }
        }

        /// <summary>
        ///  Gets the mime type which identifies renderer's result.
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "application/pdf";
            }
        }

        /// <summary>
        /// Gets the standard file extension used for the files where to save the content returned by renderer.
        /// </summary>
        public override string StandardFileExtension
        {
            get
            {
                return "pdf";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Render all data add and writes it to the specified writer
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public override void RenderAllTables(IDataSetModel model, Stream os)
        {
            // TODO check the number of horizontal & vertical key values to determine the orientation of the page
            var doc = new Document(this._pageSize, 80, 50, 30, 65);

            // This doesn't seem to do anything... 
            doc.AddHeader(Markup.HTML_ATTR_STYLESHEET, "style/pdf.css");
            doc.AddCreationDate();
            doc.AddCreator("NSI .NET Client");
            try
            {
                PdfWriter.GetInstance(doc, os);
                doc.Open();
                this.WriteTableModels(model, doc);
            }
            catch (DocumentException ex)
            {
                Trace.Write(ex.ToString());
                LogError.Instance.OnLogErrorEvent(ex.ToString());
            }
            catch (DbException ex)
            {
                Trace.Write(ex.ToString());
                LogError.Instance.OnLogErrorEvent(ex.ToString());
            }
            finally
            {
                if (doc.IsOpen())
                {
                    doc.Close();
                }
            }
        }

        /// <summary>
        /// Not implemented by this implementation. Use <see cref="RenderAllTables(Estat.Nsi.Client.Renderer.IDataSetModel,System.IO.Stream)"/> instead
        /// </summary>
        /// <param name="model">
        /// The parameter is not used.
        /// </param>
        /// <param name="os">
        /// The parameter is not used.
        /// </param>
        public override void RenderAllTables(IDataSetModel model, TextWriter os)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Commit the current table and reset state
        /// </summary>
        /// <param name="state">
        /// The state
        /// </param>
        /// <param name="doc">
        /// The pdf document to commit the current table
        /// </param>
        private static void CommitTable(RendererState state, IDocListener doc)
        {
            if (state.Table != null)
            {
                PopulateTable(state);

                // containerTable.Write(writer);
                PdfPTable table = CreatePdf(state.Table);
                if (table != null)
                {
                    doc.Add(table);
                    table.DeleteBodyRows();
                }

                doc.NewPage();
                state.Reset();
            }
        }

        /// <summary>
        /// Create a PDF representation of the table
        /// </summary>
        /// <param name="inputTable">
        /// The <c>Table</c> to parse
        /// </param>
        /// <returns>
        /// A PDF Table
        /// </returns>
        private static PdfPTable CreatePdf(Table inputTable)
        {
            if (inputTable.Children.Count > 0 && inputTable.Children[0] != null)
            {
                TableRow firstRow = inputTable.GetRow(0);
                int cols = firstRow.CellCount;
                var table = new PdfPTable(cols);
                foreach (TableRow row in inputTable.Children)
                {
                    if (row != null)
                    {
                        foreach (TableCell tableCell in row.Children)
                        {
                            if (tableCell != null)
                            {
                                var cell = new PdfPCell(new Paragraph(tableCell.Text))
                                {
                                    Rowspan = tableCell.RowSpan,
                                    Colspan = tableCell.ColumnSpan,
                                    Padding = 3.0f,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    BorderColor = _borderColor,
                                    BackgroundColor = _defaultBackgroundColor
                                };

                                cell.Phrase.Font.SetFamily("Helvetica");
                                cell.Phrase.Font.SetStyle(Font.NORMAL);

                                if (tableCell.HasClass(HtmlClasses.SliceKeyTitle))
                                {
                                    cell.Phrase.Font.SetStyle(Font.BOLD);
                                    cell.BackgroundColor = _keyValueBackgroundColor;
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                }
                                else if (tableCell.HasClass(HtmlClasses.VerticalKeyTitle)
                                         || tableCell.HasClass(HtmlClasses.HorizontalKeyTitle))
                                {
                                    cell.Phrase.Font.SetStyle(Font.BOLD);
                                    cell.BackgroundColor = _keyValueBackgroundColor;
                                }
                                else if (tableCell.HasClass(HtmlClasses.HorizontalKeyValue)
                                         || tableCell.HasClass(HtmlClasses.VerticalKeyValue))
                                {
                                    cell.BackgroundColor = _keyValueBackgroundColor;
                                }

                                // TODO handle this some other way
                                // if (td.Attr.TryGetValue("class", out clazz) && !String.IsNullOrEmpty(clazz)) {
                                // if ("keytitle".Contains(clazz.Substring(1))) {
                                // cell.Phrase.Font.SetStyle(Font.BOLD);
                                // cell.BackgroundColor = new BaseColor(0xDF, 0xEF, 0xFC);
                                // if (clazz.Contains("skeytitle")) {
                                // cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                // }
                                // }
                                // else if (clazz.Contains("keyvalue")) {
                                // cell.BackgroundColor = new BaseColor(0xDF, 0xEF, 0xFC);
                                // }

                                // }
                                table.AddCell(cell);
                            }
                        }
                    }
                }

                return table;
            }

            return null;
        }

        /// <summary>
        /// Create the slice header
        /// </summary>
        /// <param name="doc">
        /// The <see cref="Document"/>
        /// </param>
        /// <param name="state">
        /// The current <see cref="HtmlRenderer.RendererState"/>
        /// </param>
        private void CreateSliceHeader(IElementListener doc, RendererState state)
        {
            Table headerTable = this.CreateSliceHeaderTable(state);
            PdfPTable table = CreatePdf(headerTable);
            if (table != null)
            {
                doc.Add(table);
                table.DeleteBodyRows();
            }
        }

        /// <summary>
        /// Write all data splitted in slices to the specified writer
        /// </summary>
        /// <param name="model">
        /// The dataset model
        /// </param>
        /// <param name="doc">
        /// The PDF document
        /// </param>
        private void WriteTableModels(IDataSetModel model, IDocListener doc)
        {
            IDataReader reader = model.GetReader(false);

            // DIV containerTable = createContainerTable();
            string oldKeySet = null;
            var s = new RendererState(model);

            while (reader.Read())
            {
                s.InputRow = reader;
                string currentKeySet = MakeKey(model.SliceKeys, reader);

                if (!currentKeySet.Equals(oldKeySet))
                {
                    oldKeySet = currentKeySet;
                    CommitTable(s, doc);

                    // containerTable = createContainerTable();
                    this.CreateSliceHeader(doc, s);

                    // addSliceHeading(containerTable, sTable);
                    s.Table = CreateSliceTable();

                    // addSliceTable(containerTable, s.table);
                    this.InitTitles(s);
                }

                this.ParseDataRow(s);
            }

            reader.Close();
            s.InputRow = null;
            CommitTable(s, doc);
        }

        #endregion
    }
}

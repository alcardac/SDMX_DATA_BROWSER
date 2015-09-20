namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// This class represents a HTML TD (cell) with NSI Client extensions. Currently it accepts text or <see cref="Table"/> as child
    /// </summary>
    public class TableCell : HtmlEntityParent<Table>
    {
        #region Constants and Fields

        /// <summary>
        /// The cspan.
        /// </summary>
        private int _columnSpan = 1;

        /// <summary>
        /// The rspan.
        /// </summary>
        private int _rowSpan = 1;


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class. 
        /// </summary>
        public TableCell()
            : base(HtmlConstants.TableCellTag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class. 
        /// Initialize a new instance of the TD class with the specified child
        /// </summary>
        /// <param name="child">
        /// The child to add to the TD instance
        /// </param>
        /// <remarks>
        /// TD childs will not be rendered in PDF or CSV outputs
        /// </remarks>
        public TableCell(Table child)
            : base(HtmlConstants.TableCellTag, child)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class. 
        /// Initialize a new instance of the TD class with the specified text
        /// </summary>
        /// <param name="text">
        /// The text to set for this TD instance
        /// </param>
        public TableCell(string text)
            : base("td", text)
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets an empty TD
        /// </summary>
        public static TableCell Empty
        {
            get
            {
                return new TableCell(string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the cell column span
        /// </summary>
        public int ColumnSpan
        {
            get
            {
                return this._columnSpan;
            }

            set
            {
                this._columnSpan = value;
                this.SetColSpan(this._columnSpan);
            }
        }

        /// <summary>
        /// Gets or sets the current display mode of the TD. <seealso cref="DisplayMode"/>
        /// </summary>
        public DisplayMode Mode
        {
            get
            {
                string o;
                if (this.Attributes.TryGetValue(Sdmx.Mode, out o) && !string.IsNullOrEmpty(o)
                    && Enum.IsDefined(typeof(DisplayMode), o))
                {
                    return (DisplayMode)Enum.Parse(typeof(DisplayMode), o);
                }

                return DisplayMode.Code;
            }

            set
            {
                this.AddAttribute(Sdmx.Mode, ((int)value).ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets or sets the cell row span
        /// </summary>
        public int RowSpan
        {
            get
            {
                return this._rowSpan;
            }

            set
            {
                this._rowSpan = value;
                this.SetRowSpan(this._rowSpan);
            }
        }

        /// <summary>
        /// Gets or sets the sdmx attribute that stores the dimension concept ref
        /// </summary>
        public string SdmxKey
        {
            get
            {
                string sdmxKey;
                return this.Attributes.TryGetValue(Sdmx.Key, out sdmxKey) ? sdmxKey : null;
            }

            set
            {
                this.AddAttribute(Sdmx.Key, value);
            }
        }

        /// <summary>
        /// Gets or sets the sdmx attribute that stores the code description
        /// </summary>
        public string SdmxText
        {
            get
            {
                string sdmxKey;
                return this.Attributes.TryGetValue(Sdmx.Text, out sdmxKey) ? sdmxKey : null;
            }

            set
            {
                this.AddAttribute(Sdmx.Text, value);
            }
        }

        /// <summary>
        /// Gets or sets the sdmx attribute that stores the code value
        /// </summary>
        public string SdmxValue
        {
            get
            {
                string sdmxKey;
                return this.Attributes.TryGetValue(Sdmx.Value, out sdmxKey) ? sdmxKey : null;
            }

            set
            {
                this.AddAttribute(Sdmx.Value, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the cell column span
        /// </summary>
        /// <param name="span">
        /// The column span
        /// </param>
        /// <returns>
        /// This TD instance
        /// </returns>
        public TableCell SetColSpan(int span)
        {
            this._columnSpan = span;
            this.Attributes.Remove(HtmlConstants.ColumnSpanName);
            this.Attributes.Add(HtmlConstants.ColumnSpanName, span.ToString(CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Set the cell row span
        /// </summary>
        /// <param name="span">
        /// The row span
        /// </param>
        /// <returns>
        /// This TD instance
        /// </returns>
        public TableCell SetRowSpan(int span)
        {
            this._rowSpan = span;
            this.Attributes.Remove(HtmlConstants.RowSpanName);
            this.Attributes.Add(HtmlConstants.RowSpanName, span.ToString(CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Setup the display related event and class for <paramref name="dimension"/>
        /// </summary>
        /// <param name="dimension">
        /// The SDMX dimension concept reference
        /// </param>
        public void SetupDisplayText(string dimension)
        {
            this.SdmxKey = dimension;
            this.AddAttribute("onclick", HtmlClasses.OnKeyTitleClick + "(this);");
            this.AddAttribute("display_mode", "0");
            this.AddClass(HtmlClasses.TogglableKey);
        }

        /// <summary>
        /// Setup the TD display text, tooltip, onclick event and custom attributes given the code value and description, the dimension it belongs and the display mode
        /// </summary>
        /// <param name="codeValue">
        /// The code value, in other words the <c>CodeBean.Id</c> from the SDMX Model
        /// </param>
        /// <param name="codeDescription">
        /// The localised code description from the <c>CodeBean.Descriptions</c> from the SDMX Model
        /// </param>
        /// <param name="dimension">
        /// The key (dimension). It is the <c>DimensionBean.ConceptRef</c> from the SDMX Model
        /// </param>
        /// <param name="mode">
        /// The <see cref="DisplayMode"/>
        /// </param>
        /// <param name="enhancedOutput">
        /// Controls whether some events will be included
        /// </param>
        public void SetupDisplayText(
            string codeValue, string codeDescription, string dimension, DisplayMode mode, bool enhancedOutput)
        {
            this.SdmxValue = codeValue;
            if (!string.IsNullOrEmpty(codeDescription))
            {
                if (enhancedOutput)
                {
                    this.SdmxKey = dimension;

                    // SdmxValue = codeValue;
                    this.SdmxText = codeDescription;
                    this.Mode = mode;
                    this.AddAttribute("onclick", HtmlClasses.OnKeyValueClick + "(this);");
                    this.AddClass(HtmlClasses.TogglableKeyValue);
                }

                switch (mode)
                {
                    case DisplayMode.Code:
                        this.Text = codeValue;
                        this.AddAttribute("title", codeDescription);
                        break;
                    case DisplayMode.CodeDescription:
                        this.Text = string.Format(
                            CultureInfo.InvariantCulture, "[{0}] - {1}", codeValue, codeDescription);
                        break;
                    case DisplayMode.Description:
                        this.Text = codeDescription;
                        this.AddAttribute("title", codeValue);
                        break;
                }
            }
            else
            {
                this.Text = codeValue;
            }
        }

        /// <summary>
        /// Write the <see cref="HtmlEntity.Text"/> of this TD cell to the specified writer using the specified separator
        /// </summary>
        /// <param name="writer">
        /// The output TextWriter
        /// </param>
        /// <param name="separator">
        /// The CSV separator
        /// </param>
        public void WriteCsv(TextWriter writer, string separator)
        {
            if (string.IsNullOrEmpty(this.Text) && string.IsNullOrEmpty(separator))
            {
                return;
            }

            if (this.Text != null)
            {
                writer.Write("\"{0}\"", this.Text);
            }

            writer.Write(separator);
        }

        #endregion

        /// <summary>
        /// NSI Client custom attributes
        /// </summary>
        internal static class Sdmx
        {
            #region Constants and Fields

            /// <summary>
            /// The key.
            /// </summary>
            public const string Key = "sdmx_key";

            /// <summary>
            /// The mode.
            /// </summary>
            public const string Mode = "display_mode";

            /// <summary>
            /// The text.
            /// </summary>
            public const string Text = "sdmx_text";

            /// <summary>
            /// The value.
            /// </summary>
            public const string Value = "sdmx_value";

            #endregion
        }
    }
}
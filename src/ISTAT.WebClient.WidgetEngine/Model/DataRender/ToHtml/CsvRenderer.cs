using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    public class CsvRenderer : IDataSetRenderer
    {
        #region Constants and Fields

        /// <summary>
        /// The separator used in the CSV file
        /// </summary>
        private readonly string _separator = ";";

        private readonly bool _withQuotation = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRenderer"/> class. 
        /// Initialize a new instance of the CSVRenderer class using the default separator
        /// </summary>
        public CsvRenderer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRenderer"/> class. 
        /// Initialize a new instance of the CSVRenderer class with a specific separator
        /// </summary>
        /// <param name="separator">
        /// The separator.
        /// </param>
        public CsvRenderer(string separator, bool withQuotation)
        {
            if (!string.IsNullOrEmpty(separator))
            {
                this._separator = separator;
            }
            _withQuotation = withQuotation;
        }


        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets the mime type which identifies renderer's result.
        /// </summary>
        public string MimeType
        {
            get
            {
                return "text/csv";
            }
        }

        /// <summary>
        /// Gets the standard file extension used for the files where to save the content returned by renderer.
        /// </summary>
        public string StandardFileExtension
        {
            get
            {
                return "csv";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Render to the specified writer the specified DataTable as a csv table
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public void RenderAllTables(IDataSetModel model, TextWriter os)
        {
            IDataReader data = model.UnsortedReader;
            StringBuilder sb = new StringBuilder();
            for (int i = 0, j = data.FieldCount; i < j; i++)
            {
                // TODO Check if the concept names should be returned instead
                if (_withQuotation)
                    sb.AppendFormat("\"{0}\"{1}", data.GetName(i), this._separator);
                else
                    sb.AppendFormat("{0}{1}", data.GetName(i), this._separator);
            }

            sb.Length--;
            sb.AppendLine();
            os.Write(sb.ToString());
            sb.Length = 0;
            while (data.Read())
            {
                for (int i = 0, j = data.FieldCount; i < j; i++)
                {
                    string val = data[i] as string;
                    if (_withQuotation)
                        sb.AppendFormat("\"{0}\"{1}", val, this._separator);
                    else
                        sb.AppendFormat("{0}{1}", val, this._separator);
                }

                sb.Length--;
                sb.AppendLine();
                os.Write(sb.ToString());
                sb.Length = 0;
            }

            data.Close();
        }

        /// <summary>
        /// Render to the specified writer the specified DataTable as a csv table
        /// </summary>
        /// <param name="model">
        /// The model to render
        /// </param>
        /// <param name="os">
        /// The output to write to
        /// </param>
        public void RenderAllTables(IDataSetModel model, Stream os)
        {
            StreamWriter writer = new StreamWriter(os);
            this.RenderAllTables(model, writer);
            writer.Flush();
        }

        #endregion
    }
}

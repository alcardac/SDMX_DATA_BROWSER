using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.DataRender;

namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    public class XlsRenderer : HtmlRenderer2
    {
        #region Constructors and Destructors


        /// <summary>
        /// Initializes a new instance of the <see cref="XlsRenderer"/> class. 
        /// Initialize a new instance of the XLSRenderer class
        /// </summary>
        /// <param name="componentCodeDescriptionMap">
        /// A map with component concept id as key and a map between code value and description as value
        /// </param>
        public XlsRenderer(ComponentCodeDescriptionDictionary componentCodeDescriptionMap, string decimalSeparator)
            : base(componentCodeDescriptionMap, false, decimalSeparator)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets the mime type which identifies renderer's result.
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "application/vnd.ms-excel";
            }
        }

        /// <summary>
        /// Gets the standard file extension used for the files where to save the content returned by renderer.
        /// </summary>
        public override string StandardFileExtension
        {
            get
            {
                return "xls";
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
        public override void RenderAllTables(IDataSetModel model, TextWriter os)
        {
            string a = AssemblyDirectory;
            if (DecimalSeparator == ",")
                this.WriteEmbeddedHtml(os, "xls_start_comma.ftl");
            else if (DecimalSeparator == ".")
                this.WriteEmbeddedHtml(os, "xls_start_dot.ftl");
            this.WriteTableModels(model, os);
            this.WriteEmbeddedHtml(os, "xls_finish.ftl");
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        #endregion

    }
}

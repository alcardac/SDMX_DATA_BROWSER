using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISTAT.WebClient.Models;
using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetEngine.Model;
using ISTAT.WebClient.WidgetEngine.Model.DataRender;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using Org.Sdmxsource.Sdmx.Api.Manager.Output;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Structureparser.Manager;
using System.IO;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.Controllers
{
    public class DownloadController : Controller
    {
        #region Private Props

        private const string FileNameTimeStampFormat = "yyyy_MM_dd_HH_mm_ss";
        private long _maxContent;
        private System.IO.MemoryStream _memoryStream;
        private System.IO.TextWriter _textWriter;
        private IDataSetRenderer _datasetRenderer;
        private string _iD;
        private bool _useAttr;
        private DataObjectForStreaming _dataStream;

        #endregion

        #region Private Methods

        private string MakeKey(IDataflowObject artefact)
        {
            return MakeKey(artefact.Id, artefact.AgencyId, artefact.Version);
        }

        private string MakeKey(string id, string agency, string version)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}+{1}+{2}", agency, id, version);
        }

        private string GetFileName(string id, string extension)
        {
            DataObjectForStreaming DataStream = (DataObjectForStreaming)Session[id];

            string df = MakeKey(DataStream.structure.Dataflows.First());
            string stamp = DateTime.Now.ToString(FileNameTimeStampFormat, CultureInfo.InvariantCulture);
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", df, stamp, extension);
        }

        private void InitObject()
        {
            if (Session[_iD] == null || !(Session[_iD] is DataObjectForStreaming))
                throw new Exception("Data not found");

            _maxContent = (long)ISTAT.WebClient.WidgetComplements.Model.Settings.WebClientSettings.Instance.MaxResultHTML;
            _memoryStream = new System.IO.MemoryStream();
            _textWriter = new System.IO.StreamWriter(_memoryStream);
            _useAttr = (ConfigurationManager.AppSettings["ParseSDMXAttributes"].ToString().ToLower() == "true");
            _dataStream = (DataObjectForStreaming)Session[_iD];

        }

        private StreamResponseAction ExportFlush()
        {
            try
            {
                _textWriter.Flush();
                byte[] bytesInStream = _memoryStream.ToArray();
                _memoryStream.Close();

                string contentDisposition = "attachment;filename=\"" + GetFileName(_iD, _datasetRenderer.StandardFileExtension) + "\"";

                if ((bytesInStream.Length / 1000) > _maxContent)
                {
                    this.HttpContext.Response.Clear();
                    return new StreamResponseAction("HTML result out of range");
                }
                else
                {
                    this.HttpContext.Response.Clear();
                    this.HttpContext.Response.ContentType = _datasetRenderer.MimeType;
                    this.HttpContext.Response.AddHeader("content-disposition", contentDisposition);
                    this.HttpContext.Response.BinaryWrite(bytesInStream);
                    this.HttpContext.Response.End();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        private void SaveSDMXFile(ISdmxObjects sdmxObjects, StructureOutputFormatEnumType version, string outputFileName)
        {

            StructureWriterManager swm = new StructureWriterManager();

            StructureOutputFormat soFormat = StructureOutputFormat.GetFromEnum(version);
            IStructureFormat outputFormat = new SdmxStructureFormat(soFormat);

            MemoryStream memoryStream = new MemoryStream();

            swm.WriteStructures(sdmxObjects, outputFormat, memoryStream);


            byte[] bytesInStream = memoryStream.ToArray();
            memoryStream.Close();

            SendAttachment(bytesInStream, outputFileName);
        }

        private void SendAttachment(byte[] bytesInStream, string fileName)
        {

            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "application/force-download";
            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            HttpContext.Response.BinaryWrite(bytesInStream);
            HttpContext.Response.End();

        }

        #endregion

        #region Public Methods

        public StreamResponseAction ExportCsvTabular(string id, string separator, bool withQuotation)
        {
            try
            {
                _iD = id;
                InitObject();

                _datasetRenderer = new CsvRenderer(separator, withQuotation);

                DataWidget.GetDownloadStream(_datasetRenderer, _dataStream, _textWriter);

                return ExportFlush();
            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }
        }

        public StreamResponseAction ExportCsvModel(string id, string separator)
        {
            try
            {
                _iD = id;
                InitObject();

                _datasetRenderer = new CsvLayoutRenderer(separator, _dataStream.codemap);

                DataWidget.GetDownloadStream(_datasetRenderer, _dataStream, _textWriter);

                return ExportFlush();
            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }
        }

        public StreamResponseAction ExportXLS(string id, string separator)
        {
            try
            {
                _iD = id;
                InitObject();

                _datasetRenderer = new XlsRenderer(_dataStream.codemap, separator);

                DataWidget.GetDownloadStream(_datasetRenderer, _dataStream, _textWriter);

                return ExportFlush();
            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }
        }

        public StreamResponseAction ExportPdfModel(string id, string size, bool landscape)
        {

            try
            {
                _iD = id;
                InitObject();

                _datasetRenderer = new PdfRenderer(size, landscape, _dataStream.codemap);

                DataWidget.GetDownloadStream(_datasetRenderer, _dataStream, _memoryStream);

                return ExportFlush();
            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }
        }

        public StreamResponseAction ExportHtmlModel(string id)
        {
            try
            {
                _iD = id;
                InitObject();

                _datasetRenderer = new HtmlRenderer2(_dataStream.codemap, false);

                DataWidget.GetDownloadStream(_datasetRenderer, _dataStream, _textWriter);

                return ExportFlush();
            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }
        }

        public StreamResponseAction ExportSDMXStructure(string id, string sdmxVersion)
        {

            try
            {
                _iD = id;
                InitObject();

                StructureOutputFormatEnumType sdmxEnumType = StructureOutputFormatEnumType.SdmxV21StructureDocument;

                if (sdmxVersion == "20")
                    sdmxEnumType = StructureOutputFormatEnumType.SdmxV2StructureDocument;

                SaveSDMXFile(_dataStream.structure, sdmxEnumType, GetFileName(_iD, "xml"));

                return null;

            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }

        }

        public StreamResponseAction ExportSDMXDataSet(string id, string sdmxVersion)
        {
            try
            {
                _iD = id;
                InitObject();


                DataWidget.GetDataSetStream(_datasetRenderer, _dataStream, _textWriter);

                //var x = _dataStream.store;

                //Copy(query.GetSdmxMLDataSet(true), context.Response.Output);

                //SaveSDMXFile(_dataStream.structure, sdmxEnumType, GetFileName(_iD, "xml"));

                return null;

            }
            catch (Exception ex)
            {
                return new StreamResponseAction(ex.Message);
            }

        }

        private void Copy(string input, TextWriter output)
        {
            using (var sdmx = new StreamReader(input, Encoding.UTF8))
            {
                var buffer = new char[Constants.BufSize];
                while (!sdmx.EndOfStream)
                {
                    int len = sdmx.Read(buffer, 0, Constants.BufSize);
                    output.Write(buffer, 0, len);
                }
            }
            output.Flush();
        }

        public StreamResponseAction ExportSDMXZip(string id)
        {
            //SessionQuery query = SessionQueryManager.GetSessionQuery(context.Session);
            //if (query.DatasetModel == null)
            //{
            //    SessionExpired(context);
            //    return;
            //}

            //context.Response.ContentType = "application/zip";
            //string fileTrailer = Utils.MakeKey(query.Dataflow.DataStructureRef);
            //string format = string.Format(
            //    CultureInfo.InvariantCulture,
            //    Constants.AttachmentFilenameFormat,
            //    GetFileName(query.Dataflow, fileTrailer + "_partial_structure_dataset.zip"));
            //context.Response.AddHeader(Constants.ContentDispositionHttpHeader, format);

            //using (var zip = new ZipOutputStream(context.Response.OutputStream))
            //{
            //    zip.SetLevel(9); // max compression
            //    var dataset = new ZipEntry(GetFileName(query.Dataflow, "dataset.xml"));
            //    zip.PutNextEntry(dataset);
            //    var writer = new StreamWriter(zip, Encoding.UTF8);
            //    Copy(query.GetSdmxMLDataSet(true), writer);
            //    writer.Flush();
            //    var dsd = new ZipEntry(GetFileName(query.Dataflow, fileTrailer + "_partial_structure.xml"));
            //    zip.PutNextEntry(dsd);
            //    var defaultHeader = new HeaderImpl("NSIClient", "NSIClient");
            //    Utils.PopulateHeaderFromSettings(defaultHeader);
            //    ISdmxObjects structure = query.GetFullStructure();
            //    structure.Header = defaultHeader;


            //    IStructureFormat formatZip;
            //    IDataStructureObject dsdDataflow = NsiClientHelper.GetDsdFromDataflow(query.Dataflow, query.Dsds);
            //    if (query.EndPointType == Estat.Nsi.Client.EndpointType.V20 ||
            //           (dsdDataflow != null && NsiClientHelper.DataflowDsdIsCrossSectional(dsdDataflow)))
            //    {
            //        formatZip = new SdmxStructureFormat(StructureOutputFormat.GetFromEnum(StructureOutputFormatEnumType.SdmxV2StructureDocument));
            //    }
            //    else
            //    {
            //        formatZip = new SdmxStructureFormat(StructureOutputFormat.GetFromEnum(StructureOutputFormatEnumType.SdmxV21StructureDocument));
            //    }

            //    IStructureWriterManager structureWritingManager = new StructureWriterManager();
            //    structureWritingManager.WriteStructures(structure, formatZip, zip);

            //    zip.Flush();
            //    context.Response.OutputStream.Flush();
            //}
            throw new NotImplementedException();
        }

        public StreamResponseAction ExportSDMXQuery(string id)
        {
            //SessionQuery query = SessionQueryManager.GetSessionQuery(context.Session);
            //context.Response.ContentType = ContentType;
            //context.Response.ContentEncoding = Encoding.UTF8;
            //string contentDisposition = string.Format(
            //    CultureInfo.InvariantCulture,
            //    Constants.AttachmentFilenameFormat,
            //    GetFileName(query.Dataflow, "query.xml"));
            //context.Response.AddHeader(Constants.ContentDispositionHttpHeader, contentDisposition);
            //var defaultHeader = new HeaderImpl("NSICLIENT", "NSICLIENT");
            //Utils.PopulateHeaderFromSettings(defaultHeader);
            //var queryBuilder = new QueryBuilder(query);
            //IDataQuery sdmxQuery = queryBuilder.CreateQueryBean();
            //var xdoc = new XDocument();

            //if (query.EndPointType == Estat.Nsi.Client.EndpointType.V20 ||
            //    (query.KeyFamily != null && NsiClientHelper.DataflowDsdIsCrossSectional(query.KeyFamily)))
            //{
            //    IDataQueryBuilderManager dataQueryBuilderManager = new DataQueryBuilderManager(new DataQueryFactory());
            //    xdoc = dataQueryBuilderManager.BuildDataQuery(sdmxQuery, new QueryMessageV2Format());
            //}
            //else if (query.EndPointType == Estat.Nsi.Client.EndpointType.V21 ||
            //    query.EndPointType == Estat.Nsi.Client.EndpointType.REST)
            //{
            //    IDataQueryFormat<XDocument> queryFormat = new StructSpecificDataFormatV21();
            //    IBuilder<IComplexDataQuery, IDataQuery> transformer = new DataQuery2ComplexQueryBuilder(true);
            //    IComplexDataQuery complexDataQuery = transformer.Build(sdmxQuery);
            //    IComplexDataQueryBuilderManager complexDataQueryBuilderManager = new ComplexDataQueryBuilderManager(new ComplexDataQueryFactoryV21());
            //    xdoc = complexDataQueryBuilderManager.BuildComplexDataQuery(complexDataQuery, queryFormat);
            //}

            //xdoc.Save(context.Response.Output);
            //context.Response.Output.Flush();
            throw new NotImplementedException();
        }

        #endregion
    }
}

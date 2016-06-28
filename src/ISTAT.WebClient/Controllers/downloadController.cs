using System;
using System.Xml;
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
using ISTAT.WebClient.Controllers;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Factory;
using Org.Sdmxsource.Sdmx.Api.Manager.Query;
using ISTAT.WebClient.WidgetComplements.Model.Settings;
using Org.Sdmxsource.Sdmx.Api.Manager.Output;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Structureparser.Manager;
using System.IO;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.Enum;
using System.Web.Script.Serialization;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;


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

        private void SendAttachmentFile(string pathFileName )
        {


            HttpContext.Response.Clear();
            HttpContext.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", pathFileName));
            HttpContext.Response.ContentType = "application/force-download";

            HttpContext.Response.WriteFile(pathFileName);
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


        public void GetDataSet(IDataSetRenderer renderer, DataObjectForStreaming dataStream, TextWriter streamResponse, string endPointType)
        {
            EndpointSettings DataObjConfiguration = dataStream.Configuration;

            IDataStructureObject kf = dataStream.structure.DataStructures.First();

            //DataObjectForStreaming
            SDMXWSFunction op = SDMXWSFunction.GetCompactData;
            //DataObjConfiguration

            bool cross = (DataObjConfiguration._TypeEndpoint == ISTAT.WebClient.WidgetComplements.Model.Enum.EndpointType.V21 || DataObjConfiguration._TypeEndpoint == ISTAT.WebClient.WidgetComplements.Model.Enum.EndpointType.REST)
                          ? NsiClientHelper.DataflowDsdIsCrossSectional(kf) : !Utils.IsTimeSeries(kf);
            if (cross)
                op = SDMXWSFunction.GetCrossSectionalData;
            var ser = new JavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            try
            {
                IGetSDMX GetSDMXObject = WebServiceSelector.GetSdmxImplementation(DataObjConfiguration);
                BaseDataObject BDO = new BaseDataObject(DataObjConfiguration, "tem.txt");

                string fullPath = Utils.App_Data_Path + @"\Download\" + GetFileName(_iD, "xml");

                IDataQuery query = BDO.CreateQueryBean(dataStream.structure.Dataflows.First(), kf, dataStream.Criterias);
                GetSDMXObject.ExecuteQuery(query, op, fullPath);

                //if (endPointType == "V21")
                //{
                //    SendAttachment(ConvertTo21(fullPath),GetFileName(_iD, "xml")) ;
                //    return;
                //}
                SendAttachmentFile(fullPath);

            }
            catch (Exception ex)
            {

            }
            //throw new NotImplementedException();
        }



        private byte[] ConvertTo21(string fullPath)
        {

            throw new NotImplementedException();
        }

        public StreamResponseAction ExportSDMXDataSet(string id, string sdmxVersion)
        {
            try
            {
                _iD = id;
                InitObject();

                GetDataSet(_datasetRenderer, _dataStream, _textWriter, sdmxVersion);

                //var x = _dataStream.store;

                //Copy(query.GetSdmxMLDataSet(true), context.Response.Output);

                //SaveSDMXFile(_dataStream.structure, sdmxEnumType, GetFileName(_iD, "xml"));

                return null;

                /*
                SessionQuery query = SessionQueryManager.GetSessionQuery(context.Session);
                if (query.GetSdmxMLDataSet(false) == null)
                {
                    SessionExpired(context);
                    return;
                }

                context.Response.ContentType = ContentType;
                context.Response.ContentEncoding = Encoding.UTF8;

                string contentDisposition = string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.AttachmentFilenameFormat,
                    GetFileName(query.Dataflow, "dataset.xml"));
                context.Response.AddHeader(Constants.ContentDispositionHttpHeader, contentDisposition);

                ////            StreamReader sdmx = new StreamReader(query.GetSDMX_MLDataSet(true), Encoding.UTF8);
                Copy(query.GetSdmxMLDataSet(true), context.Response.Output);
                */

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

        public StreamResponseAction ExportSDMXQuery()
        {

            SessionQuery query = SessionQueryManager.GetSessionQuery(Session);

            //ControllerSupport CS = new ControllerSupport();
            //GetCodemapObject PostDataArrived = CS.GetPostData<GetCodemapObject>(this.Request);
            //PostDataArrived.Configuration.Locale = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            string request = "";
            var xdoc = new XmlDocument();

            EndpointSettings DataObjConfiguration = new EndpointSettings();
            DataObjConfiguration = query._endpointSettings;
            

            IGetSDMX GetSDMXObject = WebServiceSelector.GetSdmxImplementation(DataObjConfiguration);
            BaseDataObject BDO = new BaseDataObject(DataObjConfiguration, "appo.xml");

            ISdmxObjects structure = query.Structure;
            IDataflowObject df = structure.Dataflows.First();
            IDataStructureObject kf = structure.DataStructures.First();
            IDataQuery sdmxQuery = BDO.CreateQueryBean(df, kf, query.GetCriteria());
            GetSDMXObject.GetSdmxQuery(sdmxQuery, out request);

            string filename = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", query.Dataflow.Id, "xml");

            this.HttpContext.Response.Clear();
            this.HttpContext.Response.ContentType = "text/xml";
            this.HttpContext.Response.ContentEncoding = Encoding.UTF8;
            string contentDisposition = string.Format(
                CultureInfo.InvariantCulture,
                Constants.AttachmentFilenameFormat,
                filename);
            this.HttpContext.Response.AddHeader(Constants.ContentDispositionHttpHeader, contentDisposition);



            this.HttpContext.Response.AddHeader("content-disposition", contentDisposition);
            this.HttpContext.Response.Write(request);
            this.HttpContext.Response.End();
            throw new NotImplementedException();
            
        }

        #endregion
    }
}

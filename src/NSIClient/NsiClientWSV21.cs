// -----------------------------------------------------------------------
// <copyright file="NsiClientWSV21.cs" company="EUROSTAT">
//   Date Created : 2011-09-28
//   Copyright (c) 2009, 2015 by the European Commission, represented by Eurostat.   All rights reserved.
// 
// Licensed under the EUPL, Version 1.1 or – as soon they
// will be approved by the European Commission - subsequent
// versions of the EUPL (the "Licence");
// You may not use this work except in compliance with the
// Licence.
// You may obtain a copy of the Licence at:
// 
// https://joinup.ec.europa.eu/software/page/eupl 
// 
// Unless required by applicable law or agreed to in
// writing, software distributed under the Licence is
// distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the Licence for the specific language governing
// permissions and limitations under the Licence.
// </copyright>
// -----------------------------------------------------------------------
namespace Estat.Nsi.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;

    using Estat.Nsi.Client.Properties;
    using Estat.Sdmxsource.Extension.Builder;
    using Estat.Sdmxsource.Extension.Constant;
    using Estat.Sdmxsource.Extension.Util;
    using Estat.Sri.CustomRequests.Factory;
    using Estat.Sri.CustomRequests.Manager;
    using Estat.Sri.CustomRequests.Model;

    using log4net;

    using Org.Sdmxsource.Sdmx.Api.Builder;
    using Org.Sdmxsource.Sdmx.Api.Constants;
    using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
    using Org.Sdmxsource.Sdmx.Api.Model;
    using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
    using Org.Sdmxsource.Sdmx.Api.Model.Format;
    using Org.Sdmxsource.Sdmx.Api.Model.Header;
    using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference.Complex;
    using Org.Sdmxsource.Sdmx.Api.Model.Query;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference.Complex;
    using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
    using Org.Sdmxsource.Sdmx.Util.Objects.Container;
    using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Util.Xml;
    using Org.Sdmxsource.Util.Io;

    /// <summary>
    /// An implementation of the <see cref="INsiClient"/> that retrieves structural metadata and data from a SDMX v2.1 web service
    /// </summary>
    public class NsiClientWSV21 : INsiClient
    {
        #region Constants and Fields

        /// <summary>
        /// Empty list of <see cref="CategorySchemeBean"/>
        /// </summary>
        private static readonly IList<ICategorySchemeObject> _emptyCategoryScheme = new ReadOnlyCollection<ICategorySchemeObject>(new List<ICategorySchemeObject>());
        /// <summary>
        /// Empty list of <see cref="DataflowBean"/>
        /// </summary>
        private static readonly ISet<IDataflowObject> _emptyDataflowList = new HashSet<IDataflowObject>();

        /// <summary>
        ///  The Web Service Client configuration
        /// </summary>
        private readonly WsInfo _config;

        /// <summary>
        /// Default Header
        /// </summary>
        private readonly IHeader _defaultHeader;

        /// <summary>
        /// Log class
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(NsiClientWSV21));
        /// <summary>
        /// The configuration retrieved from the WSDL
        /// </summary>
        private readonly WSDLSettings _wsdlConfig;

        private readonly NsiClientWS _nsiClientWs;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NsiClientWS"/> class. 
        /// </summary>
        /// <param name="config">
        /// The config
        /// </param>
        public NsiClientWSV21(WsInfo config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(config.EndPoint))
            {
                throw new ArgumentException(Resources.ExceptionEndpointNotSet, "config");
            }
            
            //getting nsiClientWs
            WsInfo wsInfo = new WsInfo(config.Config, SdmxSchemaEnumType.VersionTwo);
            _nsiClientWs = new NsiClientWS(wsInfo);
           
            Logger.Info(Resources.InfoCreatingNsiClient);
            _config = config;
            Logger.Info(Resources.InfoGetWSDL);
            try
            {
                this._wsdlConfig = new WSDLSettings(config);
                Logger.Info(Resources.InfoWSDLSuccess);
            }
            catch (WebException ex)
            {
                throw NsiClientHelper.HandleWsdlException(config.EndPoint, ex, config.Wsdl);
            }
            catch (InvalidOperationException ex)
            {
                throw NsiClientHelper.HandleWsdlException(config.EndPoint, ex, config.Wsdl);
            }
            catch (UriFormatException ex)
            {
                throw NsiClientHelper.HandleWsdlException(config.EndPoint, ex, config.Wsdl);
            }

            this._defaultHeader = new HeaderImpl("NSIClient", "NSIClient");
            Utils.PopulateHeaderFromSettings(this._defaultHeader);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute SDMX Query against the NSI WS to retrieve SDMX-ML Data as a Stream
        /// </summary>
        /// <param name="query">
        /// The SDMX Query to execute
        /// </param>
        /// <param name="operationName">
        /// The type of operation, GetCompactData or GetCrossSectionalData
        /// </param>
        /// <param name="tempFileName">
        /// The temporary file name
        /// </param>
        /// <exception cref="NsiClientException">Failute to execute query</exception>
        /// <returns>
        /// The SDMX-ML data as a stream.
        /// </returns>
        public void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, string tempFileName)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            try
            {
                switch (operationName)
                {
                    case SDMXWSFunction.GetCompactData:
                        {
                            this.SendSdmxQuery(query, SDMXWSFunctionV21.GetStructureSpecificData, tempFileName);

                            using (var dataLocation = new FileReadableDataLocation(tempFileName))
                            {
                                var sdmxFooterMessage = SdmxMessageUtilExt.ParseSdmxFooterMessage(dataLocation);
                                if (sdmxFooterMessage != null && (sdmxFooterMessage.Code.Equals("510") || sdmxFooterMessage.Code.Equals("130")))
                                {
                                    var sb = new StringBuilder();
                                    foreach (var footerText in sdmxFooterMessage.FooterText)
                                    {
                                        sb.Append(footerText.Value + " ");
                                    }
                                    string info = string.Format(CultureInfo.InvariantCulture, Resources.SdmxFooterMessage, sb, sdmxFooterMessage.Code, sdmxFooterMessage.Severity);
                                    Logger.ErrorFormat(CultureInfo.InvariantCulture, Resources.MaxObservations, info);
                                    throw new FooterMessageException(Resources.EnterMoreCriteria);
                                }
                            }
                            break;
                        }
                    case  SDMXWSFunction.GetCrossSectionalData:
                        {
                           _nsiClientWs.ExecuteQuery(query, operationName,tempFileName);
                            break;
                        }  
                    default:
                        {
                           Logger.Error(Resources.ExceptionExecuteQuery);
                           throw new NsiClientException(Resources.ExceptionExecuteQuery);
                        }
                }
            }
            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionExecuteQuery);
                NsiClientHelper.TryToDelete(tempFileName);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (DataflowException e)
            {
                throw;
            }
            catch (FooterMessageException e)
            {
                NsiClientHelper.TryToDelete(tempFileName);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionExecuteQuery);
                NsiClientHelper.TryToDelete(tempFileName);
                throw new NsiClientException(Resources.ExceptionExecuteQuery, e);
            }

        }

        /// <summary>
        /// Execute SDMX Query against the NSI WS to retrieve SDMX-ML Data as a Stream
        /// </summary>
        /// <param name="query">
        /// The SDMX Query to execute
        /// </param>
        /// <param name="operationName">
        /// The type of operation, GetCompactData or GetCrossSectionalData
        /// </param>
        /// <param name="observationLimit">
        /// The maximum number of observations to return
        /// </param>
        /// <param name="tempFileName">
        /// The temporary fila name
        /// </param>
        /// <returns>
        /// The SDMX-ML data as a stream.
        /// </returns>
        public void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, int observationLimit, string tempFileName)
        {
             query = new DataQueryFluentBuilder().Initialize(query.DataStructure, query.Dataflow)
                .WithOrderAsc(true)
                .WithDataQuerySelectionGroup(query.SelectionGroups).WithMaxObservations(observationLimit).Build();
      
             this.ExecuteQuery(query, operationName, tempFileName);
        }

        /// <summary>
        /// Gets a bean with data about the codelist for specified dataflow and component.
        /// The dataflow can be retrieved from <see cref="RetrieveCategorisations"/> and the component from <see cref="GetStructure"/>
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <param name="dsd">
        /// The dsd
        /// </param>
        /// <param name="component">
        /// The component
        /// </param>
        /// <param name="criteria">
        /// The criteria includes a set of Member and MemberValue(s) for each dimension. The Member has componentRef the dimension conceptRef and the MemberValue(s) specify the selected codes for this dimension.
        /// </param>
        /// <returns>
        /// A <c>CodeListBean</c> with the requested data
        /// </returns>
        public ICodelistObject GetCodelist(IDataflowObject dataflow, IDataStructureObject dsd, IComponent component, IContentConstraintMutableObject criteria)
        {
            return _nsiClientWs.GetCodelist(dataflow, dsd, component, criteria);
        }

        /// <summary>
        /// Get the maximum number of observations that can be retrieved given the specified criteria
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <param name="criteria">
        /// The criteria includes a set of Member and MemberValue(s) for each dimension. Each member should have member values else they shouldn't be included. It can be null
        /// </param>
        /// <returns>
        /// The maximum number of observations or -1 if it can't be parsed or it is not available
        /// </returns>
        /// <exception cref="NsiClientException">
        /// NSI WS communication error or parsing error
        /// </exception>
        public int GetDataflowDataCount(IDataflowObject dataflow, IContentConstraintMutableObject criteria)
        {
            return _nsiClientWs.GetDataflowDataCount(dataflow, criteria);
        }

        /// <summary>
        /// Retrieves all available categorisations.
        /// </summary>
        /// <returns>
        ///   a list of &amp;lt;c&amp;gt;ISdmxObjects&amp;lt;/c&amp;gt; instances; the result won&amp;apos;t be &amp;lt;c&amp;gt;null&amp;lt;/c&amp;gt; if there are no
        ///   dataflows, instead an empty list will be returned
        /// </returns>
        public ISdmxObjects RetrieveCategorisations()
        {
            
            Logger.Info(Resources.InfoGettingCategorySchemes);

            //Get category schemes and categorisations
            ISdmxObjects responseCategorySchemes = new SdmxObjectsImpl();
            IComplexStructureQuery complexStructureQueryCategoryScheme = RetrieveCategorySchemesAndCategorisations();
           
            //Get dataflows
            ISdmxObjects responseDataflows = new SdmxObjectsImpl();
            IComplexStructureQuery complexStructureQueryDataflow = RetrieveDataflows();
         
            try
            {
                try
                {
                    responseCategorySchemes = this.SendQueryStructureRequest(complexStructureQueryCategoryScheme, SDMXWSFunctionV21.GetCategoryScheme);
                }
                catch (DataflowException ex)
                {
                    //do nothing
                }
                responseDataflows = this.SendQueryStructureRequest(complexStructureQueryDataflow,SDMXWSFunctionV21.GetDataflow);

                //Remove from structure (ISdmxObjects) the DSDS built with SDMX v2.0
                var structureSdmxV20DSD = responseDataflows.DataStructures.Where(o => o.Annotations.Any(a => a.FromAnnotation() == CustomAnnotationType.SDMXv20Only)).ToArray();
                foreach (var sdmxV20Only in structureSdmxV20DSD)
                {
                    responseDataflows.RemoveDataStructure(sdmxV20Only);
                }
              
                // DSDS with annotation
                var sdmxV20onlyReferences = structureSdmxV20DSD.Select(o => o.AsReference).ToArray();

                // Add the DSDS built with Sdmx V2.0
                ISdmxObjects responseDSD = new SdmxObjectsImpl();
                if (sdmxV20onlyReferences.Length > 0)
                {
                    responseDSD = _nsiClientWs.SendQueryStructureRequest(sdmxV20onlyReferences, false);
                    responseDataflows.Merge(responseDSD);
                }
                
                responseCategorySchemes.Merge(responseDataflows);

                if (responseCategorySchemes.CategorySchemes != null && responseCategorySchemes.Dataflows != null)
                {
                    Logger.Info(Resources.InfoSuccess);
                }
            }
            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionGettingDataflow);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (DataflowException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionGettingDataflow);
                Logger.Error(e.Message, e);
                throw new NsiClientException(Resources.ExceptionGettingDataflow, e);
            }
            return responseCategorySchemes;
        }

        /// <summary>
        /// Retrieves all available categorisations and category schemes.
        /// </summary>
        private IComplexStructureQuery RetrieveCategorySchemesAndCategorisations()
        {
            var catSch = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CategoryScheme));
            IRestStructureQuery structureQueryCategoryScheme = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Parents), null, catSch, false);
            IBuilder<IComplexStructureQuery, IRestStructureQuery> transformerCategoryScheme = new StructureQuery2ComplexQueryBuilder();

            IComplexStructureQuery complexStructureQueryCategoryScheme = transformerCategoryScheme.Build(structureQueryCategoryScheme);

            return complexStructureQueryCategoryScheme;
        }

        /// <summary>
        /// Retrieves all available dataflows.
        /// </summary>
        private IComplexStructureQuery RetrieveDataflows()
        {
         
            var dataflowRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));
            IRestStructureQuery structureQueryDataflow = new RESTStructureQueryCore(dataflowRefBean);

            IBuilder<IComplexStructureQuery, IRestStructureQuery> transformerDataFlow = new StructureQuery2ComplexQueryBuilder();

            IComplexStructureQuery complexStructureQueryDataflow = transformerDataFlow.Build(structureQueryDataflow);

            IList<SdmxStructureType> specificObjects = new List<SdmxStructureType>();
            specificObjects.Add(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd));

            IComplexStructureQueryMetadata complexStructureQueryMetadataWithDsd =
                    new ComplexStructureQueryMetadataCore(false,
                    ComplexStructureQueryDetail.GetFromEnum(ComplexStructureQueryDetailEnumType.Full),
                    ComplexMaintainableQueryDetail.GetFromEnum(ComplexMaintainableQueryDetailEnumType.Full),
                    StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Specific),
                    specificObjects);

            IComplexStructureQuery complexStructureQueryTempDataflow = new ComplexStructureQueryCore(
                    complexStructureQueryDataflow.StructureReference, complexStructureQueryMetadataWithDsd);

            return complexStructureQueryTempDataflow;
        }


        /// <summary>
        /// Gets a bean with data about the key family for specified dataflow.
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <returns>
        /// a <c>StructureBean</c> instance with requested data; the result is never <c>null</c> or  incomplete, instead an exception is throwed away if something goes wrong and not all required data is successfully retrieved
        /// </returns>
        /// <remarks>
        /// The resulted bean will contain exactly one key family, but also will include any concepts and codelists referenced by the key family.
        /// </remarks>
        public ISdmxObjects GetStructure(IDataflowObject dataflow, ISet<IDataStructureObject> dataStructures)
        {
            Logger.InfoFormat(
                    CultureInfo.InvariantCulture, 
                    Resources.InfoGettingStructureFormat3, 
                    dataflow.AgencyId, 
                    dataflow.Id, 
                    dataflow.Version);
            ISdmxObjects structure = new SdmxObjectsImpl();

            try
            {
                ISdmxObjects responseConceptScheme = new SdmxObjectsImpl();
                ISdmxObjects response = new SdmxObjectsImpl();
             
                IDataStructureObject dsd = NsiClientHelper.GetDsdFromDataflow(dataflow, dataStructures);
                structure.AddDataStructure(dsd);

                NsiClientValidation.CheckifStructureComplete(structure, dataflow);
                IEnumerable<IStructureReference> conceptRefs = NsiClientHelper.BuildConceptSchemeRequest(structure.DataStructures.First());
                foreach (var structureReference in conceptRefs)
                {
                    IRestStructureQuery structureQueryConceptScheme = new RESTStructureQueryCore(structureReference);
                    IBuilder<IComplexStructureQuery, IRestStructureQuery> transformerCategoryScheme = new StructureQuery2ComplexQueryBuilder();
                    IComplexStructureQuery complexStructureQueryConceptScheme = transformerCategoryScheme.Build(structureQueryConceptScheme);
                    responseConceptScheme = this.SendQueryStructureRequest(complexStructureQueryConceptScheme, SDMXWSFunctionV21.GetConceptScheme);
                    response.Merge(responseConceptScheme);
                }
                structure.Merge(response);

                NsiClientValidation.CheckConcepts(structure);
                Logger.Info(Resources.InfoSuccess);
            }
            catch (NsiClientException e)
            {
                Logger.Error(Resources.ExceptionGettingStructure);
                Logger.Error(e.Message, e);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(Resources.ExceptionGettingStructure);
                Logger.Error(e.Message, e);
                throw new NsiClientException(Resources.ExceptionGettingStructure, e);
            }

            return structure;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the specified <paramref name="complexStructureQuery"/> to the Web Service defined by <see cref="_config"/> 
        /// </summary>
        /// <param name="complexStructureQuery">The <see cref="IComplexStructureQuery"/></param>
        /// <returns>The ISdmxObjects returned by the Web Service</returns>
        private ISdmxObjects SendQueryStructureRequest(IComplexStructureQuery complexStructureQuery, SDMXWSFunctionV21 sdmxwsFunctionV21)
        {

            IStructureQueryFormat<XDocument> queryFormat = new ComplexQueryFormatV21();

            IComplexStructureQueryFactory<XDocument> factory = new ComplexStructureQueryFactoryV21<XDocument>();
            IComplexStructureQueryBuilderManager<XDocument> complexStructureQueryBuilderManager = new ComplexStructureQueryBuilderManager<XDocument>(factory);
            var wdoc = complexStructureQueryBuilderManager.BuildComplexStructureQuery(complexStructureQuery, queryFormat);
            var doc = new XmlDocument();
            doc.LoadXml(wdoc.ToString());
         
            string tempFileName = Path.GetTempFileName();

            try
            {
                this.SendRequest(doc, sdmxwsFunctionV21, tempFileName);

                ISdmxObjects structureObjects = new SdmxObjectsImpl();
                IStructureParsingManager parsingManager = new StructureParsingManager(SdmxSchemaEnumType.Null);
                using (var dataLocation = new FileReadableDataLocation(tempFileName))
                {
                    IStructureWorkspace structureWorkspace = parsingManager.ParseStructures(dataLocation);
                    structureObjects = structureWorkspace.GetStructureObjects(false);
                }
         
                NsiClientValidation.CheckResponse(structureObjects);
                return structureObjects;
            }
            finally
            {
                //Delete the temporary file
                File.Delete(tempFileName);
            }
        }


        /// <summary>
        /// Constructs a SOAP envelope request, with a body that includes the operation as element and the W3C Document and saves the SDMX Part of the response to the specified ouput
        /// The W3C Document contains either a SDMX-ML Query or a SDMX-ML Registry Interface
        /// </summary>
        /// <param name="request">
        /// The W3C Document representation of a SDMX-ML Query or QueryStructureRequest
        /// </param>
        /// <param name="webServiceOperation">
        /// The Web Service function
        /// </param>
        /// <param name="tempFileName">
        /// The temporary file name
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// request is null
        /// </exception>
        /// <exception cref="NsiClientException">
        /// Error in server response or communication
        /// </exception>
        private void SendRequest(XmlDocument request, SDMXWSFunctionV21 webServiceOperation, string tempFileName)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            NsiClientHelper.LogSdmx(_config, request);
            var sb = new StringBuilder();
            sb.AppendFormat(SoapConstants.SoapRequest, this._config.Prefix, this._wsdlConfig.GetTargetNamespace());
            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            string operationName = webServiceOperation.ToString();
            XmlNodeList nodes = doc.GetElementsByTagName(SoapConstants.Body, SoapConstants.Soap11Ns);
            XmlElement operation = doc.CreateElement(
                this._config.Prefix, operationName, this._wsdlConfig.GetTargetNamespace());

            XmlElement queryParent = operation;
            string parameterName = this._wsdlConfig.GetParameterName(operationName);
            if (!string.IsNullOrEmpty(parameterName))
            {
                queryParent = doc.CreateElement(
                    this._config.Prefix, parameterName, this._wsdlConfig.GetTargetNamespace());
                operation.AppendChild(queryParent);
            }

            if (request.DocumentElement != null)
            {
                XmlNode sdmxQueryNode = doc.ImportNode(request.DocumentElement, true);
                queryParent.AppendChild(sdmxQueryNode);
            }

            nodes[0].AppendChild(operation);

            var endpointUri = new Uri(this._config.EndPoint);
            var webRequest = (HttpWebRequest)WebRequest.Create(endpointUri);
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            string soapAction = this._wsdlConfig.GetSoapAction(operationName);
            if (soapAction != null)
            {
                webRequest.Headers.Add(SoapConstants.SoapAction, soapAction);
            }

            webRequest.ContentType = HttpConstants.Content;

            // webRequest.Accept = "text/xml";
            webRequest.Method = HttpConstants.Post;
            webRequest.Timeout = 1800*1000;
            // webRequest.CookieContainer = new CookieContainer();
            this._config.SetupWebRequestAuth(webRequest);

            using (Stream stream = webRequest.GetRequestStream())
            {
                doc.Save(stream);
            }

            try
            {
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.Indent = true;
                            using (XmlWriter writer = XmlWriter.Create(tempFileName, settings))
                            {
                                SoapUtils.ExtractSdmxMessage(stream, writer);
                            }
                            NsiClientHelper.LogSdmx(_config, tempFileName, Resources.InfoSoapResponse);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                NsiClientHelper.HandleSoapFault(ex);
            }
        }

        /// <summary>
        /// Sends the SDMX Query Request
        /// </summary>
        /// <param name="query">
        /// The SDMX Query
        /// </param>
        /// <param name="operation">
        /// The Web Service function
        /// </param>
        /// <param name="output">
        /// The output stream
        /// </param>
        private void SendSdmxQuery(IDataQuery query, SDMXWSFunctionV21 operation, string tempFileName)
        {
            IDataQueryFormat<XDocument> queryFormat = new StructSpecificDataFormatV21();
            IBuilder<IComplexDataQuery, IDataQuery> transformer = new DataQuery2ComplexQueryBuilder(true);
            IComplexDataQuery complexDataQuery = transformer.Build(query);
            
            IComplexDataQueryBuilderManager complexDataQueryBuilderManager = new ComplexDataQueryBuilderManager(new ComplexDataQueryFactoryV21());
            var xdoc = complexDataQueryBuilderManager.BuildComplexDataQuery(complexDataQuery, queryFormat);
            var doc = new XmlDocument();
            doc.LoadXml(xdoc.ToString());
            this.SendRequest(doc, operation, tempFileName);
        }

        #endregion
    }
}
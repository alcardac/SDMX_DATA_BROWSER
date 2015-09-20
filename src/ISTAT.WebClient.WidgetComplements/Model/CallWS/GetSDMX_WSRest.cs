
namespace ISTAT.WebClient.WidgetComplements.Model.CallWS
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;

    using ISTAT.WebClient.WidgetComplements.Model.Properties;
    using Estat.Sdmxsource.Extension.Constant;

    using log4net;

    using Org.Sdmxsource.Sdmx.Api.Constants;
    using Org.Sdmxsource.Sdmx.Api.Factory;
    using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
    using Org.Sdmxsource.Sdmx.Api.Manager.Query;
    using Org.Sdmxsource.Sdmx.Api.Model;
    using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.Api.Model.Format;
    using Org.Sdmxsource.Sdmx.Api.Model.Header;
    using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference.Complex;
    using Org.Sdmxsource.Sdmx.Api.Model.Query;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
    using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Factory;
    using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Manager;
    using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Model;
    using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
    using Org.Sdmxsource.Sdmx.Util.Objects.Container;
    using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
    using Org.Sdmxsource.Util.Io;

    using RestSharp;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    using ISTAT.WebClient.WidgetComplements.Model.Exceptions;
    using ISTAT.WebClient.WidgetComplements.Model.Settings;

    /// <summary>
    /// An implementation of the <see cref="IGetSDMX"/> that retrieves structural metadata and data from a SDMX v2.1 web service
    /// </summary>
    public class GetSDMX_WSRest : IGetSDMX
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
        private readonly EndpointSettings _config;

        /// <summary>
        /// Default Header
        /// </summary>
        private readonly IHeader _defaultHeader;

        /// <summary>
        /// Log class
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GetSDMX_WSV21));

        /// <summary>
        /// The _nsi client ws.
        /// </summary>
        private readonly GetSDMX_WSV20 _nsiClientWs;


        /// <summary>
        /// Request Type
        /// </summary>
        public enum RequestType
        {
            /// <summary>
            /// The data.
            /// </summary>
            Data,

            /// <summary>
            /// The structure.
            /// </summary>
            Structure
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="GetSDMX_WSRest"/> class.
        /// </summary>
        static GetSDMX_WSRest()
        {
            // HACK bug in .NET 4.0 URI. It removes any trailing dots in any part of the URL
            // Workaround from http://stackoverflow.com/questions/856885/httpwebrequest-to-url-with-dot-at-the-end
            MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    var parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        var flagsValue = (int)flagsField.GetValue(parser);

                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                        {
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSDMX_WSRest"/> class. 
        /// </summary>
        /// <param name="config">
        /// The config
        /// </param>
        public GetSDMX_WSRest(EndpointSettings config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(config.EndPoint))
            {
                throw new ArgumentException(Resources.ExceptionEndpointNotSet, "config");
            }

            // Getting nsiClientWs
            EndpointSettings V20Sett = (EndpointSettings)config.Clone();
            V20Sett.EndPointType = "V20";
            V20Sett.EndPoint = V20Sett.EndPointV20;
            this._nsiClientWs = new GetSDMX_WSV20(V20Sett);

            Logger.Info(Resources.InfoCreatingNsiClient);
            this._config = config;

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
        /// <exception cref="NsiClientException">
        /// Failute to execute query
        /// </exception>
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
                            this.SendSdmxQuery(query, tempFileName);
                            break;
                        }

                    case SDMXWSFunction.GetCrossSectionalData:
                        {
                            this._nsiClientWs.ExecuteQuery(query, operationName, tempFileName);
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
        public void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, int observationLimit, string tempFileName)
        {
            query = new DataQueryFluentBuilder().Initialize(query.DataStructure, query.Dataflow)
               .WithDataQuerySelectionGroup(query.SelectionGroups).Build();

            this.ExecuteQuery(query, operationName, tempFileName);
        }

        /// <summary>
        /// Gets a bean with data about the codelist for specified dataflow and component.
        /// The dataflow can be retrieved from <see cref="RetrieveTree"/> and the component from <see cref="GetStructure"/>
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
        public ICodelistObject GetCodelist(IDataflowObject dataflow, IDataStructureObject dsd, IComponent component, List<IContentConstraintMutableObject> criterias, bool Constrained)
        {
            return this._nsiClientWs.GetCodelist(dataflow, dsd, component, criterias, Constrained);
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
            return this._nsiClientWs.GetDataflowDataCount(dataflow, criteria);
        }

        /// <summary>
        /// Retrieves all available categorisations.
        /// </summary>
        /// <returns>
        ///   a list of &amp;lt;c&amp;gt;ISdmxObjects&amp;lt;/c&amp;gt; instances; the result won&amp;apos;t be &amp;lt;c&amp;gt;null&amp;lt;/c&amp;gt; if there are no
        ///   dataflows, instead an empty list will be returned
        /// </returns>
        public ISdmxObjects RetrieveTree()
        {

            Logger.Info(Resources.InfoGettingCategorySchemes);

            // Get category schemes and categorisations
            ISdmxObjects responseCategorySchemes = new SdmxObjectsImpl();
            string requestCategoryScheme = this.RetrieveCategorySchemesAndCategorisations();

            // Get dataflows
            ISdmxObjects responseDataflows = new SdmxObjectsImpl();
            string requestDataFlows = this.RetrieveDataflows();

            try
            {
                try
                {
                    responseCategorySchemes = this.SendQueryStructureRequest(requestCategoryScheme);
                }
                catch (DataflowException ex)
                {
                    //do nothing
                }
                responseDataflows = this.SendQueryStructureRequest(requestDataFlows);

                // Remove from structure (ISdmxObjects) the DSDS built with SDMX v2.0
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
                    responseDSD = this._nsiClientWs.SendQueryStructureRequest(sdmxV20onlyReferences, false);
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

        public ISdmxObjects GetDsd(string DsdId, string DsdAgency, string DsdVersion, bool resolseRef = false)
        {
            return null;
        }

        /// <summary>
        /// Retrieves all available categorisations and category schemes.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string RetrieveCategorySchemesAndCategorisations()
        {
            var catSch = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CategoryScheme));
            IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
            IRestStructureQuery structureQueryCategoryScheme = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Parents), null, catSch, false);
            IStructureQueryFactory factory = new RestStructureQueryFactory();

            IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);
            string request = structureQueryBuilderManager.BuildStructureQuery(structureQueryCategoryScheme, structureQueryFormat);

            return request;
        }

        /// <summary>
        /// Retrieves all available dataflows.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string RetrieveDataflows()
        {

            var dataflowRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));

            IRestStructureQuery structureQuery = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Specific), SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd), dataflowRefBean, true);
            IStructureQueryFactory factory = new RestStructureQueryFactory();
            IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);
            IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
            string request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);

            return request;
        }

        private string RetrieveDataflow(string id, string agency, string version)
        {

            IMaintainableRefObject df = new MaintainableRefObjectImpl(agency, id, version);
            var dataflowRefBean = new StructureReferenceImpl(df,SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));

            IRestStructureQuery structureQuery = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Specific), SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd), dataflowRefBean, true);
            IStructureQueryFactory factory = new RestStructureQueryFactory();
            IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);
            IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
            string request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);

            return request;
        }

        /// <summary>
        /// Gets a bean with data about the key family for specified dataflow.
        /// </summary>
        /// <param name="dataflow">
        /// The dataflow
        /// </param>
        /// <param name="dataStructures">
        /// The data Structures.
        /// </param>
        /// <returns>
        /// a <c>StructureBean</c> instance with requested data; the result is never <c>null</c> or  incomplete, instead an exception is throwed away if something goes wrong and not all required data is successfully retrieved
        /// </returns>
        /// <remarks>
        /// The resulted bean will contain exactly one key family, but also will include any concepts and codelists referenced by the key family.
        /// </remarks>
        public ISdmxObjects GetStructure(IDataflowObject dataflow, ISet<IDataStructureObject> dataStructures, bool resolseRef = false)
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

                IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
                IStructureQueryFactory factory = new RestStructureQueryFactory();
                IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);

                IDataStructureObject dsd = NsiClientHelper.GetDsdFromDataflow(dataflow, dataStructures);
                structure.AddDataStructure(dsd);

                NsiClientValidation.CheckifStructureComplete(structure, dataflow);
                IEnumerable<IStructureReference> conceptRefs = NsiClientHelper.BuildConceptSchemeRequest(structure.DataStructures.First());
                foreach (var structureReference in conceptRefs)
                {
                    IRestStructureQuery structureQuery = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.None), null, structureReference, false);
                    string request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);
                    responseConceptScheme = this.SendQueryStructureRequest(request);
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

        public ISdmxObjects GetStructure(string DataflowId, string DatafloAgency, string DatafloVersion, bool resolseRef = false)
        {
            Logger.InfoFormat(
                    CultureInfo.InvariantCulture,
                    Resources.InfoGettingStructureFormat3,
                     DatafloAgency,
                    DataflowId,
                    DatafloVersion);
            ISdmxObjects structure = new SdmxObjectsImpl();
            #region Dataflow
            // Get dataflows
            ISdmxObjects responseDataflows = new SdmxObjectsImpl();
            string requestDataFlows = this.RetrieveDataflow(DataflowId,  DatafloAgency,  DatafloVersion);
            responseDataflows = this.SendQueryStructureRequest(requestDataFlows);
            // Remove from structure (ISdmxObjects) the DSDS built with SDMX v2.0
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
                responseDSD = this._nsiClientWs.SendQueryStructureRequest(sdmxV20onlyReferences, false);
                responseDataflows.Merge(responseDSD);
            }
            if (responseDataflows.Dataflows == null || responseDataflows.Dataflows.Count == 0)
                throw new Exception("Dataflow not found");
            #endregion

            try
            {
                IDataflowObject dataflow = responseDataflows.Dataflows.First();
                ISdmxObjects responseConceptScheme = new SdmxObjectsImpl();
                ISdmxObjects response = new SdmxObjectsImpl();

                IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
                IStructureQueryFactory factory = new RestStructureQueryFactory();
                IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);

                IDataStructureObject dsd = NsiClientHelper.GetDsdFromDataflow(dataflow, responseDataflows.DataStructures);
                structure.AddDataStructure(dsd);

                NsiClientValidation.CheckifStructureComplete(structure, dataflow);
                IEnumerable<IStructureReference> conceptRefs = NsiClientHelper.BuildConceptSchemeRequest(structure.DataStructures.First());
                foreach (var structureReference in conceptRefs)
                {
                    IRestStructureQuery structureQuery = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.None), null, structureReference, false);
                    string request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);
                    responseConceptScheme = this.SendQueryStructureRequest(request);
                    response.Merge(responseConceptScheme);
                }

                structure.Merge(responseDataflows);
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
        /// Sends the specified <paramref name="request"/> to the Web Service defined by <see cref="_config"/> 
        /// </summary>
        /// <param name="request">
        /// The <see cref="IComplexStructureQuery"/>
        /// </param>
        /// <returns>
        /// The ISdmxObjects returned by the Web Service
        /// </returns>
        private ISdmxObjects SendQueryStructureRequest(string request)
        {
            string tempFileName = Path.GetTempFileName();

            try
            {
                this.SendRequest(request, tempFileName, RequestType.Structure);

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
                // Delete the temporary file
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
        /// <param name="tempFileName">
        /// The temporary file name
        /// </param>
        /// <param name="requestType">
        /// The request type
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// request is null
        /// </exception>
        /// <exception cref="NsiClientException">
        /// Error in server response or communication
        /// </exception>
        private void SendRequest(string request, string tempFileName, RequestType requestType)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var endpointUri = new Uri(this._config.EndPoint);

            try
            {
                using (var writer = File.OpenWrite(tempFileName))
                {
                    var client = new RestClient(endpointUri.ToString());

                    var restRequest = new RestRequest(request, Method.GET);
                    restRequest.AddHeader("Accept-Encoding", "gzip, deflate");
                    if (requestType == RequestType.Data)
                    {
                        restRequest.AddHeader("Accept", "application/vnd.sdmx.structurespecificdata+xml;version=2.1");
                        if (Logger.IsDebugEnabled)
                        {
                            var buildUri = client.BuildUri(restRequest);
                            Logger.DebugFormat("Requesting URI : {0}", buildUri);
                        }
                    }

                    this.SetupRestAuth(client, restRequest);
                    restRequest.Timeout = 1800 * 1000;

                    // the lambda is executed inside the using
                    // ReSharper disable AccessToDisposedClosure
                    restRequest.ResponseWriter = (responseStream) => responseStream.CopyTo(writer);

                    // ReSharper restore AccessToDisposedClosure
                    var response = client.Execute(restRequest);

                    var httpStatusCode = response.StatusCode;
                    var responseStatus = response.ResponseStatus;
                    var responseUri = response.ResponseUri;
                    Logger.DebugFormat("HTTP status {0}, Response Status {1}, ResponseUri {2}", httpStatusCode, responseStatus, responseUri);

                    // we need to check the status if it is OK or not. No exceptions are thrown.
                    if (httpStatusCode == HttpStatusCode.NotFound || httpStatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new DataflowException(Resources.NoResultsFound);
                    }
                    if (httpStatusCode != HttpStatusCode.OK)
                    {
                        Logger.ErrorFormat("HTTP status {0}, Response Status {1}, ResponseUri {2}", httpStatusCode, responseStatus, responseUri);
                        Logger.ErrorFormat("ContentType {0}, Content Length: \n{1}", response.ContentType, response.ContentLength);
                        throw new NsiClientException(response.StatusDescription);
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
            finally
            {

            }

        }

        /// <summary>
        /// Sends the SDMX Query Request
        /// </summary>
        /// <param name="query">
        /// The query
        /// </param>
        /// <param name="tempFileName">
        /// The output stream
        /// </param>
        private void SendSdmxQuery(IDataQuery query, string tempFileName)
        {
            IDataQueryFormat<string> structureQueryFormat = new RestQueryFormat();
            IDataQueryFactory dataQueryFactory = new DataQueryFactory();
            IDataQueryBuilderManager dataQueryBuilderManager = new DataQueryBuilderManager(dataQueryFactory);
            string request = dataQueryBuilderManager.BuildDataQuery(query, structureQueryFormat);

            this.SendRequest(request, tempFileName, RequestType.Data);


        }

        /// <summary>
        /// Setup the specified <paramref name="client"/> network authentication and proxy configuration
        /// </summary>
        /// <param name="client">
        /// The <see cref="client"/>
        /// </param>
        public void SetupRestAuth(RestClient client, RestRequest restRequest)
        {
            if (this._config.EnableHTTPAuthentication)
            {
                restRequest.Credentials = new NetworkCredential(this._config.UserName, this._config.Password, this._config.Domain);
            }

            if (this._config.EnableProxy)
            {
                if (this._config.UseSystemProxy)
                {
                    client.Proxy = WebRequest.DefaultWebProxy;
                }
                else
                {
                    WebProxy proxy = new WebProxy(this._config.ProxyServer, this._config.ProxyServerPort);
                    if (!string.IsNullOrEmpty(this._config.ProxyUserName) || !string.IsNullOrEmpty(this._config.ProxyPassword))
                    {
                        proxy.Credentials = new NetworkCredential(this._config.ProxyUserName, this._config.ProxyPassword);
                    }

                    client.Proxy = proxy;
                }
            }
        }

        #endregion
    }
}
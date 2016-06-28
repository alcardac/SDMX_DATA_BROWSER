
namespace ISTAT.WebClient.WidgetComplements.Model.CallWS
{
    using System.IO;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
    using System.Collections.Generic;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;
    /// <summary>
    /// Defines the methods which must be implemented by a service class used for retrieving data from a GetSDMX instance.
    /// </summary>
    public interface IGetSDMX
    {
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
        /// <returns>
        /// The SDMX-ML data as a stream.
        /// </returns>
        void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, string tempFileName);

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
        /// <returns>
        /// The SDMX-ML data as a stream.
        /// </returns>
        void ExecuteQuery(IDataQuery query, SDMXWSFunction operationName, int observationLimit, string tempFileName);


        /// <summary>
        /// Get the SDMX Query Request
        /// </summary>
        /// <param name="query">
        /// The query
        /// </param>
        /// <param name="request">
        /// The output request
        /// </param>
        void GetSdmxQuery(IDataQuery query, out string request);


        /// <summary>
        /// Retrieves all available categorisations.
        /// </summary>
        /// <returns>
        ///   ISdmxObjects&amp;lt;/c&amp;gt; instance; the result won&amp;apos;t be &amp;lt;c&amp;gt;null&amp;lt;/c&amp;gt; if there are
        ///   no SdmxObject, instead an empty list will be returned
        /// </returns>
        ISdmxObjects RetrieveTree();

        /// <summary>
        /// Gets a bean with data about the codelist for specified dataflow and component.
        /// The dataflow can be retrieved from <see cref="RetrieveDataflow"/> and the component from <see cref="GetStructure"/>
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
        ICodelistObject GetCodelist(IDataflowObject dataflow, IDataStructureObject dsd, IComponent component, List<IContentConstraintMutableObject> criterias, bool Constrained);

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
        /// The maximum number of observations
        /// </returns>
        int GetDataflowDataCount(IDataflowObject dataflow, IContentConstraintMutableObject criteria);

        /// <summary>
        /// Retrieves all available categorisations.
        /// </summary>
        /// <returns>
        ///   ISdmxObjects&amp;lt;/c&amp;gt; instance; the result won&amp;apos;t be &amp;lt;c&amp;gt;null&amp;lt;/c&amp;gt; if there are
        ///   no SdmxObject, instead an empty list will be returned
        /// </returns>
        ISdmxObjects RetrieveCategorisations();

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
        /// The resulted bean will contain exactly one key family, but also will include any concepts <del>and codelists</del> referenced by the key family.
        /// </remarks>
        ISdmxObjects GetStructure(IDataflowObject dataflow, ISet<IDataStructureObject> dataStructures, bool resolveRef = false);


        ISdmxObjects GetStructure(string DataflowId, string DatafloAgency, string DatafloVersion,bool resolveRef=false);


        ISdmxObjects GetDsd(string DsdId, string DsdAgency, string DsdVersion, bool resolveRef = false);


        #endregion

        
    }
}
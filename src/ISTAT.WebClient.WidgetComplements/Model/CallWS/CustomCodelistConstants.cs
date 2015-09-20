
namespace ISTAT.WebClient.WidgetComplements.Model.CallWS
{
    using System;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;


    /// <summary>
    /// This class holds constants for custom codelists used in special requests 
    /// </summary>
    internal static class CustomCodelistConstants
    {
        #region Constants and Fields

        /// <summary>
        /// The custom dataflow data count codelist
        /// </summary>
        public const string CountCodeList = "CL_COUNT";

        /// <summary>
        /// The custom time dimension start/end codelist
        /// </summary>
        public const string TimePeriodCodeList = "CL_TIME_PERIOD";

        /// <summary>
        /// The Agency for custom codelists
        /// </summary>
        public const string Agency = "MA";

        /// <summary>
        /// The version used for custom codelists
        /// </summary>
        public const string Version = "1.0";

        #endregion

        #region Public Methods
       

        /// <summary>
        /// Check if the specified <c>CodeListBean</c> is the special COUNT codelist
        /// </summary>
        /// <param name="codelist">
        /// The <c>CodeListBean</c> object. It should have Id and Agency set. Version is ignored.
        /// </param>
        /// <returns>
        /// True if the  <c>CodeListBean</c> ID and Agency matches the custom <see cref="CountCodeList"/> and <see cref="Agency"/>. Else false
        /// </returns>
        public static bool IsCountCodeList(ICodelistObject codelist)
        {
            return CountCodeList.Equals(codelist.Id, StringComparison.OrdinalIgnoreCase)
                   && Agency.Equals(codelist.AgencyId, StringComparison.OrdinalIgnoreCase)
                   && codelist.Items.Count == 1;
        }

        
        #endregion
    }
}
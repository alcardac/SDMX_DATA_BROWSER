// -----------------------------------------------------------------------
// <copyright file="CustomCodelistConstants.cs" company="EUROSTAT">
//   Date Created : 2011-03-10
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
/*
        /// <summary>
        /// Check if the specified <c>CodelistRefBean</c> is for the special COUNT request
        /// </summary>
        /// <param name="codelistRef">
        /// The <c>CodelistRefBean</c> object. It should have Id and Agency set. Version is ignored.
        /// </param>
        /// <returns>
        /// True if the  <c>CodelistRefBean</c> ID and Agency matches the custom <see cref="CountCodeList"/> and <see cref="Agency"/>. Else false
        /// </returns>
        public static bool IsCountRequest(CodelistRefBean codelistRef)
        {
            return CountCodeList.Equals(codelistRef.Id, StringComparison.InvariantCultureIgnoreCase)
                   && Agency.Equals(codelistRef.AgencyID, StringComparison.InvariantCultureIgnoreCase);
        }

*/

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

/*
        /// <summary>
        /// Check if the specified <c>CodeListBean</c> is the special Time Dimension codelist
        /// </summary>
        /// <param name="codelist">
        /// The <c>CodeListBean</c> object. It should have Id and Agency set. Version is ignored. 
        /// </param>
        /// <returns>
        /// True if the  <c>CodeListBean</c> ID and Agency matches the custom <see cref="TimePeriodCodeList"/> and <see cref="Agency"/>. Else false
        /// </returns>
        public static bool IsTimeDimensionCodeList(CodeListBean codelist)
        {
            return TimePeriodCodeList.Equals(codelist.Id, StringComparison.OrdinalIgnoreCase)
                   && Agency.Equals(codelist.AgencyId, StringComparison.OrdinalIgnoreCase)
                   && codelist.Items.Count > 0 && codelist.Items.Count < 3;
        }
*/
        /*
        /// <summary>
        /// Check if the specified <c>CodelistRefBean</c> is for the special Time Dimension request
        /// </summary>
        /// <param name="codelistRef">
        /// The <c>CodelistRefBean</c> object. It should have Id and Agency set. Version is ignored. 
        /// </param>
        /// <returns>
        /// True if the  <c>CodelistRefBean</c> ID and Agency matches the custom <see cref="TimePeriodCodeList"/> and <see cref="Agency"/>. Else false
        /// </returns>
        public static bool IsTimeDimensionRequest(CodelistRefBean codelistRef)
        {
            return TimePeriodCodeList.Equals(codelistRef.Id, StringComparison.OrdinalIgnoreCase)
                   && Agency.Equals(codelistRef.AgencyID, StringComparison.OrdinalIgnoreCase);
        }
        */
        #endregion
    }
}
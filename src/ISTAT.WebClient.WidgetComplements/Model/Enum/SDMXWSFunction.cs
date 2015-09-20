// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SDMXWSFunction.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This enum contains the names of  a subset of the web functions as defined in  <a href="http://sdmx.org/docs/2_0/SDMX_2_0%20SECTION_07_WebServicesGuidelines.pdf">SDMX v2.0 Section 07 Web Services Guidelines</a>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Enum
{
    /// <summary>
    /// This enum contains the names of  a subset of the web functions as defined in  <a href="http://sdmx.org/docs/2_0/SDMX_2_0%20SECTION_07_WebServicesGuidelines.pdf">SDMX v2.0 Section 07 Web Services Guidelines</a>
    /// </summary>
    public enum SDMXWSFunction
    {
        /// <summary>
        /// Obtain Compact Data: This is an exchange invoked by the Query Message, for
        /// which the response is data marked up according to the Compact Data Message.
        /// The function should be called “GetCompactData(Query)”.
        /// </summary>        
        GetCompactData,

        /// <summary>
        /// Obtain Cross-Sectional Data: This is an exchange invoked by the Query
        /// Message, for which the response is data marked up according to the Cross-
        /// Sectional Data Message. The function should be called
        /// “GetCrossSectionalData(Query)”.
        /// </summary> 
        GetCrossSectionalData,

        /// <summary>
        /// Query Structural Metadata in Repository: This is an exchange invoked by the
        /// QueryStructureRequest message, for which the response is a confirmation in the
        /// form of a QueryStructureResponse message. The function should be called
        /// “QueryStructure(QueryStructureRequest).”
        /// </summary> 
        QueryStructure  ,

        /// <summary>
        /// Usata nel richiedere dsd a ws DOTSTAT”
        /// </summary>
        GetDataStructureDefinition
    }
}
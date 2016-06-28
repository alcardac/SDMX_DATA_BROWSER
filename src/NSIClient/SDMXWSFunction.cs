// -----------------------------------------------------------------------
// <copyright file="SDMXWSFunction.cs" company="EUROSTAT">
//   Date Created : 2010-11-02
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
        QueryStructure
    }
}
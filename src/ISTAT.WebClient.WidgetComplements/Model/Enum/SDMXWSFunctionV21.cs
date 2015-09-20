// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SDMXWSFunctionV21.cs" company="Eurostat">
//   Date Created : 2013-08-19
//   Copyright (c) 2013 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This enum contains the names of  a subset of the web functions as defined in  <a href="http://sdmx.org/wp-content/uploads/2013/07/SDMX_2_1-SECTION_07_WebServicesGuidelines_2013-041.pdf">SDMX v2.1 Section 03 SOAP-Based SDMX WebServices: WSDL Operations and Behaviours</a>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Enum
{
    /// <summary>
    /// This enum contains the names of  a subset of the web functions as defined in  <a href="http://sdmx.org/wp-content/uploads/2013/07/SDMX_2_1-SECTION_07_WebServicesGuidelines_2013-041.pdf">SDMX v2.1 Section 03 SOAP-Based SDMX WebServices: WSDL Operations and Behaviours</a>
    /// </summary>
    public enum SDMXWSFunctionV21
    {

        GetCategorisation,

        GetCategory,

        GetDataStructure,

        GetCategoryScheme,

        GetCodelist,

        GetConceptScheme,

        GetDataflow,

        /// <summary>
        /// Obtain Compact Data or CrossSectional Data: This is an exchange invoked by the 
        /// Complex Data Query Message, for which the response is data marked up according 
        /// to the Compact Data Message.
        /// The function should be called "GetStructureSpecificData(ComplexDataQuery)".
        /// </summary>
        GetStructureSpecificData

    }
}
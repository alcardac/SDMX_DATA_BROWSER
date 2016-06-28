// -----------------------------------------------------------------------
// <copyright file="SDMXWSFunctionV21.cs" company="EUROSTAT">
//   Date Created : 2013-08-20
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
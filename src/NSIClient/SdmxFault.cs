// -----------------------------------------------------------------------
// <copyright file="SdmxFault.cs" company="EUROSTAT">
//   Date Created : 2013-12-06
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
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// Sdmx Fault
    /// </summary>
    public class SdmxFault
    {

        private readonly string _errorMessage;
        private readonly int _errorNumber;
    
        /// <summary>
        /// Gets or sets the error number
        /// </summary>
        public int ErrorNumber
        {
            get
            {
                return this._errorNumber;
            }
        }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this._errorMessage;
            }
        }

        /// <summary>
        /// The sdmx fault
        /// </summary>
        public SdmxFault(int errorNumber, string errorMessage)
        {
           this._errorNumber = errorNumber;
           this._errorMessage = errorMessage;
        }

        /// <summary>
        /// Get error number
        /// </summary>
        public static SdmxFault GetErrorNumber(XmlDocument fault)
        {
          string errorMessage="";
          int errorNumberV = 0;
          XmlNodeList errorNumberElement = fault.GetElementsByTagName("ErrorNumber");
          if (errorNumberElement.Count > 0)
          {
              var value = errorNumberElement[0].InnerXml;
              if (value == null ||
                  !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out errorNumberV))
              {
                  errorNumberV = 0;
              }
          }
          XmlNodeList element = fault.GetElementsByTagName("ErrorMessage");
          if (element.Count > 0 && element[0].InnerXml != null)
          {
              errorMessage = element[0].InnerXml;
          }

          return new SdmxFault(errorNumberV, errorMessage);
      }
   }
}

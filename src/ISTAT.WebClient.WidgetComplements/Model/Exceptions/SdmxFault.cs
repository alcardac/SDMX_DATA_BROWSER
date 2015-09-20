// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SdmxFault.cs" company="Eurostat">
//   Date Created : 2013-12-06
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Sdmx fault &lt;SdmxFault&gt;
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace ISTAT.WebClient.WidgetComplements.Model.Exceptions
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
            string errorMessage = "";
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

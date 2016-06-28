// -----------------------------------------------------------------------
// <copyright file="SoapConstants.cs" company="EUROSTAT">
//   Date Created : 2011-09-28
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
    /// A list of SOAP related constants
    /// </summary>
    internal static class SoapConstants
    {
        /// <summary>
        /// SOAP 1.1 namespace
        /// </summary>
        internal const string Soap11Ns = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// This field holds a template for soap 1.1 request envelope
        /// </summary>
        internal const string SoapRequest =
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:{0}=\"{1}\"><soap:Header/><soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        /// SOAP Body tag
        /// </summary>
        internal const string Body = "Body";

        /// <summary>
        /// SOAPAction HTTP header
        /// </summary>
        internal const string SoapAction = "SOAPAction";
    }
}
// -----------------------------------------------------------------------
// <copyright file="HttpConstants.cs" company="EUROSTAT">
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
    /// This class contains a collection of HTTP related constants
    /// </summary>
    internal static class HttpConstants
    {
        /// <summary>
        /// HTTP Header content type
        /// </summary>
        public const string Content = "text/xml;charset=\"utf-8\"";

        /// <summary>
        /// HTTP POST
        /// </summary>
        public const string Post = "POST";

        /// <summary>
        /// HTTP GET
        /// </summary>
        public const string Get = "GET";
    }
}
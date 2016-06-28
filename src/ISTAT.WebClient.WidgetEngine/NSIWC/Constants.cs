// -----------------------------------------------------------------------
// <copyright file="Constants.cs" company="EUROSTAT">
//   Date Created : 2010-11-11
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
namespace ISTAT.WebClient.WidgetEngine.NSIWC 
{
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Defines various constants used by all other application's classes.
    /// </summary>
    internal static class Constants
    {
        #region Constants and Fields

        /// <summary>
        /// Fallback language
        /// </summary>
        public const string FallbackLang = "en";

        /// <summary>
        /// The name of the attributed used for saving the NSIClient instance into HTTP sessions
        /// </summary>
        public const string HTTPNsiclientAttr = "NSIClient";

        /// <summary>
        ///  The name of the attribute used for saving the query (a <code>SessionQuery</code> instance) into HTTP sessions.
        /// </summary>
        /// <remarks>
        /// <p>Note that the value should not be changed, as it is hard-coded into some ASP pages and .NET code too.</p>
        /// </remarks>
        public const string HTTPSessionQueryAttr = "sessionQuery";

        /// <summary>
        /// The maximum observation config attribute
        /// </summary>
        public const string MaxObs = "max_observations";

        #endregion

        /// <summary>
        /// default size of internal buffer of streams is 4096 bytes.
        /// </summary>
        public const int BufSize = 4096;

        /// <summary>
        /// The attachment filename format used in Content-Disposition HTTP header
        /// </summary>
        public const string AttachmentFilenameFormat = "attachment;filename=\"{0}\"";

        /// <summary>
        /// The Content-Disposition HTTP header
        /// </summary>
        public const string ContentDispositionHttpHeader = "Content-Disposition";

        /// <summary>
        /// Sqlite connection string format
        /// </summary>
        public const string FileDBSettingsFormat = "Data Source={0};Version=3;New=true;";

        /// <summary>
        /// Sqlite database provider
        /// </summary>
        public const string SystemDataSqlite = "System.Data.SQLite";

        /// <summary>
        /// Connection string name. It can be almost anything.
        /// </summary>
        public const string ConnectionStringSettingsName = "temp";

        /// <summary>
        /// Database file name format
        /// </summary>
        public const string FileDBFileNameFormat = "{0}-{1}.sqlite";

        /// <summary>
        /// In memory database connection string settings
        /// </summary>
        public static readonly ConnectionStringSettings InMemoryDBSettings = new ConnectionStringSettings(
            ConnectionStringSettingsName, string.Format(CultureInfo.InvariantCulture, FileDBSettingsFormat, ":memory:"), SystemDataSqlite);
         
         
        /// <summary>
        ///Start date description.
        /// </summary>
        public static string StartDateDescription = "Start Date";
    
  
        /// <summary>
        ///End date description.
        /// </summary>
        public static string EndDateDescription = "End Date";
    }
}
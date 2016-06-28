// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Defines various constants used by all other application's classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model
{
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Defines various constants used by all other application's classes.
    /// </summary>
    public static class Constants
    {
        #region Constants and Fields

        /// <summary>
        /// Service Namespace
        /// </summary>
        public const string ServiceNamespace="http://sdmx.istat.it/webclient";
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
      

        ///// <summary>
        ///// In memory database connection string settings
        ///// </summary>
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
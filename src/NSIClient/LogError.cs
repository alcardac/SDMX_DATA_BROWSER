// -----------------------------------------------------------------------
// <copyright file="LogError.cs" company="EUROSTAT">
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
    using System;

    /// <summary>
    /// Helper class to log errors.
    /// TODO move this to logger
    /// </summary>
    public class LogError
    {
        #region Constants and Fields

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly LogError _instance = new LogError();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="LogError"/> class from being created. 
        /// </summary>
        private LogError()
        {
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Triggered when an log error event occurs.
        /// </summary>
        public event EventHandler<LogErrorEventArgs> LogErrorEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Singleton instance
        /// </summary>
        public static LogError Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raise a ErrorEvent
        /// </summary>
        /// <param name="e">
        /// The log error args
        /// </param>
        public void OnLogErrorEvent(LogErrorEventArgs e)
        {
            if (this.LogErrorEvent != null)
            {
                this.LogErrorEvent(this, e);
            }
        }

        /// <summary>
        /// Raise a ErrorEvent
        /// </summary>
        /// <param name="message">
        /// The log error args
        /// </param>
        public void OnLogErrorEvent(string message)
        {
            this.OnLogErrorEvent(new LogErrorEventArgs(message));
        }

        #endregion
    }
}
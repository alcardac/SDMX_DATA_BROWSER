// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogError.cs" company="Eurostat">
//   Date Created : 2011-03-04
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Helper class to log errors.
//   TODO move this to logger
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Log
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
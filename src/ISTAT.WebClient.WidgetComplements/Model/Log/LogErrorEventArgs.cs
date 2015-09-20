// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogErrorEventArgs.cs" company="Eurostat">
//   Date Created : 2011-03-04
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Log event arguments
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Log
{
    using System;

    /// <summary>
    /// Log event arguments
    /// </summary>
    public class LogErrorEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        /// The message to log
        /// </summary>
        private readonly string _message;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogErrorEventArgs"/> class. 
        /// Initialize a new instance of the LogErrorEventArgs
        /// </summary>
        /// <param name="message">
        /// The message to log
        /// </param>
        public LogErrorEventArgs(string message)
        {
            this._message = message;
        }

        #endregion

        /// <summary>
        /// Gets the message to log
        /// </summary>
        public string Message
        {
            get
            {
                return this._message;
            }
        }
    }
}
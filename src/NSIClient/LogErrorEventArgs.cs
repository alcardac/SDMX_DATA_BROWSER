// -----------------------------------------------------------------------
// <copyright file="LogErrorEventArgs.cs" company="EUROSTAT">
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetRendererException.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Description of DataSetRendererException.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Description of DataSetRendererException.
    /// </summary>
    [Serializable]
    public class DataSetRendererException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetRendererException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The error message
        /// </param>
        public DataSetRendererException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetRendererException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The error message
        /// </param>
        /// <param name="innerException">
        /// The original exception
        /// </param>
        public DataSetRendererException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetRendererException"/> class.
        /// </summary>
        public DataSetRendererException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetRendererException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null. 
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). 
        /// </exception>
        protected DataSetRendererException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
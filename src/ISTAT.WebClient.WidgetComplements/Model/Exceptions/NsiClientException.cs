// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NsiClientException.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   The exception used by a NSI client implementations for signaling errors when something goes wrong.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception used by a NSI client implementations for signaling errors when something goes wrong.
    /// </summary>
    [Serializable]
    public class NsiClientException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the NsiClientException class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public NsiClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NsiClientException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public NsiClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NsiClientException"/> class.
        /// </summary>
        public NsiClientException()
        {
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NsiClientException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        /// </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. 
        /// </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. 
        /// </exception><exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). 
        /// </exception>
        protected NsiClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
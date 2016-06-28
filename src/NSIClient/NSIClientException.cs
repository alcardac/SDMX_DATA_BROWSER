// -----------------------------------------------------------------------
// <copyright file="NSIClientException.cs" company="EUROSTAT">
//   Date Created : 2010-11-02
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
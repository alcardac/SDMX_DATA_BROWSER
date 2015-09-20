// -----------------------------------------------------------------------
// <copyright file="DataflowException.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DataflowException : Exception
    {
        public DataflowException(string message)
            : this(message, null)
        {

        }

        public DataflowException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}

namespace ISTAT.WebClient.WidgetComplements.Model.DataRenderNSI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Component value collection
    /// </summary>
    [Serializable]
    public class ComponentValueDictionary : Dictionary<string, List<string>>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentValueDictionary"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public ComponentValueDictionary()
            : base(StringComparer.Ordinal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentValueDictionary"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </param>
        /// <param name="context">
        /// A <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </param>
        protected ComponentValueDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
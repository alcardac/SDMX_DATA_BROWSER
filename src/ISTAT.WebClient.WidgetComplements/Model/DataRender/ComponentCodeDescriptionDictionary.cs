namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The component code description dictionary.
    /// </summary>
    [Serializable]
    public class ComponentCodeDescriptionDictionary : Dictionary<string, Dictionary<string, string>>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCodeDescriptionDictionary"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public ComponentCodeDescriptionDictionary()
            : base(StringComparer.Ordinal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCodeDescriptionDictionary"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </param>
        /// <param name="context">
        /// A <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </param>
        protected ComponentCodeDescriptionDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
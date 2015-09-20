namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A <see cref="HtmlEntity"/> collection
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="HtmlEntity"/> based class
    /// </typeparam>
    public class HtmlEntityCollection<T> : Collection<T>
        where T : HtmlEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEntityCollection{T}"/> class.
        /// </summary>
        public HtmlEntityCollection()
            : base(new List<T>())
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sorts the elements in the <see cref="HtmlEntityCollection{T}"/>
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing elements, or nullNothingnullptra null reference (Nothing in Visual Basic) to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        public void Sort(IComparer<T> comparer)
        {
            var entities = this.Items as List<T>;
            if (entities != null)
            {
                entities.Sort(comparer);
            }
        }

        #endregion
    }
}
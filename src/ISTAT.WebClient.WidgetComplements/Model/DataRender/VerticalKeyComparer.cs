namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System.Collections.Generic;

    /// <summary>
    /// The vertical key comparer.
    /// </summary>
    public class VerticalKeyComparer : IComparer<TableRow>
    {
        #region Constants and Fields

        /// <summary>
        /// The _vertical key count.
        /// </summary>
        private readonly int _verticalKeyCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalKeyComparer"/> class.
        /// </summary>
        /// <param name="verticalKeyCount">
        /// The vertical key count.
        /// </param>
        public VerticalKeyComparer(int verticalKeyCount)
        {
            this._verticalKeyCount = verticalKeyCount;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// Value 
        ///                     Condition 
        ///                     Less than zero
        ///                 <paramref name="firstRow"/> is less than <paramref name="secondRow"/>.
        ///                     Zero
        ///                 <paramref name="firstRow"/> equals <paramref name="secondRow"/>.
        ///                     Greater than zero
        ///                 <paramref name="firstRow"/> is greater than <paramref name="secondRow"/>.
        /// </returns>
        /// <param name="firstRow">
        /// The first object to compare.
        /// </param>
        /// <param name="secondRow">
        /// The second object to compare.
        /// </param>
        public int Compare(TableRow firstRow, TableRow secondRow)
        {
            if (firstRow == null && secondRow == null)
            {
                return 0;
            }

            if (firstRow == null)
            {
                return -1;
            }

            if (secondRow == null)
            {
                return 1;
            }

            if (firstRow.Children.Count < this._verticalKeyCount && secondRow.Children.Count < this._verticalKeyCount)
            {
                return 0;
            }

            if (firstRow.Children.Count < this._verticalKeyCount)
            {
                return -1;
            }

            if (secondRow.Children.Count < this._verticalKeyCount)
            {
                return 1;
            }

            int ret = 0;
            for (int i = 0; i < this._verticalKeyCount && ret == 0; i++)
            {

                var firstValue = firstRow.Children[i].Text;
                if (string.IsNullOrEmpty(firstValue)) firstValue = firstRow.Children[i].Children[0].Children[0].Children[0].Text;
                var secondValue = secondRow.Children[i].Text;
                if (string.IsNullOrEmpty(secondValue)) secondValue = secondRow.Children[i].Children[0].Children[0].Children[0].Text;

                ret = string.CompareOrdinal(firstValue, secondValue);
            }

            return ret;
        }

        #endregion
    }
}
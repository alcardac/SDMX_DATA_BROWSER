namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System.IO;

    /// <summary>
    /// This class represents a HTML TR (Row)
    /// </summary>
    public class TableRow : HtmlEntityParent<TableCell>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableRow"/> class. 
        /// Initialize a new instance of the TR class
        /// </summary>
        public TableRow()
            : base(HtmlConstants.TableRowTag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableRow"/> class. 
        /// Initialize a new instance of the TR class
        /// </summary>
        /// <param name="child">
        /// The child to add to the TD instance
        /// </param>
        public TableRow(TableCell child)
            : base(HtmlConstants.TableRowTag, child)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of columns by taking in consideration the colspan as well.
        /// </summary>
        /// <value>The cell count.</value>
        public int CellCount
        {
            get
            {
                int ret = 0;
                foreach (TableCell tableCell in this.Children)
                {
                    if (tableCell != null)
                    {
                        ret += tableCell.ColumnSpan;
                    }
                }

                return ret;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add the specified cell to the specified position.
        /// If a cell at this position exists it will be replaced.
        /// Else any cell missing in between then a new TD with space are added
        /// </summary>
        /// <param name="tableCell">
        /// The TD to add
        /// </param>
        /// <param name="columnIndex">
        /// Zero based index of the TD to add
        /// </param>
        /// <returns>
        /// this instance
        /// </returns>
        public TableRow AddAt(TableCell tableCell, int columnIndex)
        {
            for (int i = this.Children.Count; i <= columnIndex; i++)
            {
                this.AddElement(new TableCell(" "));
            }

            this.Children[columnIndex] = tableCell;

            return this;
        }

        /// <summary>
        /// Add a <paramref name="cell"/>
        /// </summary>
        /// <param name="cell">
        /// The cell.
        /// </param>
        public void AddCell(TableCell cell)
        {
            this.Children.Add(cell);
        }

        /// <summary>
        /// Gets the cell at <paramref name="index"/>
        /// </summary>
        /// <param name="index">
        /// The cell index
        /// </param>
        /// <returns>
        /// The cell at <paramref name="index"/>; otherwise null
        /// </returns>
        public TableCell GetCell(int index)
        {
            return this.Children[index];
        }

        /// <summary>
        /// Write the TR to unformatted layout based CSV
        /// </summary>
        /// <param name="writer">
        /// The output
        /// </param>
        /// <param name="separator">
        /// The CSV separator.
        /// </param>
        /// <param name="rowSpanCols">
        /// The row Span Cols.
        /// </param>
        public void WriteCsv(TextWriter writer, string separator, int[] rowSpanCols)
        {
            if (this.Children.Count == 0 || this.Children[0] == null)
            {
                writer.WriteLine();
                return;
            }

            int i = 0;
            int lastCell = this.Children.Count - 1;
            for (int x = 0, j = rowSpanCols.Length; x < j && i <= lastCell; x++)
            {
                if (rowSpanCols[x] > 1)
                {
                    rowSpanCols[x]--;
                    TableCell.Empty.WriteCsv(writer, i == lastCell ? string.Empty : separator);
                }
                else
                {
                    TableCell cell;
                    if (i < lastCell)
                    {
                        cell = this.Children[i];
                        if (cell != null)
                        {
                            rowSpanCols[x] = cell.RowSpan;
                            cell.WriteCsv(writer, separator);
                            for (int k = 1; k < cell.ColumnSpan; k++)
                            {
                                TableCell.Empty.WriteCsv(writer, separator);
                            }
                        }

                        i++;
                    }
                    else if (i == lastCell)
                    {
                        cell = this.Children[i];
                        if (cell != null)
                        {
                            rowSpanCols[x] = cell.RowSpan;
                            cell.WriteCsv(writer, string.Empty);
                        }

                        i++;
                    }
                }
            }

            writer.WriteLine();
        }

        #endregion
    }
}
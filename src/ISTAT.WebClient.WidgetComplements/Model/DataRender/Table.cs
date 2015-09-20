namespace ISTAT.WebClient.WidgetComplements.Model.DataRender
{
    using System.IO;

    /// <summary>
    /// This class represents a table entity 
    /// </summary>
    public class Table : HtmlEntityParent<TableRow>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class. 
        /// Initialize a new instance of the Table class
        /// </summary>
        public Table()
            : base(HtmlConstants.TableTag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class. 
        /// Initialize a new instance of the Table class with the specified child
        /// </summary>
        /// <param name="child">
        /// The child to add to the Table instance
        /// </param>
        public Table(TableRow child)
            : base(HtmlConstants.TableTag, child)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add the <paramref name="row"/> to <see cref="HtmlEntityParent{T}.Children"/>
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public void AddRow(TableRow row)
        {
            this.Children.Add(row);
        }

        /// <summary>
        /// Add the <paramref name="table"/> to the cell at row position <paramref name="row"/> and cell position <paramref name="cell"/>
        /// </summary>
        /// <param name="row">
        /// The row index.
        /// </param>
        /// <param name="cell">
        /// The cell index.
        /// </param>
        /// <param name="table">
        /// The table to add.
        /// </param>
        public void AddTable(int row, int cell, Table table)
        {
            this.GetRow(row).GetCell(cell).AddElement(table);
        }

        /// <summary>
        /// Gets the row at <paramref name="index"/>
        /// </summary>
        /// <param name="index">
        /// The row index
        /// </param>
        /// <returns>
        /// The row at <paramref name="index"/>; otherwise null
        /// </returns>
        public TableRow GetRow(int index)
        {
            return this.Children[index];
        }

        /// <summary>
        /// Write the table and it's children in text format separated by the specified separator
        /// </summary>
        /// <param name="writer">
        /// The output TextWriter
        /// </param>
        /// <param name="separator">
        /// The CSV separator
        /// </param>
        public void WriteCsv(TextWriter writer, string separator)
        {
            if (this.Children.Count == 0)
            {
                return;
            }

            var firstRow = this.Children[0];

            if (firstRow != null)
            {
                var rowSpanCols = new int[firstRow.CellCount];
                foreach (TableRow tableRow in this.Children)
                {
                    if (tableRow != null)
                    {
                        tableRow.WriteCsv(writer, separator, rowSpanCols);
                    }
                }
            }
        }

        /// <summary>
        /// Write the table and it's TR children in text format separated by the specified separator
        /// It will add a new line and flush the output
        /// </summary>
        /// <param name="writer">
        /// The output TextWriter
        /// </param>
        /// <param name="separator">
        /// The CSV separator
        /// </param>
        public void WriteLineCsv(TextWriter writer, string separator)
        {
            this.WriteCsv(writer, separator);
            writer.WriteLine();
            writer.Flush();
        }

        #endregion
    }
}
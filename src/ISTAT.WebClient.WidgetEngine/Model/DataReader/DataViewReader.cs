// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewReader.cs" company="Eurostat">
//   Date Created : 2011-09-26
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   A System.Data.DataView IDataReader
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetEngine.Model.DataReader
{
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    /// A System.Data.DataView IDataReader
    /// </summary>
    /// <remarks>
    /// Taken as it is from http://weblogs.sqlteam.com/jeffs/archive/2008/02/28/DataViewReader.aspx
    /// </remarks>
    internal sealed class DataViewReader : IDataReader
    {
        #region Constants and Fields

        /// <summary>
        /// The <see cref="DataView"/> source
        /// </summary>
        private readonly DataView _source;

        /// <summary>
        /// A value indicating if it is closed.
        /// </summary>
        private bool _closed = true;

        /// <summary>
        /// The enumerator instance.
        /// </summary>
        private IEnumerator _enum;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewReader"/> class. 
        /// </summary>
        /// <param name="source">
        /// The source <see cref="DataView"/>
        /// </param>
        public DataViewReader(DataView source)
        {
            this._source = source;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <returns>
        /// The level of nesting.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Depth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <returns>
        /// When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int FieldCount
        {
            get
            {
                return this._source.Table.Columns.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <returns>
        /// true if the data reader is closed; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsClosed
        {
            get
            {
                return this._closed;
            }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <returns>
        /// The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int RecordsAffected
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current DataRowView
        /// </summary>
        private DataRowView Current
        {
            get
            {
                return (DataRowView)this._enum.Current;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <returns>
        /// The column with the specified name as an <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="name">The name of the column to find. 
        /// </param><exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. 
        /// </exception><filterpriority>2</filterpriority>
        public object this[string name]
        {
            get
            {
                return this.Current[name];
            }
        }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <returns>
        /// The column located at the specified index as an <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="i">The zero-based index of the column to get. 
        /// </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception><filterpriority>2</filterpriority>
        public object this[int i]
        {
            get
            {
                return this.Current[i];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader"/> Object.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Close()
        {
            this._closed = true;
            return;
        }

        /// <summary>
        /// Does nothing in this implementation
        /// </summary>
        public void Dispose()
        {
            // _source.Dispose();
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <returns>
        /// The value of the column.
        /// </returns>
        /// <param name="i">
        /// The zero-based column ordinal. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public bool GetBoolean(int i)
        {
            return (bool)this.Current[i];
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <param name="i">
        /// The zero-based column ordinal. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public byte GetByte(int i)
        {
            return (byte)this.Current[i];
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <returns>
        /// The actual number of bytes read.
        /// </returns>
        /// <param name="i">
        /// The zero-based column ordinal. 
        /// </param>
        /// <param name="fieldOffset">
        /// The index within the field from which to start the read operation. 
        /// </param>
        /// <param name="buffer">
        /// The buffer into which to read the stream of bytes. 
        /// </param>
        /// <param name="bufferoffset">
        /// The index for <paramref name="buffer"/> to start the read operation. 
        /// </param>
        /// <param name="length">
        /// The number of bytes to read. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <param name="i">
        /// The zero-based column ordinal. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public char GetChar(int i)
        {
            return (char)this.Current[i];
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <returns>
        /// The actual number of characters read.
        /// </returns>
        /// <param name="i">
        /// The zero-based column ordinal. 
        /// </param>
        /// <param name="fieldoffset">
        /// The index within the row from which to start the read operation. 
        /// </param>
        /// <param name="buffer">
        /// The buffer into which to read the stream of bytes. 
        /// </param>
        /// <param name="bufferoffset">
        /// The index for <paramref name="buffer"/> to start the read operation. 
        /// </param>
        /// <param name="length">
        /// The number of bytes to read. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/>.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public string GetDataTypeName(int i)
        {
            return this.GetFieldType(i).Name;
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public DateTime GetDateTime(int i)
        {
            return (DateTime)this.Current[i];
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public decimal GetDecimal(int i)
        {
            return (decimal)this.Current[i];
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public double GetDouble(int i)
        {
            return (double)this.Current[i];
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetFieldType(int i)
        {
            return this._source.Table.Columns[i].DataType;
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        ///                 </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public float GetFloat(int i)
        {
            return (float)this.Current[i];
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <returns>
        /// The GUID value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        ///                 </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public Guid GetGuid(int i)
        {
            return (Guid)this.Current[i];
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        ///                 </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public short GetInt16(int i)
        {
            return (short)this.Current[i];
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        ///                 </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public int GetInt32(int i)
        {
            return (int)this.Current[i];
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        ///                 </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public long GetInt64(int i)
        {
            return (long)this.Current[i];
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        ///                 </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public string GetName(int i)
        {
            return this._source.Table.Columns[i].ColumnName;
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <returns>
        /// The index of the named field.
        /// </returns>
        /// <param name="name">
        /// The name of the field to find. 
        ///                 </param>
        /// <filterpriority>2</filterpriority>
        public int GetOrdinal(string name)
        {
            return this._source.Table.Columns[name].Ordinal;
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.IDataReader"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The <see cref="T:System.Data.IDataReader"/> is closed. 
        ///                 </exception>
        /// <filterpriority>2</filterpriority>
        public DataTable GetSchemaTable()
        {
            return this._source.Table;
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <returns>
        /// The string value of the specified field.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public string GetString(int i)
        {
            return (string)this.Current[i];
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public object GetValue(int i)
        {
            return this.Current[i];
        }

        /// <summary>
        /// Gets all the attribute fields in the collection for the current record.
        /// </summary>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object"/> in the array.
        /// </returns>
        /// <param name="values">
        /// An array of <see cref="T:System.Object"/> to copy the attribute fields into. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, this.FieldCount);

            for (int i = 0; i < count; i++)
            {
                values[i] = this.Current[i];
            }

            return count;
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <param name="i">
        /// The index of the field to find. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public bool IsDBNull(int i)
        {
            return this.Current[i] == DBNull.Value;
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool NextResult()
        {
            return false;
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool Read()
        {
            if (this._closed)
            {
                this._enum = this._source.GetEnumerator();
            }

            this._closed = false;
            return this._enum.MoveNext();
        }

        #endregion
    }
}
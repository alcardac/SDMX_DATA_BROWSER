// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetStoreDataView.cs" company="Eurostat">
//   Date Created : 2011-04-05
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This is an implementation if the <see cref="IDataSetStore" />  that stores the SDMX-ML dataset inside a <see cref="DataTable" /> and a <see cref="DataView" />.
//   The <see cref="DataView" /> is used to get filtered and/or sorted data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.DataReaderNSI
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Xml;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using ISTAT.WebClient.WidgetComplements.NSIWC;

    
    /// <summary>
    /// This is an implementation if the <see cref="IDataSetStore"/>  that stores the SDMX-ML dataset inside a <see cref="DataTable"/> and a <see cref="DataView"/>.
    /// The <see cref="DataView"/> is used to get filtered and/or sorted data.
    /// </summary>
    /// <remarks>
    /// This is implementation is not recommeded for big SDMX-ML datasets (&gt; 5-10 thousand observations) as <see cref="DataTable"/> and <see cref="DataView"/> consume a lot of memory and are very slow
    /// </remarks>
    public class DataSetStoreDataView : IDataSetStore
    {
        #region Constants and Fields

        /// <summary>
        /// The size of each varchar
        /// </summary>
        private const int VarCharSize = 1000;

        /// <summary>
        /// The _log.
        /// </summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof(DataSetStoreDB));

        /// <summary>
        /// Holds the cache database table name.
        /// </summary>
        private readonly string _cacheTableName;

        /// <summary>
        /// The available columns
        /// </summary>
        private readonly Dictionary<string, object> _columns = new Dictionary<string, object>();

        /// <summary>
        /// Holds database info instance
        /// </summary>
        private readonly DBInfo _dbInfo;

        /// <summary>
        /// The ordered list of keys that will be used in order by
        /// </summary>
        private readonly List<string> _sort = new List<string>();

        /// <summary>
        /// The database specific type of "varchar".
        /// </summary>
        private readonly string _varChar = "varchar";

        /// <summary>
        /// The current command
        /// </summary>
        private DbCommand _cmd;

        /// <summary>
        /// The criteria in a map of key and value
        /// </summary>
        private List<DataCriteria> _criteria = new List<DataCriteria>();

        /// <summary>
        /// The concept id of the measure dimension
        /// </summary>
        private string _measureColumn;

        private IDataStructureObject kf;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetStoreDB"/> class
        /// </summary>
        /// <param name="dbInfo">
        /// The database information which will used to connect
        /// </param>
        /// <param name="cacheTableName">
        /// The cache database table name
        /// </param>
        /// <param name="keyFamily">
        /// The SDMX keyfamily bean
        /// </param>
        public DataSetStoreDataView(DBInfo dbInfo, string cacheTableName, IDataStructureObject keyFamily, bool CreateTable, bool useAttr)
        {
            if (dbInfo == null)
            {
                throw new ArgumentNullException("dbInfo");
            }

            if (string.IsNullOrEmpty(cacheTableName))
            {
                throw new ArgumentNullException("cacheTableName");
            }

            if (keyFamily == null)
            {
                throw new ArgumentNullException("keyFamily");
            }

            this._dbInfo = dbInfo;
            this._cacheTableName = cacheTableName;
            if (dbInfo.Connection.GetType().Name.Contains("Oracle"))
            {
                this._varChar = "varchar2";
            }

            kf = keyFamily;
            if (CreateTable)
                this.CreateSqlTable(dbInfo, keyFamily);
            else
                this.PrepareSqlColumns(keyFamily);

            parseSdmx = useAttr;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetStoreDB"/> class
        /// </summary>
        /// <param name="dbInfo">
        /// The database information which will used to connect
        /// </param>
        /// <param name="cacheTableName">
        /// The cache database table name
        /// </param>
        /// <param name="keyFamily">
        /// The SDMX keyfamily bean
        /// </param>
        public DataSetStoreDataView(DBInfo dbInfo, string cacheTableName, IDataStructureObject keyFamily)
        {
            if (dbInfo == null)
            {
                throw new ArgumentNullException("dbInfo");
            }

            if (string.IsNullOrEmpty(cacheTableName))
            {
                throw new ArgumentNullException("cacheTableName");
            }

            if (keyFamily == null)
            {
                throw new ArgumentNullException("keyFamily");
            }

            this._dbInfo = dbInfo;
            this._cacheTableName = cacheTableName;
            if (dbInfo.Connection.GetType().Name.Contains("Oracle"))
            {
                this._varChar = "varchar2";
            }

            this.CreateSqlTable(dbInfo, keyFamily);
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the measure dimension concept ref
        /// </summary>
        public string MeasureColumn
        {
            get
            {
                return this._measureColumn;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SDMX Attributes should be parsed. Default is false
        /// </summary>

        private bool parseSdmx = false;
        public bool ParseSdmxAttributes { get { return true; } set { parseSdmx = value; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add (Append) to criteria the given key value pair 
        /// </summary>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <param name="value">
        /// The key's value
        /// </param>
        public void AddCriteria(DataCriteria Criterio)
        {
            this._criteria.Add(Criterio);
        }

        /// <summary>
        /// Add row to dataset store. Each row should contain the values added with <see cref="AddToStore(XmlReader)"/> or <see cref="AddToStore(string,string)"/>. It should be called after each iteration after all <c>AddToStore</c> overloads
        /// Must be called after <see cref="BeginDataSetImport"/>  and before <see cref="Commit"/>
        /// In this implementation it calls <see cref="DbCommand.ExecuteNonQuery"/> at the current <see cref="DbCommand"/>
        /// </summary>
        public void AddRow()
        {
            try
            {
                this._cmd.ExecuteNonQuery();
            }
            catch
            {
                this._dbInfo.Rollback();
                this.DisposeCmd();
                throw;
            }
        }

        /// <summary>
        /// Add the current XML attributes to sdmx dataset store. In this implementation they are added to the current <see cref="DbCommand"/> parameters
        /// Only XML attributes that exist in the dataset store are imported and it is determined by <see cref="ExistsColumn"/>
        /// Must be called after <see cref="BeginDataSetImport"/> and before <see cref="Commit"/>
        /// </summary>
        /// <param name="reader">
        /// The XMLReader instance to read the XML attributes from
        /// </param>
        public void AddToStore(XmlReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                var name = reader.LocalName;
                this.AddToStore(name, reader.Value);
            }
        }

        /// <summary>
        /// Add the specified key and value to sdmx dataset store. In this implementation they are added to the current <see cref="DbCommand"/> parameters
        /// The key must by a valid column else it is ignored and it is determined by <see cref="ExistsColumn"/> 
        /// Must be called after <see cref="BeginDataSetImport"/>  and before <see cref="Commit"/>
        /// </summary>
        /// <param name="key">
        /// The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        public void AddToStore(string key, string value)
        {
            string paraName = this._dbInfo.Database.BuildParameterName(key);
            int idx = this._cmd.Parameters.IndexOf(paraName);
            if (idx > -1)
            {
                this._cmd.Parameters[idx].Value = value;
            }
        }

        /// <summary>
        /// Begin SDMX-ML DataSet import. 
        /// This implementation creates the current <see cref="DbCommand"/> with a prepared statement and initializes it's parameters 
        /// </summary>
        public void BeginDataSetImport()
        {
            this.CreateCmd(true);
            this._cmd.CommandText = this.BuildInsertStmt();
            this.InitParameters(this._cmd);
            this._cmd.Prepare();
        }

        /// <summary>
        /// Commit the added rows with <see cref="AddRow"/>. Must be called after all calls of <see cref="AddRow"/>
        /// </summary>
        public void Commit()
        {
            this._dbInfo.Commit();
            this.DisposeCmd();
        }

        /// <summary>
        /// Get the distinct count of the specified key
        /// Criteria added either by <see cref="AddCriteria"/> or <see cref="SetCriteria"/> are considered
        /// </summary>
        /// <param name="key">
        /// The key
        /// </param>
        /// <returns>
        /// The distinct count
        /// </returns>
        public long Count(string key)
        {
            long count = 0;

            using (var dcmd = this._dbInfo.Command)
            {
                dcmd.CommandText = string.Format(
                    CultureInfo.InvariantCulture,
                    "select count(distinct {0}) from {1} {2}",
                    key,
                    this._cacheTableName,
                    this.BuildWhere(dcmd));
                var o = dcmd.ExecuteScalar();
                if (o != null && !Convert.IsDBNull(o))
                {
                    count = Convert.ToInt64(o, CultureInfo.InvariantCulture);
                }
            }

            return count;
        }

        /// <summary>
        /// Get count of the records
        /// </summary>
        /// <param name="all">
        /// If set to true it will ignore criteria else it will use them
        /// </param>
        /// <returns>
        /// The number of records
        /// </returns>
        public int Count(bool all)
        {
            this.CreateCmd(false);
            this._cmd.CommandText = string.Format(
                CultureInfo.InvariantCulture,
                "select count(*) from {0} {1}",
                this._cacheTableName,
                all ? string.Empty : this.BuildWhere(this._cmd));
            return Convert.ToInt32(this._cmd.ExecuteScalar(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Create a sorted <see cref="IDataReader"/>
        /// </summary>
        /// <param name="all">
        /// If set to true it will ignore criteria else it will use them
        /// </param>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        public IDataReader CreateDataReader(bool all)
        {
            this.CreateCmd(false);
            var sql = new StringBuilder("select * from ").Append(this._cacheTableName);
            if (!all)
            {
                sql.Append(this.BuildWhere(this._cmd));
            }

            sql.Append(this.BuildSort());
            this._cmd.CommandText = sql.ToString();
            return this._cmd.ExecuteReader();
        }

        /// <summary>
        /// Create a unsorted <see cref="IDataReader"/>. It ignores sort and criteria
        /// </summary>
        /// <returns>
        /// A <see cref="IDataReader"/>
        /// </returns>
        public IDataReader CreateDataReaderAllUnsorted()
        {
            this.CreateCmd(false);
            this._cmd.CommandText = string.Format(
                CultureInfo.InvariantCulture, "select * from {0}", this._cacheTableName);
            return this._cmd.ExecuteReader();
        }

        /// <summary>
        /// Dispose the current <see cref="DbCommand"/> and drop the cache database table
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.Collect();
        }

        /// <summary>
        /// Check if the given name exists as a column in SDMX dataset.
        /// </summary>
        /// <param name="name">
        /// The name of the key i.e. the concept ref of the component, the xml attribute name
        /// </param>
        /// <returns>
        /// True if it exists else false
        /// </returns>
        public bool ExistsColumn(string name)
        {
            return name != null && this._columns.ContainsKey(name);
        }

        /// <summary>
        /// Return all columns
        /// </summary>
        /// <returns>Return all columns</returns>
        public List<string> GetAllColumns()
        {
            return this._columns.Keys.ToList();
        }

        /// <summary>
        /// Reset all criteria and sort
        /// </summary>
        public void ResetCriteriaSort()
        {
            this._criteria.Clear();
            this._sort.Clear();
            this.DisposeCmd();
        }

        /// <summary>
        /// Perform a select distinct on the specified key. 
        /// Criteria added either by <see cref="AddCriteria"/> or <see cref="SetCriteria"/> are considered
        /// </summary>
        /// <typeparam name="T">
        /// The type of key values. In most cases it should be <see cref="System.String"/>
        /// </typeparam>
        /// <param name="key">
        /// The key, The key i.e. the concept ref of the component , the xml attribute name
        /// </param>
        /// <returns>
        /// A Set of unique values
        /// </returns>
        public IDictionary<T, object> SelectDistinct<T>(string key)
        {
            IDictionary<T, object> values = new SortedList<T, object>();

            using (DbCommand dcmd = this._dbInfo.Command)
            {
                dcmd.CommandText = string.Format(
                    CultureInfo.InvariantCulture,
                    "select distinct {0} from {1} {2} order by {0}",
                    key,
                    this._cacheTableName,
                    this.BuildWhere(dcmd));
                using (IDataReader reader = dcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object obj = reader.GetValue(0);
                        if (obj is T)
                        {
                            values.Add((T)obj, null);
                        }
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Set criteria to the given map of key and value.
        /// </summary>
        /// <param name="criteria">
        /// A map of key and value.  Set to 
        /// <code>
        /// null
        /// </code>
        /// or empty map to reset all criteria
        /// </param>
        public void SetCriteria(List<DataCriteria> criteria)
        {
            if (criteria != null)
            {
                this._criteria = new List<DataCriteria>(criteria);
            }
            else
            {
                this._criteria.Clear();
            }

            this.DisposeCmd();
        }

        /// <summary>
        /// Set sort order, i.e. order by, to the specified list. All entries will be added as ascenting and order of the list matters.
        /// </summary>
        /// <param name="sort">
        /// The pre-ordered list of keys to include when sorting
        /// </param>
        public void SetSort(IList<string> sort)
        {
            this._sort.Clear();
            if (sort != null)
            {
                this._sort.AddRange(sort);
            }

            this.DisposeCmd();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Build the prepared insert statement for <see cref="_cacheTableName"/> from <see cref="_columns"/>
        /// </summary>
        /// <returns>
        /// A string with the prepared insert statement
        /// </returns>
        protected string BuildInsertStmt()
        {
            var insert = new StringBuilder("insert into " + this._cacheTableName + " (");
            var values = new StringBuilder();
            int lastComma = 0;
            int lastCommaV = 0;
            foreach (string name in this._columns.Keys)
            {
                insert.AppendFormat("{0},", name);
                lastComma = insert.Length - 1;
                string paraName = this._dbInfo.Database.BuildParameterName(name);
                values.AppendFormat("{0},", paraName);
                lastCommaV = values.Length - 1;
            }

            insert.Length = lastComma;
            values.Length = lastCommaV;
            insert.Append(") VALUES (" + values + ")");

            return insert.ToString();
        }

        /// <summary>
        /// Create the <see cref="_cacheTableName"/> table in the database specified in the given <see cref="DBInfo"/> with columns from the specified KeyFamily Bean
        /// It also populates the <see cref="_columns"/>. The components that are used are the Dimensions, TimeDimensions and Primary Measure. Depending on <see cref="ParseSdmxAttributes"/> the SDMX Attributes are either ignored or used
        /// </summary>
        /// <param name="dbInfo">
        /// The <see cref="DBInfo"/>
        /// </param>
        /// <param name="keyFamily">
        /// The key family bean
        /// </param>
        protected void CreateSqlTable(DBInfo dbInfo, IDataStructureObject keyFamily)
        {
            var createTable = new StringBuilder("CREATE TABLE " + this._cacheTableName + " (");
            var primaryMeasureKey = new StringBuilder();

            // dimensions add them all and to primary key
            foreach (IDimension comp in keyFamily.DimensionList.Dimensions)
            {
                if (!comp.TimeDimension)
                {
                    createTable.AppendFormat("{0} {1}({2}) NOT NULL,", comp.Id, this._varChar, VarCharSize);

                    createTable.AppendLine();
                    this._columns.Add(comp.Id, null);
                    if (comp.MeasureDimension)
                    {
                        this._measureColumn = comp.Id;
                    }

                    primaryMeasureKey.Append(comp.Id);
                    primaryMeasureKey.Append(",");
                }
            }

            primaryMeasureKey.Length--;
            if (keyFamily.TimeDimension != null)
            {

                createTable.AppendFormat(
                    "{0} {1}({2}) NOT NULL,", keyFamily.TimeDimension.Id, this._varChar, VarCharSize);
                primaryMeasureKey.Append(",");
                primaryMeasureKey.Append(keyFamily.TimeDimension.Id);

                /*
                lastComma = createTable.Length -1 ;
*/
                createTable.AppendLine();
                this._columns.Add(keyFamily.TimeDimension.Id, null);
            }

            createTable.AppendFormat("{0} {1}({2}),", keyFamily.PrimaryMeasure.Id, this._varChar, VarCharSize);
            int lastComma = createTable.Length - 1;
            createTable.AppendLine();
            this._columns.Add(keyFamily.PrimaryMeasure.Id, null);
            if (this.ParseSdmxAttributes)
            {
                foreach (IAttributeObject comp in keyFamily.Attributes)
                {
                    createTable.AppendFormat("{0} {1}({2}),", comp.Id, this._varChar, VarCharSize);
                    this._columns.Add(comp.Id, null);
                    lastComma = createTable.Length - 1;
                    createTable.AppendLine();
                }
            }

            if (primaryMeasureKey.Length > 0)
            {
                createTable.Append("PRIMARY KEY(");
                createTable.Append(primaryMeasureKey);
                createTable.Append(")");
                createTable.AppendLine();
            }
            else
            {
                createTable.Length = lastComma;
            }

            createTable.Append(")");
            using (DbCommand scmd = dbInfo.TransactionCommand)
            {
                scmd.CommandText = createTable.ToString();
                scmd.ExecuteNonQuery();
            }
            dbInfo.Commit();


        }

        protected void PrepareSqlColumns(IDataStructureObject keyFamily)
        {
            // dimensions add them all and to primary key
            foreach (IDimension comp in keyFamily.DimensionList.Dimensions)
            {
                if (!comp.TimeDimension)
                {
                    this._columns.Add(comp.Id, null);
                    if (comp.MeasureDimension)
                        this._measureColumn = comp.Id;
                }
            }

            if (keyFamily.TimeDimension != null)
                this._columns.Add(keyFamily.TimeDimension.Id, null);

            this._columns.Add(keyFamily.PrimaryMeasure.Id, null);
            if (this.ParseSdmxAttributes)
            {
                foreach (IAttributeObject comp in keyFamily.Attributes)
                    this._columns.Add(comp.Id, null);
            }

        }

        /// <summary>
        /// Dispose any open DataView and DataTable
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether to dispose managed resources or just native
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeCmd();
                this.DisposeTable();
            }
        }

        /// <summary>
        /// Initialize the parameters for the specified  <see cref="DbCommand"/> from <see cref="_columns"/>
        /// Existing parameters are removed.
        /// </summary>
        /// <param name="command">
        /// The <see cref="DbCommand"/>
        /// </param>
        protected void InitParameters(DbCommand command)
        {
            command.Parameters.Clear();
            foreach (string name in this._columns.Keys)
            {
                string paraName = this._dbInfo.Database.BuildParameterName(name);
                DbParameter p = command.CreateParameter();
                p.ParameterName = paraName;
                p.Value = string.Empty;
                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// Build the <c>order by</c> part of the SQL select statement
        /// </summary>
        /// <returns>
        /// If <see cref="_sort"/> is not empty a string with the "order by " contents. Else an empty string
        /// </returns>
        private string BuildSort()
        {
            var orderBy = new StringBuilder(" order by ");
            var lastClause = 0;
            for (int i = 0, j = this._sort.Count; i < j; i++)
            {
                orderBy.Append(this._sort[i]);
                lastClause = orderBy.Length;
                orderBy.Append(", ");
            }

            orderBy.Length = lastClause;
            return orderBy.ToString();
        }

        /// <summary>
        /// Build the <c>where</c> part of the SQL prepared select statement and the parameters
        /// </summary>
        /// <param name="dbCommand">
        /// The <see cref="DbCommand"/> that will execute the  SQL prepared select statement
        /// </param>
        /// <returns>
        /// If <see cref="_criteria"/> is not empty a string with the "where " contents. Else an empty string
        /// </returns>
        private string BuildWhere(DbCommand dbCommand)
        {
            dbCommand.Parameters.Clear();
            List<string> parameters = new List<string>();
            string TimeWhere = "";
            foreach (DataCriteria kv in this._criteria)
            {
                if (kv.values == null || kv.values.Count == 0)
                    continue;
                if (kv.component == kf.TimeDimension.Id)
                {
                    TimeWhere = GetTimeWhere(kv);
                    continue;
                }
                if (kv.values.Count == 1)
                {
                    string paramName = this._dbInfo.Database.BuildParameterName(kv.component);
                    parameters.Add(string.Format(" {0} = {1} ", kv.component, paramName));
                    DbParameter param = dbCommand.CreateParameter();
                    param.ParameterName = paramName;
                    param.Value = kv.values[0];
                    dbCommand.Parameters.Add(param);
                }
                else
                {
                    List<string> parametersOR = new List<string>();
                    foreach (string val in kv.values)
                    {
                        string paramName = this._dbInfo.Database.BuildParameterName(kv.component + val);
                        parametersOR.Add(string.Format(" {0} = {1} ", kv.component, paramName));
                        DbParameter param = dbCommand.CreateParameter();
                        param.ParameterName = paramName;
                        param.Value = val;
                        dbCommand.Parameters.Add(param);
                    }
                    parameters.Add(string.Format(" ({0}) ", string.Join(" or ", parametersOR)));
                }
            }
            if (parameters.Count == 0 && string.IsNullOrEmpty(TimeWhere))
                return String.Empty;
            else if (parameters.Count == 0)
                return string.Format(" where {0}", TimeWhere.Substring(5));
            else
                return string.Format(" where {0} {1}", string.Join(" and ", parameters), TimeWhere);
        }

        private string GetTimeWhere(DataCriteria kv)
        {

            if (kv == null || kv.values == null || kv.values.Count == 0)
                return string.Empty;

            DateTime from;
            DateTime to = DateTime.Now;
            bool UseFrom = DateTime.TryParseExact(kv.values[0], "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.None, out from);
            bool UseTo =kv.values.Count>1? DateTime.TryParseExact(kv.values[1], "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.None, out to):false;

            if (!UseFrom && !UseTo)
                return string.Empty;
            else if (!UseFrom)
                return string.Format(" and {0}", GetTimeWhereStatment(to, "<"));
            else if (!UseTo)
                return string.Format(" and {0}", GetTimeWhereStatment(from, ">"));
            else
                return string.Format(" and ({0} and {1})", GetTimeWhereStatment(from, ">"), GetTimeWhereStatment(to, "<"));
        }

        private string GetTimeWhereStatment(DateTime dt, string SymbolComp)
        {
            string timedim = kf.TimeDimension.Id;
            List<string> OrTimeFormatWhere = new List<string>();

            int MounthConsidered = dt.Date.Month;


            //Tutte le possibili condizioni
            //==yyyy-MM-dd oppure yyyyMMdd
            OrTimeFormatWhere.Add(string.Format("({0} = '{2}')", timedim, SymbolComp, dt.Date.ToString("yyyy-MM-dd")));
            #region Anno
            OrTimeFormatWhere.Add(string.Format("(LENGTH({0}) = 4 AND {0} {1}= '{2}')", timedim, SymbolComp, dt.Date.Year));
            #endregion
            #region Mensile
            string DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.Month)];
            OrTimeFormatWhere.Add(string.Format(@"
(
    LENGTH({0}) = 7 AND
    SUBSTR({0},6,1)  IN('0','1')  
    AND 
    (
        SUBSTR({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTR({0},0,5) = '{3}' 
            AND 
            CAST(SUBSTR({0},6,LENGTH({0})-5) as INTEGER) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Year, MounthConsidered));
            #endregion
            #region Quartely
            DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.QuarterOfYear)];
            int quarter = (MounthConsidered + 2) / 3;
            OrTimeFormatWhere.Add(string.Format(@"
(
    LENGTH({0}) > 4 AND
    SUBSTR({0},5,1) ='{1}' 
    AND 
    (
        SUBSTR({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTR({0},0,5) = '{3}' 
            AND 
            CAST(SUBSTR({0},6,LENGTH({0})-5 ) as INTEGER) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Year, quarter));
            #endregion
            #region ThirdOfYear
            DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.ThirdOfYear)];
            int ThirdOfYear = (MounthConsidered + 3) / 4;
            OrTimeFormatWhere.Add(string.Format(@"
(
    LENGTH({0}) > 4 AND
    SUBSTR({0},5,1) ='{1}' 
    AND 
    (
        SUBSTR({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTR({0},0,5) = '{3}' 
            AND 
            CAST(SUBSTR({0},6,LENGTH({0})-5 ) as INTEGER) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Year, ThirdOfYear));
            #endregion
            #region HalfOfYear
            DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.HalfOfYear)];
            int HalfOfYear = (MounthConsidered + 5) / 6;
            OrTimeFormatWhere.Add(string.Format(@"
(
    LENGTH({0}) > 4 AND
    SUBSTR({0},5,1) ='{1}' 
    AND 
    (
        SUBSTR({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTR({0},0,5) = '{3}' 
            AND 
            CAST(SUBSTR({0},6,LENGTH({0})-5 ) as INTEGER) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Year, HalfOfYear));
            #endregion


            return string.Format("({0})", string.Join(" OR ", OrTimeFormatWhere));


        }
        private Dictionary<TimeFormat, string> MappingTimePeriodDbFormat = new Dictionary<TimeFormat, string>()
        {
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Year), ""},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Month), "M"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.QuarterOfYear), "Q"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.HalfOfYear), "S"},
            
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Week), "W"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Date), "D"},

            {TimeFormat.GetFromEnum(TimeFormatEnumType.DateTime), "-"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Hour), "-"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.ThirdOfYear), "T"},
        };

        /// <summary>
        /// Create a new current <see cref="DbCommand"/>. It will dispose the existing command
        /// </summary>
        private void CreateCmd(bool inTrans)
        {
            this.DisposeCmd();
            if (inTrans)
                this._cmd = this._dbInfo.TransactionCommand;
            else
                this._cmd = this._dbInfo.Command;
        }

        /// <summary>
        /// Dispose the current <see cref="DbCommand"/>
        /// </summary>
        private void DisposeCmd()
        {
            if (this._cmd == null)
            {
                return;
            }

            this._cmd.Dispose();
            this._cmd = null;
        }

        /// <summary>
        /// Drop the <see cref="_cacheTableName"/>
        /// </summary>
        private void DisposeTable()
        {
            if (this._dbInfo.Disposed)
            {
                return;
            }
            this._dbInfo.Dispose();
            //try
            //{
            //    using (DbCommand command = this._dbInfo.TransactionCommand)
            //    {
            //        command.CommandText = string.Format(CultureInfo.InvariantCulture, "DROP TABLE {0}", this._cacheTableName);
            //        command.ExecuteNonQuery();
            //        this._dbInfo.Commit();
            //    }
            //}
            //catch (DbException e)
            //{
            //    _log.ErrorFormat("Could not drop table {0}", this._cacheTableName);
            //    _log.Error(e.Message, e);
            //}
            //catch (Exception e)
            //{
            //    _log.ErrorFormat("Could not drop table {0}", this._cacheTableName);
            //    _log.Error(e.Message, e);
            //}
        }

        #endregion



    }
}
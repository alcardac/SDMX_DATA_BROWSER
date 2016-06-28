// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInfo.cs" company="Eurostat">
//   Date Created : 2011-04-01
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   Database connectio helper class
//   It provides wrappers on Entlib 3.1 Database class and various Db* classes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetEngine.Model.DBData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Data;
    using ISTAT.WebClient.WidgetComplements.Model;

    /// <summary>
    /// Database connectio helper class
    /// It provides wrappers on Entlib 3.1 Database class and various Db* classes
    /// </summary>
    public class DBInfo : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The Entlib database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The list temporary tables that need to be deleted on Dispose
        /// </summary>
        private readonly List<string> _tempTables = new List<string>();

        /// <summary>
        /// The current connection
        /// </summary>
        private DbConnection _connection;

        /// <summary>
        /// The currect transaction
        /// </summary>
        private DbTransaction _transaction;

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructors and Destructors


        /// <summary>
        /// Initializes a new instance of the <see cref="DBInfo"/> class. 
        /// </summary>
        /// <param name="connectionStringSettings">
        /// The db connection settings
        /// </param>
        public DBInfo(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionStringSettings");

            DictionaryConfigurationSource source = new DictionaryConfigurationSource();
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings(Constants.ConnectionStringSettingsName, connectionString, Constants.SystemDataSqlite);
            ConnectionStringsSection section = new ConnectionStringsSection();
            section.ConnectionStrings.Add(connectionStringSettings);
            source.Add("connectionStrings", section);
            this._database = new DatabaseProviderFactory(source).Create(connectionStringSettings.Name);
            this.CreateConnection();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DBInfo"/> class. 
        /// </summary>
        /// <param name="connectionStringSettings">
        /// The db connection settings
        /// </param>
        public DBInfo(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
            {
                throw new ArgumentNullException("connectionStringSettings");
            }

            DictionaryConfigurationSource source = new DictionaryConfigurationSource();
            ConnectionStringsSection section = new ConnectionStringsSection();
            section.ConnectionStrings.Add(connectionStringSettings);
            source.Add("connectionStrings", section);
            this._database = new DatabaseProviderFactory(source).Create(connectionStringSettings.Name);
            this.CreateConnection();
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="DbCommand"/> from open connection
        /// </summary>
        /// <value>Db command</value>
        public DbCommand Command
        {
            get
            {
                DbCommand cmd = this.OpenConnection.CreateCommand();
                cmd.Transaction = this._transaction;

                return cmd;
            }
        }

        /// <summary>
        /// Gets the current connection
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                return this._connection;
            }
        }

        /// <summary>
        /// Gets an open connection to the database and begin transaction
        /// </summary>
        /// <value>
        ///   The db connection
        /// </value>
        public DbConnection ConnectionWithTransaction
        {
            get
            {
                DbConnection dbConn = this.OpenConnection;
                if (this._transaction == null)
                {
                    this._transaction = dbConn.BeginTransaction();
                }

                return dbConn;
            }
        }

        /// <summary>
        /// Gets the Entlib database
        /// </summary>
        public Database Database
        {
            get
            {
                return this._database;
            }
        }

        /// <summary>
        /// Gets open connection to the database
        /// </summary>
        /// <value>
        ///   The db connection
        /// </value>
        public DbConnection OpenConnection
        {
            get
            {
                if ((this._connection.State & ConnectionState.Open) != ConnectionState.Open)
                {
                    this._connection.Open();
                }

                return this._connection;
            }
        }

        /// <summary>
        /// Gets the list temporary tables that need to be deleted on Dispose
        /// </summary>
        public IList<string> TempTables
        {
            get
            {
                return this._tempTables;
            }
        }

        /// <summary>
        /// Gets the db command from open connection and begin transaction
        /// </summary>
        /// <value>Db command</value>
        public DbCommand TransactionCommand
        {
            get
            {
                DbCommand cmd = this.ConnectionWithTransaction.CreateCommand();
                cmd.Transaction = this._transaction;

                return cmd;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        public bool Disposed
        {
            get
            {
                return this._disposed;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Commit transaction
        /// </summary>
        public void Commit()
        {
            if (this._transaction != null)
            {
                this._transaction.Commit();
                this._transaction = null;
            }
        }

        /// <summary>
        /// Drop tables and close the connection
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                this.Dispose(true);
                GC.Collect();
                this._disposed = true;
            }
        }

        /// <summary>
        /// Rollback transcaction
        /// </summary>
        public void Rollback()
        {
            if (this._transaction != null)
            {
                this._transaction.Rollback();
                this._transaction = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Dispose native and depending on <paramref name="disposing"/> managed resources 
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._transaction != null)
                {
                    this._transaction.Commit();
                    this._transaction = null;
                }
                //this.DropTempTables();
                this._connection.Close();
                this._connection.Dispose();
                this._connection = null;

            }
        }

        /// <summary>
        /// Create new connection to the database
        /// </summary>
        private void CreateConnection()
        {
            this._connection = this._database.CreateConnection();
        }

        /// <summary>
        /// Drop all temp files and clear the temp list
        /// </summary>
        private void DropTempTables()
        {
            using (DbCommand cmd = this.TransactionCommand)
            {
                for (int i = 0, j = this._tempTables.Count; i < j; i++)
                {
                    cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "DROP TABLE {0}", this._tempTables[i]);
                    cmd.ExecuteNonQuery();
                }
            }

            this.Commit();
            this._tempTables.Clear();
        }

        #endregion
    }
}
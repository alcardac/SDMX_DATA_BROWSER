// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetReader.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   De-Serialize SDMX-ML Dataset to the speficied store
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace ISTAT.WebClient.WidgetComplements.Model.DataReaderNSI
{
    using System;
    using System.Xml;

    using ISTAT.WebClient.WidgetComplements.Model.Properties;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Engine;
    using Org.Sdmxsource.Sdmx.Api.Util;
    using Org.Sdmxsource.Sdmx.DataParser.Engine;
    using Org.Sdmxsource.Sdmx.DataParser.Engine.Reader;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;

    /// <summary>
    /// De-Serialize SDMX-ML Dataset to the speficied store
    /// </summary>
    public abstract class DataSetReader
    {
        #region Constants and Fields

        /// <summary>
        /// The <see cref="IDataSetStore"/> in which the SDMX-ML dataset will be stored
        /// </summary>
        private readonly IDataSetStore _dataSetStore;

        /// <summary>
        /// The key family of the SDMX-ML dataset
        /// </summary>
        private readonly IDataStructureObject _keyFamily;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSetReader"/> class. 
        /// Initialize a new instance of the <see cref="DataSetReader"/>
        /// </summary>
        /// <param name="keyFamily">
        /// The key family of the SDMX-ML dataset
        /// </param>
        /// <param name="store">
        /// The <see cref="IDataSetStore"/> in which the SDMX-ML dataset will be stored
        /// </param>
        protected DataSetReader(IDataStructureObject keyFamily, IDataSetStore store)
        {
            this._dataSetStore = store;
            this._keyFamily = keyFamily;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="IDataSetStore"/> in which the SDMX-ML dataset will be stored
        /// </summary>
        protected internal IDataSetStore DataSetStore
        {
            get
            {
                return this._dataSetStore;
            }
        }

        /// <summary>
        /// Gets the key family of the SDMX-ML dataset
        /// </summary>
        protected internal IDataStructureObject KeyFamily
        {
            get
            {
                return this._keyFamily;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a reader based on the specified <paramref name="operation"/>
        /// </summary>
        /// <param name="operation">
        /// The operation
        /// </param>
        /// <param name="keyFamily">
        /// The SDMX-ML dataset KeyFamily (i.e. DSD) 
        /// </param>
        /// <param name="store">
        /// The <see cref="IDataSetStore"/> in which the dataset will be stored
        /// </param>
        /// <param name="dataflow">
        /// The <see cref="IDataflowObject"/> the dataflow
        /// </param>
        /// <param name="dataLocation">
        /// The <see cref="IReadableDataLocation"/> the data location 
        /// </param>
        public static void GetReader(SDMXWSFunction operation, IDataStructureObject keyFamily, IDataSetStore store,
          IDataflowObject dataflow, IReadableDataLocation dataLocation)
        {
            switch (operation)
            {
                case SDMXWSFunction.GetCompactData:
                    var compact = new CompactDataReaderEngine(dataLocation, dataflow, keyFamily);
                    var readerCompact = new SdmxDataReader(keyFamily, store);
                    readerCompact.ReadData(compact);
                    break;

                case SDMXWSFunction.GetCrossSectionalData:
                    var dsdCrossSectional = (ICrossSectionalDataStructureObject)keyFamily;
                    var crossSectional = new CrossSectionalDataReaderEngine(dataLocation, dsdCrossSectional, dataflow);
                    var reader = new SdmxDataReader(keyFamily, store);
                    reader.ReadData(crossSectional);
                    break;

                default:
                    throw new ArgumentException(Resources.ExceptionUnsupported_operation + operation.ToString(), "operation");
            }
        }

        /// <summary>
        /// Read SDMX-ML DataSet files into the <see cref="IDataSetStore"/> given at the constructor
        /// </summary>
        /// <param name="dataReader">
        /// The XMLReader to read te SDMX-ML from
        /// </param>
        public abstract void ReadData(IDataReaderEngine dataReader);

        #endregion
    }
}
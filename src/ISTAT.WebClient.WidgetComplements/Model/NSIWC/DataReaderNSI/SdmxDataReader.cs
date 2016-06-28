// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompactDataReader.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   De-Serializes a SDMX-ML Compact data set to a <see cref="System.Data.DataTable" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model.DataReaderNSI
{
    using System;
    using System.Xml;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Constants.InterfaceConstant;
    using Org.Sdmxsource.Sdmx.Api.Engine;


    /// <summary>
    /// De-Serializes a SDMX-ML Compact data set to a <see cref="System.Data.DataTable"/>
    /// </summary>
    public class SdmxDataReader : DataSetReader
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SdmxDataReader"/> class. 
        /// Initialize a new instance of the <see cref="SdmxDataReader"/>
        /// </summary>
        /// <param name="keyFamily">
        /// The key family of the SDMX-ML dataset
        /// </param>
        /// <param name="store">
        /// The <see cref="IDataSetStore"/> in which the SDMX-ML dataset will be stored
        /// </param>
        public SdmxDataReader(IDataStructureObject keyFamily, IDataSetStore store)
            : base(keyFamily, store)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read SDMX-ML DataSet files into the <see cref="IDataSetStore"/> given at the constructor
        /// </summary>
        /// <param name="dataReader">
        /// The IDataReaderEngine to read te SDMX-ML from
        /// </param>
        public override void ReadData(IDataReaderEngine dataReader)
        {
            this.DataSetStore.BeginDataSetImport();
            bool isTimeSeries = KeyFamily.TimeDimension != null;


            while (dataReader.MoveNextKeyable())
            {

                // In DatasetAttributes ci sono gli attributi a livello di dataset
                foreach (var key in dataReader.DatasetAttributes)
                {
                    this.DataSetStore.AddToStore(key.Concept, key.Code);
                }
                // In CurrentKey.Key ci sono le dimensioni
                foreach (var key in dataReader.CurrentKey.Key)
                {
                    this.DataSetStore.AddToStore(key.Concept, key.Code);
                }

                // In CurrentKey.Attributes ci sono gli attributi a livello di serie
                foreach (var key in dataReader.CurrentKey.Attributes)
                {
                    this.DataSetStore.AddToStore(key.Concept, key.Code);
                }

                while (dataReader.MoveNextObservation())
                {
                    if (isTimeSeries)
                    {
                        this.DataSetStore.AddToStore(DimensionObject.TimeDimensionFixedId,
                                                     dataReader.CurrentObservation.ObsTime);
                    }
                    if (dataReader.CurrentObservation.CrossSection)
                    {
                        this.DataSetStore.AddToStore(dataReader.CurrentObservation.CrossSectionalValue.Concept,
                                                     dataReader.CurrentObservation.CrossSectionalValue.Code);
                    }

                    // In CurrentObservation.Attributes ci sono gli attributi a livello di osservazione
                    foreach (var key in dataReader.CurrentObservation.Attributes)
                    {
                        this.DataSetStore.AddToStore(key.Concept, key.Code);
                    }

                    this.DataSetStore.AddToStore(PrimaryMeasure.FixedId, dataReader.CurrentObservation.ObservationValue);

                    this.DataSetStore.AddRow();
                }
            }

            this.DataSetStore.Commit();
        }
        #endregion

    }

}
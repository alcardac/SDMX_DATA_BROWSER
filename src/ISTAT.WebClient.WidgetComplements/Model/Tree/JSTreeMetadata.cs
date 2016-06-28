// -----------------------------------------------------------------------
// <copyright file="JSTreeMetadata.cs" company="EUROSTAT">
//   Date Created : 2011-09-28
//   Copyright (c) 2009, 2015 by the European Commission, represented by Eurostat.   All rights reserved.
// 
// Licensed under the EUPL, Version 1.1 or – as soon they
// will be approved by the European Commission - subsequent
// versions of the EUPL (the "Licence");
// You may not use this work except in compliance with the
// Licence.
// You may obtain a copy of the Licence at:
// 
// https://joinup.ec.europa.eu/software/page/eupl 
// 
// Unless required by applicable law or agreed to in
// writing, software distributed under the Licence is
// distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the Licence for the specific language governing
// permissions and limitations under the Licence.
// </copyright>
// -----------------------------------------------------------------------
namespace ISTAT.WebClient.WidgetComplements.Model.Tree
{
    /// <summary>
    /// The dataflow tree specific metadata. Needs to be converted to be more generic
    /// </summary>
    public class JSTreeMetadata
    {
        #region Constants and Fields

        /// <summary>
        /// The dataflow agency
        /// </summary>
        private string _dataflowAgency;

        /// <summary>
        /// The dataflow id
        /// </summary>
        private string _dataflowID;

        /////// <summary>
        /////// The dataflow name
        /////// </summary>
        ////private string _dataflow_name;

        /// <summary>
        /// The dataflow version
        /// </summary>
        private string _dataflowVersion;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the dataflow agency
        /// </summary>
        public string DataflowAgency
        {
            get
            {
                return this._dataflowAgency;
            }

            set
            {
                this._dataflowAgency = value;
            }
        }

        /// <summary>
        /// Gets or sets the dataflow id
        /// </summary>
        public string DataflowID
        {
            get
            {
                return this._dataflowID;
            }

            set
            {
                this._dataflowID = value;
            }
        }

        /*
        /// <summary>
        /// Gets the dataflow name
        /// </summary>
        public string dataflow_name
        {
            get
            {
                return this._dataflow_name;
            }

            set
            {
                this._dataflow_name = value;
            }
        }
        */

        /// <summary>
        /// Gets or sets the dataflow version
        /// </summary>
        public string DataflowVersion
        {
            get
            {
                return this._dataflowVersion;
            }

            set
            {
                this._dataflowVersion = value;
            }
        }

        #endregion
    }
}
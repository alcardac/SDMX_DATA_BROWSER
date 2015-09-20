// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JSTreeMetadata.cs" company="Eurostat">
//   Date Created : 2011-09-28
//   Copyright (c) 2011 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   The dataflow tree specific metadata. Needs to be converted to be more generic
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetEngine.Builder.Tree
{


    public class DataflowMetaUrl {
        private string _title;
        private string _url;

        public string Title { get { return _title; } set { _title = value; } }
        public string URL { get { return _url; } set { _url = value; } }
    }

    /// <summary>
    /// The dataflow tree specific metadata. Needs to be converted to be more generic
    /// </summary>
    public class JSTreeMetadata
    {
        #region Constants and Fields

        /// <summary>
        /// The dataflow url
        /// </summary>
        private string _dataflowUrlV20;
        private string _dataflowUrl;
        private string _dataflowUrlType;
        private string _dataflowSource;
        private string _dataflowDecimalCulture;
        private string _dataflowID;
        private string _dataflowAgency;   
        private string _dataflowVersion;
        private string _dataflow_name;
        private string _dataflow_desc;
        private string _dataflow_urls;
        
        

        #endregion

        #region Public Properties
        
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

        public string DataflowUrlV20
        {
            get
            {
                return this._dataflowUrlV20;
            }

            set
            {
                this._dataflowUrlV20 = value;
            }
        }
        public string DataflowUrl
        {
            get
            {
                return this._dataflowUrl;
            }

            set
            {
                this._dataflowUrl = value;
            }
        }
        public string DataflowUrlType
        {
            get
            {
                return this._dataflowUrlType;
            }

            set
            {
                this._dataflowUrlType = value;
            }
        }
        public string DataflowSource
        {
            get
            {
                return this._dataflowSource;
            }

            set
            {
                this._dataflowSource = value;
            }
        }

        public string DataflowDecimalCulture
        {
            get
            {
                return this._dataflowDecimalCulture;
            }

            set
            {
                this._dataflowDecimalCulture = value;
            }
        }

        public string DataflowName
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

        public string DataflowDesc
        {
            get
            {
                return this._dataflow_desc;
            }

            set
            {
                this._dataflow_desc = value;
            }
        }
        public string DataflowUrls
        {
            get
            {
                return this._dataflow_urls;
            }

            set
            {
                this._dataflow_urls = value;
            }
        }

        #endregion
    }
}
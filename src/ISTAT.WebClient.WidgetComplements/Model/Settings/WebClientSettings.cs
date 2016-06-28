using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.Settings
{
    public class WebClientSettings : ConfigurationSection
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly WebClientSettings _instance =
            (WebClientSettings)ConfigurationManager.GetSection("WebClientSettings");

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static WebClientSettings Instance
        {
            get
            {
                return _instance;
            }
        }

        [ConfigurationProperty("DecimalSeparator", DefaultValue = ".")]
        public string DecimalSeparator
        {
            get
            {
                return (string)this["DecimalSeparator"];
            }

            set
            {
                this["DecimalSeparator"] = value;
            }
        }


        [ConfigurationProperty("EnableCacheTree", DefaultValue = true)]
        public bool EnableCacheTree
        {
            get
            {
                return (bool)this["EnableCacheTree"];
            }

            set
            {
                this["EnableCacheTree"] = value;
            }
        }

        [ConfigurationProperty("RefreshCacheTree", DefaultValue = 1000)]
        public int RefreshCacheTree
        {
            get
            {
                return (int)this["RefreshCacheTree"];
            }

            set
            {
                this["RefreshCacheTree"] = value;
            }
        }

        [ConfigurationProperty("DeleteCacheTree", DefaultValue = 24)]
        public int DeleteCacheTree
        {
            get
            {
                return (int)this["DeleteCacheTree"];
            }

            set
            {
                this["DeleteCacheTree"] = value;
            }
        }

        [ConfigurationProperty("MaxResultHTML", DefaultValue = 100000)]
        public int MaxResultHTML
        {
            get
            {
                return (int)this["MaxResultHTML"];
            }

            set
            {
                this["MaxResultHTML"] = value;
            }
        }

        [ConfigurationProperty("MaxResultObs", DefaultValue = 100000)]
        public int MaxResultObs
        {
            get
            {
                return (int)this["MaxResultObs"];
            }

            set
            {
                this["MaxResultObs"] = value;
            }
        }

        [ConfigurationProperty("UseWidgetCache", DefaultValue = true)]
        public bool UseWidgetCache
        {
            get
            {
                return (bool)this["UseWidgetCache"];
            }

            set
            {
                this["UseWidgetCache"] = value;
            }
        }

       [ConfigurationProperty("CachedTree", DefaultValue = false)]
        public bool CachedTree
        {
            get
            {
                return (bool)this["CachedTree"];
            }

            set
            {
                this["CachedTree"] = value;
            }
        }




        #endregion
    }
}

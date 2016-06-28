using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.WidgetComplements.Model.Settings
{
    public class WCSettings :  ConfigurationSection 
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly WCSettings _instance =
            (WCSettings)ConfigurationManager.GetSection("Settings");

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static WCSettings Instance
        {
            get
            {
                return _instance;
            }
        }



        [ConfigurationProperty("view_tree", IsRequired = false)]
        public bool view_tree
        {
            get { return (bool)this["view_tree"]; }
            set { this["view_tree"] = value; }
        }

        [ConfigurationProperty("view_tree_req", IsRequired = false)]
        public bool view_tree_req
        {
            get { return (bool)this["view_tree_req"]; }
            set { this["view_tree_req"] = value; }
        }

        [ConfigurationProperty("view_tree_select", IsRequired = false)]
        public bool view_tree_select
        {
            get { return (bool)this["view_tree_select"]; }
            set { this["view_tree_select"] = value; }
        }





        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;



namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{


    public class Settings : ConfigurationSection
    {
        /* public bool view_tree { get; set; }  
         public bool view_tree_req { get; set; }
         public bool view_tree_select { get; set; }
        */
         private static readonly Settings _instance =
              (Settings)ConfigurationManager.GetSection("Settings");

         /// <summary>
         /// Gets the current instance
         /// </summary>
         public static Settings Instance
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




    }
    public class GetEndpointSettings 
    {
        public Settings settings { get; set; }
        public List<EndpointSettings> endpoints { get; set; }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;



namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{

    

    public class Settings 
    {
         private bool view_login_def = true; 

         public bool view_tree { get; set;}
         public bool view_tree_req { get; set; }
         public bool view_tree_select { get; set; }
         public bool view_login
         {
             get { return this.view_login_def; }
             set { this.view_login_def = value; }
         }

    }
    public class GetEndpointSettings 
    {
        public Settings settings { get; set; }
        public List<EndpointSettings> endpoints { get; set; }
    }



}

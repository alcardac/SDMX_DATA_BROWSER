using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{

    public class Settings{
         public bool view_tree { get; set; }  
         public bool view_tree_req { get; set; }
         public bool view_tree_select { get; set; }
    }
    public class GetEndpointSettings
    {
        public Settings settings { get; set; }
        public List<EndpointSettings> endpoints { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class MaintenableObj
    {
        public string id { get; set; }
        public string agency { get; set; }
        public string version { get; set; }
        public string source { get; set; }

        public string name { get; set; }
        public string description { get; set; }
    }
}

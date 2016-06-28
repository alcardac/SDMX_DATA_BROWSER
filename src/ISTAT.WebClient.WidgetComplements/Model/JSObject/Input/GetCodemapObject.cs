using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class GetCodemapObject
    {
        public EndpointSettings Configuration { get; set; }
        public MaintenableObj Dataflow { get; set; }
        public string Codelist { get; set; }
        public Dictionary<string,List<string>> PreviusCostraint { get; set; }
        public string Selectionkey { get; set; }
        public string Selectionval { get; set; }
    }
}

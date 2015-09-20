using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{

    public class CodeObj
    {
        public string name { get; set; }
        public string parent { get; set; }
    }
    public class CodemapObj
    {
        public object title { get; set; }
        public Dictionary<string, CodeObj> codes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class CodemapResponseObject
    {
        public MaintenableObj dataflow { get; set; }
        public Dictionary<string, CodemapObj> codemap { get; set; }
        public Dictionary<string, List<string>> costraint { get; set; }
        public List<string> hideDimension { get; set; }
        public string key_time_dimension { get; set; }
        public string freq_dimension { get; set; }
        public bool enabledVar { get; set; }
        public bool enabledCri { get; set; }
        public bool enabledDec { get; set; }
        
    }
    public class CodemapSpecificResponseObject : CodemapResponseObject
    {

        public string codelist_target { get; set; }
    }
}

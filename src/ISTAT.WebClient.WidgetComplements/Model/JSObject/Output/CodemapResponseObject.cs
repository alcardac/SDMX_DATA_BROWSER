using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;


namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class CodemapResponseObject
    {
        public MaintenableObj dataflow { get; set; }
        public Dictionary<string, CodemapObj> codemap { get; set; }
        public Dictionary<string, CodemapObj> concept { get; set; }
        public Dictionary<string, List<string>> costraint { get; set; }
        public List<string> hideDimension { get; set; }
        public string key_time_dimension { get; set; }
        public string freq_dimension { get; set; }
        public bool enabledVar { get; set; }
        public bool enabledCri { get; set; }
        public bool enabledDec { get; set; }

        /*public Dictionary<string, List<string>> SliceKeyValidValues { get; set; }
        public OrderedDictionary SliceKeyValues { get; set; }
        public ICollection SliceKeys { get; set; }*/
/*
                         { "SliceKeyValidValues", query.DatasetModel.SliceKeyValidValues },
                         { "SliceKeyValues", query.DatasetModel.SliceKeyValues },
                         { "SliceKeys", query.DatasetModel.SliceKeys },

  */      
    }
    public class CodemapSpecificResponseObject : CodemapResponseObject
    {

        public string codelist_target { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class DatasetJsonObj
    {
        public Dictionary<string, Dictionary<string,string>> series { get; set; }

    }
}

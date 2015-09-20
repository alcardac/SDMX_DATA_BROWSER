using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetEngine.Model
{
    public class DataCriteria
    {
        public string component { get; set; }
        public List<string> values { get; set; }

       
    }
}

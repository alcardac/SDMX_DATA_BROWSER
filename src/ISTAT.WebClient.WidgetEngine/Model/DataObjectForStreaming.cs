using ISTAT.WebClient.WidgetComplements.Model.DataRender;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model;

namespace ISTAT.WebClient.WidgetEngine.Model
{
    public class DataObjectForStreaming
    {

        public EndpointSettings Configuration { get; set; }
        public IDataSetStore store { get; set; }
        public LayoutObj layObj { get; set; }
        public List<DataCriteria> Criterias { get; set; }
        public ISdmxObjects structure { get; set; }
        public ComponentCodeDescriptionDictionary codemap { get; set; }
        public int WidgetID { get; set; }
    }
}

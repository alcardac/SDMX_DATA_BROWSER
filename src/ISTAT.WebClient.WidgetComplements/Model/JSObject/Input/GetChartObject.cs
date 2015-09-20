using ISTAT.WebClient.WidgetComplements.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class CustomChartSerie {

        public string code { get; set; }
        public string chartType { get; set; }
        public string title { get; set; }
    }

    public class GetChartObject
    {
        public EndpointSettings Configuration { get; set; }
        public MaintenableObj Dataflow { get; set; }
        public Dictionary<string, List<string>> Criteria { get; set; }
        public List<string> ObsValue { get; set; }
        public string ChartType { get; set; }
        public string DimensionAxe { get; set; }
        public string CustomKey { get; set; }
        public List<CustomChartSerie> CustomChartType { get; set; }
        public int WidgetId { get; set; }
    }
    public class GetDsdObject
    {
        public EndpointSettings Configuration { get; set; }
        public MaintenableObj Dataflow { get; set; }
    }
}

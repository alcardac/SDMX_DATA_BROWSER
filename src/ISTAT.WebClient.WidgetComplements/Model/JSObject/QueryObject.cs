using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class QueryObject
    {
        public string QueryId { get; set; }
        public string Title { get; set; }

        public MaintenableObj Dataflow { get; set; }
        public LayoutObj Layout { get; set; }
        public Dictionary<string, List<string>> Criteria { get; set; }
        public EndpointSettings Configuration { get; set; }

        [JsonIgnore]
        public string _DataflowString
        {
            get
            { return new JavaScriptSerializer().Serialize(Dataflow).Replace("'", "''"); }
            set
            { Dataflow = new JavaScriptSerializer().Deserialize<MaintenableObj>(value); }
        }
        [JsonIgnore]
        public string _LayoutString
        {
            get
            { return new JavaScriptSerializer().Serialize(Layout).Replace("'", "''"); }
            set
            { Layout = new JavaScriptSerializer().Deserialize<LayoutObj>(value); }
        }
        [JsonIgnore]
        public string _CriteriaString
        {
            get
            { return new JavaScriptSerializer().Serialize(Criteria).Replace("'", "''"); }
            set
            { Criteria = new JavaScriptSerializer().Deserialize<Dictionary<string, List<string>>>(value); }
        }
        [JsonIgnore]
        public string _ConfigurationString
        {
            get
            { return new JavaScriptSerializer().Serialize(Configuration).Replace("'", "''"); }
            set
            { Configuration = new JavaScriptSerializer().Deserialize<EndpointSettings>(value); }
        }
        [JsonIgnore]
        public string _QueryUniqueKeyString
        {
            get
            { return new JavaScriptSerializer().Serialize(Dataflow.id + "+" + Dataflow.agency + "+" + Dataflow.version + "+" + Configuration.EndPoint).Replace("'", "''"); }
            set
            { Configuration = new JavaScriptSerializer().Deserialize<EndpointSettings>(value); }
        }

    }
}

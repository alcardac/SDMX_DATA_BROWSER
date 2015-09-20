using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class TemplateObject
    {
        public string TemplateId { get; set; }
        public string Title { get; set; }

        public EndpointSettings Configuration { get; set; }
        public MaintenableObj Dataflow { get; set; }
        public LayoutObj Layout { get; set; }
        public Dictionary<string, List<string>> Criteria { get; set; }
        public List<string> HideDimension { get; set; }
        public bool BlockXAxe { get; set; }
        public bool BlockYAxe { get; set; }
        public bool BlockZAxe { get; set; }
                                            
        public bool EnableCriteria { get; set; }
        public bool EnableDecimal { get; set; }
        public bool EnableVaration { get; set; }
                    
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

        [JsonIgnore]
        public string _HideDimensionString
        {
            get
            { return new JavaScriptSerializer().Serialize(HideDimension).Replace("'", "''"); }
            set
            { HideDimension = new JavaScriptSerializer().Deserialize<List<string>>(value); }
        }
        [JsonIgnore]
        public string _BlockXAxeString
        {
            get
            { return new JavaScriptSerializer().Serialize(BlockXAxe).Replace("'", "''"); }
            set
            { BlockXAxe = new JavaScriptSerializer().Deserialize<bool>(value); }
        }
        [JsonIgnore]
        public string _BlockYAxeString
        {
            get
            { return new JavaScriptSerializer().Serialize(BlockYAxe).Replace("'", "''"); }
            set
            { BlockYAxe = new JavaScriptSerializer().Deserialize<bool>(value); }
        }
        [JsonIgnore]
        public string _BlockZAxeString
        {
            get
            { return new JavaScriptSerializer().Serialize(BlockZAxe).Replace("'", "''"); }
            set
            { BlockZAxe = new JavaScriptSerializer().Deserialize<bool>(value); }
        }
    }
}

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    using ISTAT.WebClient.WidgetComplements.Model.Properties;
    using ISTAT.WebClient.WidgetComplements.Model.Exceptions;
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Net;
    using Newtonsoft.Json;
    using ISTAT.WebClient.WidgetComplements.Model.Enum;

    public class EndpointSettings : ICloneable
    {
        private EndpointSettings endpointSetting;

        public EndpointSettings()
        {
            this.Prefix = "web";
        }

        public EndpointSettings(EndpointSettings endpointSetting)
        {
            // TODO: Complete member initialization
            this.endpointSetting = endpointSetting;
        }

        public string Locale { get; set; }
        public string IDNode { get; set; }
        public string Title { get; set; }
        public string DecimalSeparator { get; set; }
        public string Domain { get; set; }
        public bool EnableHTTPAuthentication { get; set; }
        public bool EnableProxy { get; set; }
        public string EndPoint { get; set; }
        public string EndPointV20 { get; set; }
        public string EndPointType { get; set; }
        public string EndPointSource { get; set; }
        public string Password { get; set; }
        public string Prefix { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyServer { get; set; }
        public int ProxyServerPort { get; set; }
        public string ProxyUserName { get; set; }
        public bool UseSystemProxy { get; set; }
        public string UserName { get; set; }
        public string Wsdl { get; set; }
        public bool Active { get; set; }
        public bool UseUncategorysed { get; set; }
        public bool UseVirtualDf { get; set; }
        public int Ordinamento { get; set; }

        [JsonIgnore]
        public EndpointType _TypeEndpoint
        {
            get
            {
                if (string.IsNullOrEmpty(EndPointType))
                    return EndpointType.V20;
                switch (EndPointType.Trim().ToUpper())
                {
                    case "V20": return EndpointType.V20;
                    case "V21": return EndpointType.V21;
                    case "REST": return EndpointType.REST;
                    default: return EndpointType.V20;
                }
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
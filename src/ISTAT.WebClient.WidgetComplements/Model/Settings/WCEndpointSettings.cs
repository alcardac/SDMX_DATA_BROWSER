using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.WidgetComplements.Model.Settings
{
    public class WCEndpointSettings : ConfigurationSection
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly WCEndpointSettings _instance =
            (WCEndpointSettings)ConfigurationManager.GetSection("EndpointSettings");

        private static readonly EndpointSettings pippo =
              (EndpointSettings)ConfigurationManager.GetSection("EndpointSettings");



        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static WCEndpointSettings WCInstance
        {
            get
            {
                return _instance;
            }
        }

       

        [ConfigurationProperty("Locale",  IsRequired = false)]
        public string Locale
        {
            get { return (string)this["Locale"]; }
            set { this["Locale"] = value; }
        }

        [ConfigurationProperty("IDNode", IsRequired = true)]
        public string IDNode
        {
            get { return (string)this["IDNode"]; }
            set { this["IDNode"] = value; }
        }

        [ConfigurationProperty("Title", IsKey = true, IsRequired = true)]
        public string Title
        {
            get { return (string)this["Title"]; }
            set { this["Title"] = value; }
        }


        [ConfigurationProperty("DecimalSeparator", IsRequired = true)]
        public string DecimalSeparator
        {
            get { return (string)this["DecimalSeparator"]; }
            set { this["DecimalSeparator"] = value; }
        }

        [ConfigurationProperty("Domain", IsRequired = false)]
        public string Domain
        {
            get { return (string)this["Domain"]; }
            set { this["Domain"] = value; }
        }



        [ConfigurationProperty("EnableHTTPAuthentication", IsRequired = false)]
        public bool EnableHTTPAuthentication
        {
            get { return (bool)this["EnableHTTPAuthentication"]; }
            set { this["EnableHTTPAuthentication"] = value; }
        }

        [ConfigurationProperty("EnableProxy", IsRequired = false)]
        public bool EnableProxy
        {
            get { return (bool)this["EnableProxy"]; }
            set { this["EnableProxy"] = value; }
        }

        [ConfigurationProperty("EndPoint", IsRequired = true)]
        public string EndPoint
        {
            get { return (string)this["EndPoint"]; }
            set { this["EndPoint"] = value; }
        }

        [ConfigurationProperty("EndPointV20", IsRequired = true)]
        public string EndPointV20
        {
            get { return (string)this["EndPointV20"]; }
            set { this["EndPointV20"] = value; }
        }

        [ConfigurationProperty("EndPointType", IsRequired = true)]
        public string EndPointType
        {
            get { return (string)this["EndPointType"]; }
            set { this["EndPointType"] = value; }
        }

        [ConfigurationProperty("EndPointSource", IsRequired = true)]
        public string EndPointSource
        {
            get { return (string)this["EndPointSource"]; }
            set { this["EndPointSource"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("Prefix", IsRequired = false)]
        public string Prefix
        {
            get { return (string)this["Prefix"]; }
            set { this["Prefix"] = value; }
        }


        [ConfigurationProperty("ProxyPassword", IsRequired = false)]
        public string ProxyPassword
        {
            get { return (string)this["ProxyPassword"]; }
            set { this["ProxyPassword"] = value; }
        }

        [ConfigurationProperty("ProxyServer", IsRequired = false)]
        public string ProxyServer
        {
            get { return (string)this["ProxyServer"]; }
            set { this["ProxyServer"] = value; }
        }

        [ConfigurationProperty("ProxyServerPort", IsRequired = false)]
        public int ProxyServerPort
        {
            get { return (int)this["ProxyServerPort"]; }
            set { this["ProxyServerPort"] = value; }
        }

        [ConfigurationProperty("ProxyUserName", IsRequired = false)]
        public string ProxyUserName
        {
            get { return (string)this["ProxyUserName"]; }
            set { this["ProxyUserName"] = value; }
        }

        [ConfigurationProperty("UseSystemProxy", IsRequired = false)]
        public bool UseSystemProxy
        {
            get { return (bool)this["UseSystemProxy"]; }
            set { this["UseSystemProxy"] = value; }
        }

        [ConfigurationProperty("UserName", IsRequired = false)]
        public string UserName
        {
            get { return (string)this["UserName"]; }
            set { this["UserName"] = value; }
        }

        [ConfigurationProperty("Wsdl", IsRequired = false)]
        public string Wsdl
        {
            get { return (string)this["Wsdl"]; }
            set { this["Wsdl"] = value; }
        }

        [ConfigurationProperty("Active", IsRequired = false)]
        public bool Active
        {
            get { return (bool)this["Active"]; }
            set { this["Active"] = value; }
        }

        [ConfigurationProperty("UseUncategorysed", IsRequired = false)]
        public bool UseUncategorysed
        {
            get { return (bool)this["UseUncategorysed"]; }
            set { this["UseUncategorysed"] = value; }
        }

        [ConfigurationProperty("UseVirtualDf", IsRequired = false)]
        public bool UseVirtualDf
        {
            get { return (bool)this["UseVirtualDf"]; }
            set { this["UseVirtualDf"] = value; }
        }

        [ConfigurationProperty("Ordinamento", IsRequired = false)]
        public int Ordinamento
        {
            get { return (int)this["Ordinamento"]; }
            set { this["Ordinamento"] = value; }
        }

        [ConfigurationProperty("_TypeEndpoint", IsRequired = false)]
        public int _TypeEndpoint
        {
            get { return (int)this["_TypeEndpoint"]; }
            set { this["_TypeEndpoint"] = value; }
        }




        #endregion
    }
}

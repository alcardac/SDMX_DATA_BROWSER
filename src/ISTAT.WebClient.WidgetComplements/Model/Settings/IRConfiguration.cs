using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;

namespace ISTAT.WebClient.WidgetComplements.Model.Settings
{
    public class IRConfiguration
    {
        public static EndPointRetrieverSection Config = ConfigurationManager.GetSection("EndPointSection") as EndPointRetrieverSection;

        //public static SettingRetriever Settings = ConfigurationManager.GetSection("EndPointSection") as SettingRetriever;

        public static EndPointElement GetEndPointByName(string endPointTitle)
        {
            EndPointElement epRet = null;

            foreach (EndPointElement endPointEl in Config.EndPoints)
            {
                if (endPointEl.Title == endPointTitle)
                    epRet = endPointEl;
            }
            return epRet;
        }

    }

    #region endpoint
    public class EndPointElement : ConfigurationElement 
    {        

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

    }

    [ConfigurationCollection(typeof(EndPointElement))]
    public class EndPointCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EndPointElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EndPointElement)element).Title;
        }
    }
   
    public class EndPointRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("EndPoints", IsDefaultCollection = true)]
        public EndPointCollection EndPoints
        {
            get { return (EndPointCollection)this["EndPoints"]; }
            set { this["EndPoints"] = value; }
        }
    }
    #endregion endpoints

    public class SettingsConfigurationSection : ConfigurationSection
    {

        // Create a "defaultCategoryScheme" element.
        [ConfigurationProperty("Settings", IsRequired = true)]
        public SettingsElement ArtefactFilterList
        {
            get { return (SettingsElement)this["Settings"]; }
            set { this["Settings"] = value; }
        }

    }


    #region SettingsElementSection
    public class SettingsElement : ConfigurationElement
    {
        // Create a "view_tree" attribute.
        [ConfigurationProperty("view_tree", DefaultValue = "true", IsRequired = false)]
        public Boolean ViewTree
        {
            get
            {
                return (Boolean)this["view_tree"];
            }
            set
            {
                this["view_tree"] = value;
            }
        }

        // Create a "view_tree_req" attribute.
        [ConfigurationProperty("view_tree_req", DefaultValue = "true", IsRequired = false)]
        public Boolean ViewTreeReq
        {
            get
            {
                return (Boolean)this["view_tree_req"];
            }
            set
            {
                this["view_tree_req"] = value;
            }
        }

        // Create a "view_tree_select" attribute.
        [ConfigurationProperty("view_tree_select", DefaultValue = "true", IsRequired = false)]
        public Boolean ViewTreeSelect
        {
            get
            {
                return (Boolean)this["view_tree_select"];
            }
            set
            {
                this["view_tree_select"] = value;
            }
        }


    }


    #endregion



}

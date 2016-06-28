using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ISTAT.WebClient.WidgetComplements.Model.Settings
{
    public class EndPointSettings : ConfigurationSection
    {
//        public static EndPointRetrieverSection Config = ConfigurationManager.GetSection("EndPointSection") as EndPointRetrieverSection;

        /*public static EndPointRetrieverSection Config =
             (EndPointRetrieverSection)ConfigurationManager.GetSection("EndPointSection");*/

        #region Constants and Fields

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly EndPointRetrieverSection _config =
            (EndPointRetrieverSection)ConfigurationManager.GetSection("EndPointSection");

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static EndPointRetrieverSection Config
        {
            get
            {
                return _config;
            }
        }
        #endregion
        public static EndPointElement GetEndPointByName(string endPointName)
        {
            EndPointElement epRet = null;

            foreach (EndPointElement endPointEl in Config.EndPoints)
            {
                if (endPointEl.Title == endPointName)
                    epRet = endPointEl;
            }
            return epRet;
        }

    }

    public class EndPointElement : ConfigurationElement
    {
        [ConfigurationProperty("Locale", IsKey = true, IsRequired = true)]
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

        [ConfigurationProperty("Title", IsRequired = true)]
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

        [ConfigurationProperty("Domain", IsRequired = true)]
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

    #region CategoryViewConfigurationSection

/*
    public class CategoryViewConfigurationSection : ConfigurationSection
    {
        // Create a "wsEndPoint" attribute.
        [ConfigurationProperty("wsEndPoint", IsRequired = true)]
        public String WsEndPoint
        {
            get
            {
                return (String)this["wsEndPoint"];
            }
            set
            {
                this["wsEndPoint"] = value;
            }
        }
    */
        // Create a "enableCategoryDropDownList" attribute.
    /*
        [ConfigurationProperty("enableCategoryDropDownList", DefaultValue = "true", IsRequired = false)]
        public Boolean EnableCategoryDropDownList
        {
            get
            {
                return (Boolean)this["enableCategoryDropDownList"];
            }
            set
            {
                this["enableCategoryDropDownList"] = value;
            }
        }
    */

        // Create a "defaultCategoryScheme" element.
    /*
    [ConfigurationProperty("defaultCategoryScheme", IsRequired = false)]
        public DefaultCategorySchemeElement DefaultCategoryScheme
        {
            get { return (DefaultCategorySchemeElement)this["defaultCategoryScheme"]; }
            set { this["defaultCategoryScheme"] = value; }
        }

        // Create a "defaultCategoryScheme" element.
        [ConfigurationProperty("artefactFilterList", IsRequired = true)]
        public ArtefactFilterListElement ArtefactFilterList
        {
            get { return (ArtefactFilterListElement)this["artefactFilterList"]; }
            set { this["artefactFilterList"] = value; }
        }
    
    }
    */
    /*
    public class DefaultCategorySchemeElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = false)]
        [StringValidator(InvalidCharacters = "<>",  MaxLength = 255)]
        public String Id
        {
            get
            {
                return (String)this["id"];
            }
            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("agency", IsRequired = false)]
        [StringValidator(InvalidCharacters = "<>",  MaxLength = 255)]
        public String Agency
        {
            get
            {
                return (String)this["agency"];
            }
            set
            {
                this["agency"] = value;
            }
        }

        [ConfigurationProperty("version", IsRequired = false)]
        [StringValidator(InvalidCharacters = "<>~!@#$%^&*()[]{}/;'\"|\\ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", MaxLength = 10)]
        public String Version
        {
            get
            {
                return (String)this["version"];
            }
            set
            {
                this["version"] = value;
            }
        }
    }


    public class ArtefactFilterListElement : ConfigurationElement
    {
        [ConfigurationProperty("enableCodelist", IsRequired = true)]
        public Boolean EnableCodelist
        {
            get
            {
                return (Boolean)this["enableCodelist"];
            }
            set
            {
                this["enableCodelist"] = value;
            }
        }

        [ConfigurationProperty("enableDataFlow", IsRequired = true)]
        public Boolean EnableDataFlow
        {
            get
            {
                return (Boolean)this["enableDataFlow"];
            }
            set
            {
                this["enableDataFlow"] = value;
            }
        }

        [ConfigurationProperty("enableDsd", IsRequired = true)]
        public Boolean EnableDsd
        {
            get
            {
                return (Boolean)this["enableDsd"];
            }
            set
            {
                this["enableDsd"] = value;
            }
        }

        [ConfigurationProperty("enableConceptScheme", IsRequired = true)]
        public Boolean EnableConceptScheme
        {
            get
            {
                return (Boolean)this["enableConceptScheme"];
            }
            set
            {
                this["enableConceptScheme"] = value;
            }
        }


    }
    */
    #endregion



}

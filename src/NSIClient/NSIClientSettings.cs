// -----------------------------------------------------------------------
// <copyright file="NSIClientSettings.cs" company="EUROSTAT">
//   Date Created : 2010-11-02
//   Copyright (c) 2009, 2015 by the European Commission, represented by Eurostat.   All rights reserved.
// 
// Licensed under the EUPL, Version 1.1 or – as soon they
// will be approved by the European Commission - subsequent
// versions of the EUPL (the "Licence");
// You may not use this work except in compliance with the
// Licence.
// You may obtain a copy of the Licence at:
// 
// https://joinup.ec.europa.eu/software/page/eupl 
// 
// Unless required by applicable law or agreed to in
// writing, software distributed under the Licence is
// distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the Licence for the specific language governing
// permissions and limitations under the Licence.
// </copyright>
// -----------------------------------------------------------------------
namespace Estat.Nsi.Client
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Net;
    using System.Web;
    using System.Web.Configuration;

    using Estat.Nsi.Client.Properties;

    /// <summary>
    /// Configuration section &lt;NSIClient&gt;
    /// </summary>
    /// <remarks>
    /// Assign properties to your child class that has the attribute 
    /// <c>[ConfigurationProperty]</c> to store said properties in the xml.
    /// </remarks>
    public sealed class NSIClientSettings : ConfigurationSection
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly NSIClientSettings _instance =
            (NSIClientSettings)ConfigurationManager.GetSection(typeof(NSIClientSettings).Name);

        /// <summary>
        /// The _ config.
        /// </summary>
        private Configuration _config;


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="NSIClientSettings"/> class from being created. 
        /// Private Constructor used by our factory method.
        /// </summary>
        private NSIClientSettings()
        {
            // Allow this section to be stored in user.app. By default this is forbidden.
            this.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current singleton instance
        /// </summary>
        public static NSIClientSettings Instance
        {
            get
            {
                return _instance;
            }
        }

        /*
         *  Uncomment the following section and add a Configuration Collection 
         *  from the with the file named NSIClient.cs
         */

        // /// <summary>
        // /// A custom XML section for an application's configuration file.
        // /// </summary>
        // [ConfigurationProperty("customSection", IsDefaultCollection = true)]
        // public NSIClientCollection NSIClient
        // {
        // get { return (NSIClientCollection) base["customSection"]; }
        // }

        /// <summary>
        /// Gets or sets the Domain
        /// </summary>
        [ConfigurationProperty("Domain")]
        [Description("Domain")]
        [Category("HTTP Authentication")]
        public string Domain
        {
            get
            {
                return (string)this["Domain"];
            }

            set
            {
                this["Domain"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP Authentication is enabled.
        /// </summary>
        [ConfigurationProperty("EnableHTTPAuthentication", DefaultValue = false)]
        [Description("Enable HTTP Authentication")]
        [Category("HTTP Authentication")]
        [DefaultValue(false)]
        public bool EnableHTTPAuthentication
        {
            get
            {
                return (bool)this["EnableHTTPAuthentication"];
            }

            set
            {
                this["EnableHTTPAuthentication"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Proxy is enabled
        /// </summary>
        [ConfigurationProperty("EnableProxy", DefaultValue = false)]
        [Description("Enable Proxy")]
        [Category("Proxy")]
        [DefaultValue(false)]
        public bool EnableProxy
        {
            get
            {
                return (bool)this["EnableProxy"];
            }

            set
            {
                this["EnableProxy"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        [ConfigurationProperty("EndPoint" /*,IsRequired = true*/)]
        [Description("Web Service Endpoint URL")]
        [Category("Endpoint")]
        public string EndPoint
        {
            get
            {
                return (string)this["EndPoint"];
            }

            set
            {
                this["EndPoint"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        [ConfigurationProperty("EndPointV20")]
        [Description("Web Service EndpointV20 URL")]
        [Category("EndpointV20")]
        public string EndPointV20
        {
            get
            {
                return (string)this["EndPointV20"];
            }

            set
            {
                this["EndPointV20"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Web Service EndpointType 
        /// </summary>
        [ConfigurationProperty("EndPointType" ,IsRequired = true)]
        [Description("Web Service Endpoint Type")]
        [Category("Endpoint")]
        public string EndPointType
        {
            get
            {
                return (string)this["EndPointType"];
            }

            set
            {
                this["EndPointType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SDMX input/output traffic should be logged or not
        /// </summary>
        [ConfigurationProperty("logSDMX", DefaultValue = false)]
        [Description("Log the SDMX input/output traffic or not")]
        [Category("Log")]
        public bool LogSDMX
        {
            get
            {
                return (bool)this["logSDMX"];
            }

            set
            {
                this["logSDMX"] = value;
            }
        }

      
        /// <summary>
        /// Gets or sets the HTTP Password
        /// </summary>
        [ConfigurationProperty("Password")]
        [Description("HTTP Password")]
        [Category("HTTP Authentication")]
        [PasswordPropertyText(true)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }

            set
            {
                this["Password"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Namespace prefix
        /// </summary>
        [ConfigurationProperty("Prefix", DefaultValue = "web")]
        [Description("Web service Namespace prefix")]
        [Category("Endpoint")]
        [DefaultValue("web")]
        public string Prefix
        {
            get
            {
                return (string)this["Prefix"];
            }

            set
            {
                this["Prefix"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Proxy Password
        /// </summary>
        [ConfigurationProperty("ProxyPassword")]
        [Description("Proxy Password")]
        [Category("Proxy")]
        [PasswordPropertyText(true)]
        public string ProxyPassword
        {
            get
            {
                return (string)this["ProxyPassword"];
            }

            set
            {
                this["ProxyPassword"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Proxy server
        /// </summary>
        [ConfigurationProperty("ProxyServer")]
        [Description("Proxy server")]
        [Category("Proxy")]
        public string ProxyServer
        {
            get
            {
                return (string)this["ProxyServer"];
            }

            set
            {
                this["ProxyServer"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Proxy port
        /// </summary>
        [ConfigurationProperty("ProxyServerPort", DefaultValue = 0, IsRequired = false)]
        [IntegerValidator(MinValue = 0, MaxValue = 65535, ExcludeRange = false)]
        [Description("Proxy port")]
        [Category("Proxy")]
        public int ProxyServerPort
        {
            get
            {
                return (int)this["ProxyServerPort"];
            }

            set
            {
                this["ProxyServerPort"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Proxy User name
        /// </summary>
        [ConfigurationProperty("ProxyUsername")]
        [Description("Proxy User name")]
        [Category("Proxy")]
        public string ProxyUserName
        {
            get
            {
                return (string)this["ProxyUsername"];
            }

            set
            {
                this["ProxyUsername"] = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable use of <see cref="System.Net.WebRequest.DefaultWebProxy"/> proxy configuration
        /// </summary>
        [ConfigurationProperty("UseSystemProxy")]
        [Description("Use default proxy configuration")]
        [Category("Proxy")]
        [DefaultValue(false)]
        public bool UseSystemProxy
        {
            get
            {
                return (bool)this["UseSystemProxy"];
            }

            set
            {
                this["UseSystemProxy"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the HTTP User name
        /// </summary>
        [ConfigurationProperty("Username")]
        [Description("HTTP User name")]
        [Category("HTTP Authentication")]
        public string UserName
        {
            get
            {
                return (string)this["Username"];
            }

            set
            {
                this["Username"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        [ConfigurationProperty("WSDL")]
        [Description("Web Service WSDL URL")]
        [Category("Endpoint")]
        public string Wsdl
        {
            get
            {
                return (string)this["WSDL"];
            }

            set
            {
                this["WSDL"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Web Service Endpoint URL
        /// </summary>
        [ConfigurationProperty("WSDLV20")]
        [Description("Web Service WSDLV20 URL")]
        [Category("EndpointV20")]
        public string WsdlV20
        {
            get
            {
                return (string)this["WSDLV20"] != "" && (string)this["WSDLV20"] != null ? (string)this["WSDLV20"] : EndPointV20 + "?wsdl";
            }

            set
            {
                this["WSDLV20"] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the current applications &lt;NSIClient&gt; section.
        /// </summary>
        /// <param name="configLevel">
        /// The &lt;ConfigurationUserLevel&gt; that the config file
        /// is retrieved from.
        /// </param>
        /// <returns>
        /// The configuration file's &lt;NSIClient&gt; section.
        /// </returns>
        public static NSIClientSettings GetSection(ConfigurationUserLevel configLevel)
        {
            /* 
             * This class is setup using a factory pattern that forces you to
             * name the section &lt;NSIClient&gt; in the config file.
             * If you would prefer to be able to specify the name of the section,
             * then remove this method and mark the constructor public.
             */
            Configuration config = null;
            HttpContext ctx = HttpContext.Current;
            NSIClientSettings clientSettings = null;

            if (ctx != null)
            {
                string virtualPath = ctx.Request.ApplicationPath;

                try
                {
                    config = WebConfigurationManager.OpenWebConfiguration(virtualPath);
                }
                catch (ConfigurationErrorsException ex)
                {
                    string message = Resources.ExceptionReadingConfiguration + ex.Message;
                    if (virtualPath != null)
                    {
                        message = string.Format(
                            CultureInfo.InvariantCulture, 
                            Resources.InfoWebConfigLocation, 
                            message, 
                            ctx.Server.MapPath(virtualPath));
                    }

                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(message);
                    try
                    {
                        var map = new WebConfigurationFileMap();
                        var vd = new VirtualDirectoryMapping(ctx.Server.MapPath(virtualPath), true);

                        map.VirtualDirectories.Add(virtualPath, vd);
                        config = WebConfigurationManager.OpenMappedWebConfiguration(map, virtualPath);
                    }
                    catch (ConfigurationException e)
                    {
                        Console.WriteLine(e.ToString());

                        // read-only..
                        clientSettings =
                            (NSIClientSettings)
                            WebConfigurationManager.GetWebApplicationSection(typeof(NSIClientSettings).Name);
                    }

                    // throw new NSIClientException(message, ex);
                }
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(configLevel);
            }

            if (config != null)
            {
                clientSettings = (NSIClientSettings)config.GetSection("NSIClientSettings");
                if (clientSettings == null)
                {
                    clientSettings = new NSIClientSettings();
                    config.Sections.Add("NSIClientSettings", clientSettings);
                }

                clientSettings._config = config;
            }
            else
            {
                if (clientSettings == null)
                {
                    throw new NsiClientException("Missing NSIClientSettings. Cannot add NSIClientSettings settings");
                }
            }

            return clientSettings;
        }

        /// <summary>
        /// Saves the configuration to the config file.
        /// </summary>
        public void Save()
        {
            this._config.Save();
        }

        #endregion
    }
}
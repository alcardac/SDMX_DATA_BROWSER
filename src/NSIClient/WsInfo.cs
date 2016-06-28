// -----------------------------------------------------------------------
// <copyright file="WsInfo.cs" company="EUROSTAT">
//   Date Created : 2013-09-13
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
    using System.Net;

    using Org.Sdmxsource.Sdmx.Api.Constants;

    using RestSharp;

    /// <summary>
    /// Web Service information &lt;NSIClient&gt;
    /// </summary>
    public class WsInfo
   {
       #region Constants and Fields

       private readonly NSIClientSettings _config;
       private readonly SdmxSchemaEnumType _schemaVersion;
       private readonly EndpointType _endpointType;
       private readonly string _endpoint;
       private readonly string _wsdl;

       #endregion

       #region Constructors and Destructors


       /// <summary>
       /// Constructor
       /// </summary>
       public WsInfo(NSIClientSettings config, SdmxSchemaEnumType schemaVersion)
       {
           _config = config;
           _schemaVersion = schemaVersion;

           //endpointType
           bool endpointTypeOk = Enum.TryParse(_config.EndPointType, true, out _endpointType);
           if (!endpointTypeOk)
           {
               _endpointType = EndpointType.V20;
           }

           //endpoint, wsdl
           if (_schemaVersion == SdmxSchemaEnumType.VersionTwo)
           {
              if (_endpointType == EndpointType.V20 && _config.EndPoint != null)
              {
                  _endpoint = _config.EndPoint;
                  _wsdl = _config.Wsdl;
              }
              else
              {
                  _endpoint = _config.EndPointV20;
                  _wsdl = _config.WsdlV20;
              }
           }
           else
           {
                  _endpoint = _config.EndPoint;
                  _wsdl = _config.Wsdl;
           }
       }

       #endregion

       #region Public Properties

       /// <summary>
       /// Gets the Config
       /// </summary>
       public NSIClientSettings Config
       {
           get
           {
                return _config;
           }
       }
        
       /// <summary>
       /// Gets the Domain
       /// </summary>
       public string Domain
       {
           get
           {
            return _config.Domain;
           }
       }

       /// <summary>
       /// Gets a value indicating whether HTTP Authentication is enabled.
       /// </summary>
       public bool EnableHTTPAuthentication
       {
           get
           {
               return _config.EnableHTTPAuthentication;
           }
       }

       /// <summary>
       /// Gets a value indicating whether the Proxy is enabled
       /// </summary>
       public bool EnableProxy
       {
           get
           {
               return _config.EnableProxy;
           }
       }

       /// <summary>
       /// Gets the Web Service Endpoint URL
       /// </summary>
       public string EndPoint
       {
           get
           {
               return _endpoint;
           }
       }

       /// <summary>
       /// Gets the Web Service EndpointType 
       /// </summary>
       public EndpointType EndPointType
       {
           get
           {
               return _endpointType;
           }
       }

       /// <summary>
       /// Gets a value indicating whether the SDMX input/output traffic should be logged or not
       /// </summary>
       public bool LogSDMX
       {
           get
           {
               return _config.LogSDMX;
           }
       }

       /// <summary>
       /// Gets the HTTP Password
       /// </summary>
       public string Password
       {
           get
           {
               return _config.Password;
           }
       }

       /// <summary>
       /// Gets the Namespace prefix
       /// </summary>
       public string Prefix
       {
           get
           {
               return _config.Prefix;
           }
       }

       /// <summary>
       /// Gets the Proxy Password
       /// </summary>
       public string ProxyPassword
       {
           get
           {
               return _config.ProxyPassword;
           }
       }

       /// <summary>
       /// Gets the Proxy server
       /// </summary>
       public string ProxyServer
       {
           get
           {
               return _config.ProxyServer;
           }
       }

       /// <summary>
       /// Gets the Proxy port
       /// </summary>
       public int ProxyServerPort
       {
           get
           {
               return _config.ProxyServerPort;
           }
       }

       /// <summary>
       /// Gets the Proxy User name
       /// </summary>
       public string ProxyUserName
       {
           get
           {
               return _config.ProxyUserName;
           }
       }

       /// <summary>
       /// Gets a value indicating whether to enable or disable use of <see cref="System.Net.WebRequest.DefaultWebProxy"/> proxy configuration
       /// </summary>
       public bool UseSystemProxy
       {
           get
           {
               return _config.UseSystemProxy;
           }
       }

       /// <summary>
       /// Gets the HTTP User name
       /// </summary>
       public string UserName
       {
           get
           {
               return _config.UserName;
           }
       }

       /// <summary>
       /// Gets the Web Service Endpoint URL
       /// </summary>
       public string Wsdl
       {
           get
           {
               return _wsdl;
           }
       }
        
       #endregion

       #region Public Methods

       /// <summary>
       /// Setup the specified <paramref name="webRequest"/> network authentication and proxy configuration
       /// </summary>
       /// <param name="webRequest">
       /// The <see cref="WebRequest"/>
       /// </param>
       public void SetupWebRequestAuth(WebRequest webRequest)
       {
           if (this.EnableHTTPAuthentication)
           {
               webRequest.Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain);
           }

           if (this.EnableProxy)
           {
               if (this.UseSystemProxy)
               {
                   webRequest.Proxy = WebRequest.DefaultWebProxy;
               }
               else
               {
                   WebProxy proxy = new WebProxy(this.ProxyServer, this.ProxyServerPort);
                   if (!string.IsNullOrEmpty(this.ProxyUserName) || !string.IsNullOrEmpty(this.ProxyPassword))
                   {
                       proxy.Credentials = new NetworkCredential(this.ProxyUserName, this.ProxyPassword);
                   }

                   webRequest.Proxy = proxy;
               }
           }
       }

       /// <summary>
       /// Setup the specified <paramref name="client"/> network authentication and proxy configuration
       /// </summary>
       /// <param name="client">
       /// The <see cref="client"/>
       /// </param>
       public void SetupRestAuth(RestClient client, RestRequest restRequest)
       {
           if (this.EnableHTTPAuthentication)
           {
              restRequest.Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain);
           }

           if (this.EnableProxy)
           {
               if (this.UseSystemProxy)
               {
                   client.Proxy = WebRequest.DefaultWebProxy;
               }
               else
               {
                   WebProxy proxy = new WebProxy(this.ProxyServer, this.ProxyServerPort);
                   if (!string.IsNullOrEmpty(this.ProxyUserName) || !string.IsNullOrEmpty(this.ProxyPassword))
                   {
                       proxy.Credentials = new NetworkCredential(this.ProxyUserName, this.ProxyPassword);
                   }

                   client.Proxy = proxy;
               }
           }
       }

       #endregion
   }
}

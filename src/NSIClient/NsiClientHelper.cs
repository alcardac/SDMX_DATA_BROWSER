// -----------------------------------------------------------------------
// <copyright file="NsiClientHelper.cs" company="EUROSTAT">
//   Date Created : 2013-08-30
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    using Estat.Nsi.Client.Properties;

    using log4net;

    using Org.Sdmxsource.Sdmx.Api.Constants;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Util.Objects.Reference;

    public static  class NsiClientHelper
    {

        /// <summary>
        /// Log class
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(NsiClientHelper));

        /// <summary>
        /// Handle a SOAP Fault from the WS. It will parse the soap details and throw an NsiClientException
        /// </summary>
        /// <param name="ex">
        /// The soap fault
        /// </param>
        /// <exception cref="NsiClientException">
        /// It is always thrown
        /// </exception>
        public static void HandleSoapFault(WebException ex)
        {
            var error = new StringBuilder(Resources.ExceptionServerResponse);
            error.AppendLine(ex.Message);
            if (ex.Response != null)
            {
                if (ex.Response is HttpWebResponse && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new DataflowException(Resources.NoResultsFound);
                }
                error.AppendLine(Resources.ExceptionReceivedSoapFault);
                XmlDocument fault = null;
                using (Stream stream = ex.Response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        fault = new XmlDocument();
                        fault.Load(stream);
                      //  error.Append(fault.InnerText);
                    }
                }
            
                //Hahaha Production flag. This is due to poor design of app, NSI WS, DR and SR
                if (fault != null)
                {
                    SdmxFault sdmxFault = SdmxFault.GetErrorNumber(fault);
                    if (sdmxFault.ErrorNumber == 110 || sdmxFault.ErrorMessage.Equals(Resources.Unauthorized, StringComparison.OrdinalIgnoreCase)
                        || sdmxFault.ErrorMessage.Equals(Resources.NoResultsFound, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataflowException(Resources.NoResultsFound);
                    }
                }
            }

            Logger.Error(error.ToString());
            Logger.Error(ex.Message, ex);
            throw new NsiClientException(error.ToString(), ex);
        }

        /// <summary>
        /// Handle WSDL related exception and return an <see cref="NsiClientException"/>
        /// </summary>
        /// <param name="endPoint">
        /// The endPoint
        /// </param>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <param name="wsdlUrl">
        /// The wsdl url.
        /// </param>
        /// <returns>
        /// A <see cref="NsiClientException"/>
        /// </returns>
        public static NsiClientException HandleWsdlException(string endPoint, Exception ex, string wsdlUrl)
        {
            string message = string.Format(
                CultureInfo.InvariantCulture, Resources.ExceptionCannotLoadWsdlFormat2, wsdlUrl, endPoint);
            Logger.Error(message);
            Logger.Error(ex.Message, ex);
            return new NsiClientException(message, ex);
        }

        /// <summary>
        /// Handle WSDL related exception and return an <see cref="DataflowException"/>
        /// </summary>
        public static NsiClientException HandleWsdlException(string endPoint, WebException ex, string wsdlUrl)
        {
            if (ex.Response is HttpWebResponse && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new DataflowException(Resources.NoResultsFound);
            }
            return HandleWsdlException(endPoint, (Exception)ex, wsdlUrl);
        }

        /// <summary>
        /// Log the SDMX-ML message stored in the stream.
        /// </summary>
        /// <param name="config">
        /// The config
        /// </param>
        /// <param name="request">
        /// The stream to the SDMX-ML message
        /// </param>
        public static void LogSdmx(WsInfo config, XmlDocument request)
        {
            if (config.LogSDMX)
            {
                Logger.Info(request.InnerXml);
            }
        }

        /// <summary>
        /// Log the SDMX-ML message stored in the stream.
        /// </summary>
        /// <param name="config">
        /// The config
        /// </param>
        /// <param name="request">
        /// The stream to the SDMX-ML message
        /// </param>
        public static void LogSdmx(WsInfo config, string request)
        {
            if (config.LogSDMX)
            {
                Logger.Info(request);
            }
        }

        /// <summary>
        /// Log the SDMX-ML message stored in the stream.
        /// </summary>
        /// <param name="config">
        /// The config
        /// </param>
        /// <param name="tempFileName">
        /// The temporary file name
        /// </param>
        /// <param name="prefix">
        /// A prefix message to log before the SDMX-ML message
        /// </param>
        public static void LogSdmx(WsInfo config, string tempFileName, string prefix)
        {
            if (config.LogSDMX)
            {
                Logger.Info(prefix);
                using(var reader = new StreamReader(tempFileName, Encoding.UTF8))
                {
                    Logger.Info(reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Build concept scheme requests from the concept scheme references of the specified KeyFamilyBean object
        /// </summary>
        /// <param name="kf">
        /// The KeyFamily to look for concept Scheme references
        /// </param>
        /// <returns>
        /// A list of concept scheme requests 
        /// </returns>
        public static IEnumerable<IStructureReference> BuildConceptSchemeRequest(IDataStructureObject kf)
        {
            var conceptSchemeSet = new Dictionary<string, object>();
            var ret = new List<IStructureReference>();
            var crossDsd = kf as ICrossSectionalDataStructureObject;

            List<IComponent> components = new List<IComponent>();

            components.AddRange(kf.GetDimensions());
            components.AddRange(kf.Attributes);
            if (kf.PrimaryMeasure != null)
            {
                components.Add(kf.PrimaryMeasure);
            }
            if (crossDsd != null)
            {
                components.AddRange(crossDsd.CrossSectionalMeasures);
            }

            ICollection<IComponent> comps = components;

            foreach (IComponent comp in comps)
            {
                string key = Utils.MakeKey(comp.ConceptRef.MaintainableReference.MaintainableId, comp.ConceptRef.MaintainableReference.Version, comp.ConceptRef.MaintainableReference.AgencyId);
                if (!conceptSchemeSet.ContainsKey(key))
                {
                    // create concept ref


                    var conceptSchemeRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.ConceptScheme))
                    {
                        MaintainableId = comp.ConceptRef.MaintainableReference.MaintainableId,
                        AgencyId = comp.ConceptRef.MaintainableReference.AgencyId,
                        Version = comp.ConceptRef.MaintainableReference.Version
                    };

                    // add it to request
                    ret.Add(conceptSchemeRef);

                    // added it to set of visited concept schemes
                    conceptSchemeSet.Add(key, null);
                }
            }

            return ret;
        }

        /// <summary>
        /// Wrap the Delete method with an exception handler.
        /// </summary>
        public static bool TryToDelete(string f)
        {
            try
            {
                // Try to delete the file.
                if (f != null)
                {
                    File.Delete(f);
                }
                return true;
            }
            catch (IOException)
            {
                // We could not delete the file.
                return false;
            }
        }

        public static bool DataflowDsdIsCrossSectional(IDataStructureObject dsd)
        {
            return dsd is ICrossSectionalDataStructureObject;
        }
        public static IDataStructureObject GetDsdFromDataflow(IDataflowObject dataflow, ISet<IDataStructureObject> dataStructure)
        {
            foreach (var dsd in dataStructure)
            {
                if (dataflow.DataStructureRef.Equals(dsd.AsReference))
                {
                    return dsd;
                }
            }
            return null;
        }
    }
}

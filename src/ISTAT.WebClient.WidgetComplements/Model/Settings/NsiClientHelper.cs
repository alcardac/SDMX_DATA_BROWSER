// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NsiClientHelper.cs" company="Eurostat">
//   Date Created : 2013-08-24
//   Copyright (c) 2013 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   An implementation of the <see cref="INsiClient" /> that retrieves structural metadata and data from a SDMX v2.0 web service
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace ISTAT.WebClient.WidgetComplements.Model.Settings
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;
    using ISTAT.WebClient.WidgetComplements.Model.Properties;
    using log4net;
    using System.Collections.Generic;
    using Org.Sdmxsource.Sdmx.Api.Constants;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
    using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
    using ISTAT.WebClient.WidgetComplements.Model.Exceptions;
    using ISTAT.WebClient.WidgetComplements.Model.CallWS;

    public static class NsiClientHelper
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

        public static void LogSdmx(string tempFileName, string prefix)
        {
            Logger.Info(prefix);
            using (var reader = new StreamReader(tempFileName, Encoding.UTF8))
            {
                Logger.Info(reader.ReadToEnd());
            }
        }
        public static void LogSdmx(XmlDocument request)
        {
            Logger.Info(request.InnerXml);
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

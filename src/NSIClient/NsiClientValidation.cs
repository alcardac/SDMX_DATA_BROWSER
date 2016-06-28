// -----------------------------------------------------------------------
// <copyright file="NsiClientValidation.cs" company="EUROSTAT">
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
    using System.Linq;
    using System.Text;

    using Estat.Nsi.Client.Properties;

    using log4net;

    using Org.Sdmxsource.Sdmx.Api.Model.Objects;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

    public static class NsiClientValidation
    {
        /// <summary>
        /// Log class
        /// </summary>
       private static readonly ILog Logger = LogManager.GetLogger(typeof(NsiClientValidation));

        /// <summary>
        /// Check if the specified structure has all referenced concept schemes from the first keyfamily
        /// </summary>
        /// <param name="structure">
        /// The StructureBean to check
        /// </param>
        public static void CheckConcepts(ISdmxObjects structure)
        {
            var cshtMap = new Dictionary<string, IConceptSchemeObject>();
            IDataStructureObject kf = structure.DataStructures.First();
            foreach (IConceptSchemeObject c in structure.ConceptSchemes)
            {
                cshtMap.Add(Utils.MakeKey(c), c);
            }

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

            var comps = components;

            foreach (IComponent comp in comps)
            {
                string conceptKey = Utils.MakeKey(comp.ConceptRef.MaintainableReference.MaintainableId,
                    comp.ConceptRef.MaintainableReference.AgencyId, comp.ConceptRef.MaintainableReference.Version);
                if (!cshtMap.ContainsKey(conceptKey))
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Resources.ExceptionMissingConceptSchemeFormat1, conceptKey);
                    Logger.Error(message);
                    throw new NsiClientException(message);
                }
            }
        }


        /// <summary>
        /// Check the Status of a QueryStructureResponse.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// If it doesn't exist, do nothing. Maybe there is a bug at the NSI WS implementation so we ignore it hoping that the rest of the message is ok 
        /// </item>
        /// <item>
        /// if the status is Success do nothing.
        /// </item>
        /// <item>
        /// If the status is Warning only log the messages with warning.
        /// </item>
        /// <item>
        /// if the status is failure throw an NsiClientException
        /// </item>
        /// <item>
        /// if Response or response structure are null. An exception is thrown 
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="response">
        /// The QueryStructureResponse to check
        /// </param>
        public static void CheckResponse(ISdmxObjects response)
        {
            var error = new StringBuilder();
            if (response == null)
            {
                error.Append(Resources.ExceptionMissingResponse);
            }

            if (error.Length > 0)
            {
                Logger.Error(error.ToString());
                throw new NsiClientException(error.ToString());
            }
        }

        /// <summary>
        /// Checks if a structure is complete according to the requirements of <see cref="GetStructure"/>
        /// </summary>
        /// <param name="structure">
        /// The StructureBean object to check.
        /// </param>
        /// <param name="dataflow">
        /// The requested dataflow
        /// </param>
        /// <exception cref="NsiClientException">
        /// Server response error
        /// </exception>
        public static void CheckifStructureComplete(ISdmxObjects structure, IDataflowObject dataflow)
        {
            if (structure.DataStructures.Count != 1)
            {
                Logger.Error(Resources.ExceptionKeyFamilyCountNot1);
                throw new NsiClientException(Resources.ExceptionKeyFamilyCountNot1);
            }

            IDataStructureObject kf = structure.DataStructures.First();
            var keyFamilyRef = dataflow.DataStructureRef;
            if (kf.Id == null || keyFamilyRef == null || !kf.Id.Equals(keyFamilyRef.MaintainableReference.MaintainableId)
                || !kf.AgencyId.Equals(keyFamilyRef.MaintainableReference.AgencyId) || !kf.Version.Equals(keyFamilyRef.MaintainableReference.Version))
            {
                Logger.Error(Resources.ExceptionServerResponseInvalidKeyFamily);
                throw new NsiClientException(Resources.ExceptionServerResponseInvalidKeyFamily);
            }
        }

    }
}

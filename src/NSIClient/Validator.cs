// -----------------------------------------------------------------------
// <copyright file="Validator.cs" company="EUROSTAT">
//   Date Created : 2010-11-11
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
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

    /// <summary>
    /// This is a utility class for checking a dsd for compliance 
    /// with producing Compact or Cross-Sectional Data. 
    /// </summary>
    internal static class Validator
    {
        #region Public Methods

        /// <summary>
        /// This method checks a dsd for compliance with producing Compact Data. 
        /// In detail, it checks that if a TimeDimension is present and at least
        /// one dimension is frequency dimension. If there is none an error message
        /// is returned to the caller
        /// </summary>
        /// <param name="dsd">
        /// The <see cref="Estat.Sdmx.Model.Structure.KeyFamilyBean"/>of the DSD to be checked
        /// </param>
        /// <returns>
        /// The error messages in case of invalid dsd or an empty string in case a valid dsd
        /// </returns>
        public static string ValidateForCompact(IDataStructureObject dsd)
        {
            string text = string.Empty;
            bool isFrequency = false;

            foreach (IDimension dimension in dsd.DimensionList.Dimensions)
            {
                if (dimension.FrequencyDimension)
                {
                    isFrequency = true;
                    break;
                }
            }

            if (dsd.TimeDimension == null)
            {
                // text= "Dsd does not have at least one Frequency dimension";
                text = "DSD " + dsd.Id + " v" + dsd.Version
                       + " does not have a Time Dimension. Only Cross-Sectional data may be requested.";
            }
            else if (!isFrequency)
            {
                // normally it should never reach here
                text = "DSD " + dsd.Id + " v" + dsd.Version
                       +
                       " does not have a Frequency dimension. According SDMX v2.0: Any DSD which uses the Time dimension must also declare a frequency dimension.";
            }

            return text;
        }

/*
        /// <summary>
        /// This method checks a dsd for compliance with producing  Cross-Sectional Data. 
        /// In detail, it checks all Dimensions and all Attributes for having or not having 
        /// cross-sectional attachment group. If there are components with no attachment level, 
        /// it returns a list with them in the message. 
        /// </summary>
        /// <param name="dsd">
        /// The <see cref="Estat.Sdmx.Model.Structure.KeyFamilyBean"/>of the DSD to be checked
        /// </param>
        /// <returns>
        /// The error messages in case of invalid dsd or an empty string in case a valid dsd
        /// </returns>
        public static string ValidateForCrossSectional(KeyFamilyBean dsd)
        {
            // this is where the message of a possible exception will be gradually built up
            StringBuilder crosstext = new StringBuilder();

            foreach (DimensionBean dimension in dsd.Dimensions)
            {
                if (!Utils.IsTrueString(dimension.CrossSectionalAttachDataSet)
                    && !Utils.IsTrueString(dimension.CrossSectionalAttachGroup)
                    && !Utils.IsTrueString(dimension.CrossSectionalAttachSection)
                    && !Utils.IsTrueString(dimension.CrossSectionalAttachObservation)
                    && !Utils.IsTrueString(dimension.IsMeasureDimension) && !Utils.IsTrueString(dimension.IsFrequencyDimension))
                {
                    crosstext.AppendFormat(
                        "Dimension {0} does not have a Cross-Sectional attachment level.\n", dimension.ConceptRef);
                }
            }

            foreach (AttributeBean attribute in dsd.Atributes)
            {
                if (!Utils.IsTrueString(attribute.CrossSectionalAttachDataSet)
                    && !Utils.IsTrueString(attribute.CrossSectionalAttachGroup)
                    && !Utils.IsTrueString(attribute.CrossSectionalAttachSection)
                    && !Utils.IsTrueString(attribute.CrossSectionalAttachObservation))
                {
                    crosstext.AppendFormat(
                        "Attribute {0} does not have a Cross-Sectional attachment level.\n", attribute.ConceptRef);
                }
            }

            return crosstext.ToString();
        }
        */
        #endregion

        #region Methods

        #endregion
    }
}
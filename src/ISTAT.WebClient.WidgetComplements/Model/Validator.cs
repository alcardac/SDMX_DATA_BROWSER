// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validator.cs" company="Eurostat">
//   Date Created : 2010-11-20
//   Copyright (c) 2010 by the European   Commission, represented by Eurostat. All rights reserved.
//   Licensed under the European Union Public License (EUPL) version 1.1. 
//   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// <summary>
//   This is a utility class for checking a dsd for compliance
//   with producing Compact or Cross-Sectional Data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ISTAT.WebClient.WidgetComplements.Model
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

        
        #endregion

       
    }
}
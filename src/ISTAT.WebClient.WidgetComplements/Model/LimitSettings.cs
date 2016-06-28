using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
namespace ISTAT.WebClient.WidgetComplements.Model
{
    class LimitSettings
    {
    }
}
*/

namespace ISTAT.WebClient.WidgetComplements.Model
{
    using System.Configuration;

    /// <summary>
    /// This configuration stores the various limits for the Web Application
    /// </summary>
    internal class LimitSettings : ConfigurationSection
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly LimitSettings _instance =
            (LimitSettings)ConfigurationManager.GetSection("LimitSettings");

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static LimitSettings Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Gets or sets the max obs
        /// </summary>
        [ConfigurationProperty(Constants.MaxObs, DefaultValue = 0)]
        public int ConfiguredMaxObs
        {
            get
            {
                return (int)this[Constants.MaxObs];
            }

            set
            {
                if (value < 0)
                {
                    this[Constants.MaxObs] = 0;
                }
                else
                {
                    this[Constants.MaxObs] = value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get if a maximum observation limit is configured
        /// </summary>
        /// <returns>
        /// True if there a maximum observation limit configured and it's value is larger than 0
        /// </returns>
        public bool HasMaxObs()
        {
            return (int)this[Constants.MaxObs] > 0;
        }

        #endregion
    }
}
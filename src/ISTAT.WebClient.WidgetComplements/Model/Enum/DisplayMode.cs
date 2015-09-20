using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.Enum
{
    /// <summary>
    /// Enumation of the display modes for layout cells
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Show only description
        /// </summary>
        Description = 0,

        /// <summary>
        /// Show code and description.
        /// TODO Check if this can be done with flags
        /// </summary>
        CodeDescription = 1,

        /// <summary>
        /// Show only codes
        /// </summary>
        Code = 2
    }
}

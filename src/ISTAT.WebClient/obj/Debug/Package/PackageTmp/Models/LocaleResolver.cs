using ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ISTAT.WebClient.Models
{
    /// <summary>
    /// This class handles the user specified locale cookies
    /// </summary>
    public static class LocaleResolver
    {
        #region Public Methods

        /// <summary>
        /// Get the locale from the cookie from the specified Context if available. Else return the default.
        /// </summary>
        /// <param name="context">
        /// The HTTP Context to get the cookie from
        /// </param>
        /// <returns>
        /// If the lang cookie is available, the cookie. Else the default from <see cref="I18NSupport.DefaultLocale"/>
        /// </returns>
        public static CultureInfo GetCookie(HttpContext context)
        {
            CultureInfo locale = CultureInfo.CurrentUICulture;
            HttpCookie cookie = context.Request.Cookies["lang"];
            if (cookie != null)
            {
                string value = cookie.Value;
                locale = Messages.GetLocale(value) ?? CultureInfo.CurrentUICulture;
            }

            return locale;
        }

        /// <summary>
        /// Remove cookie from browser
        /// </summary>
        /// <param name="context">
        /// The HTTP Context to add the cookie to
        /// </param>
        public static void RemoveCookie(HttpContext context)
        {
            HttpCookie cookie = new HttpCookie("lang", CultureInfo.CurrentUICulture.Name);
            cookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Send a cookie with the specified locale using the specified context
        /// </summary>
        /// <param name="locale">
        /// The locale string. Must be a valid locale name see <see cref="System.Globalization.CultureInfo"/>
        /// </param>
        /// <param name="context">
        /// The HTTP Context to add the cookie to
        /// </param>
        public static void SendCookie(CultureInfo locale, HttpContext context)
        {
            HttpCookie cookie = new HttpCookie("lang", locale.Name);
            cookie.Expires = DateTime.Now.AddDays(14);
            context.Response.Cookies.Add(cookie);
        }

        #endregion
    }
}

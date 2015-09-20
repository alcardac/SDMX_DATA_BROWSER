using ISTAT.WebClient.WidgetComplements.Model.Log;
using ISTAT.WebClient.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ISTAT.WebClient.WidgetComplements.Model.Properties;
using ISTAT.WebClient.WidgetComplements.Model;

namespace ISTAT.WebClient
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        #region Constants and Fields

        /// <summary>
        /// log
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        /// <summary>
        /// Lock object to avoid re-initialization of the <see cref="initialized"/>
        /// </summary>
        private static readonly object _mapLock = new object();

        /// <summary>
        /// A value indicationg whether static initialization has been completed.
        /// </summary>
        private static bool initialized;

        #endregion

        /// <summary>
        /// Executed when application is started
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The parameter is not used.
        /// </param>
        protected void Application_Start()
        {
            /*
            CultureInfo culture = LocaleResolver.GetCookie(HttpContext.Current);
            CultureInfo c = ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources.Messages.SetLocale((string)culture.TwoLetterISOLanguageName);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            */
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var path = Server.MapPath("~/log4net.xml");
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));
            Logger.Info("Application started");

            ISTAT.WebClient.CacheManager.CacheManager.RunManager();


            if (initialized)
            {
                return;
            }

            lock (_mapLock)
            {
                if (initialized)
                {
                    return;
                }

                LogError.Instance.LogErrorEvent -= HandleLogErrorEvent;
                LogError.Instance.LogErrorEvent += HandleLogErrorEvent;
                initialized = true;

            }

        }

        /// <summary>
        /// Log application restarts
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The parameter is not used.
        /// </param>
        protected void Application_End(object sender, EventArgs e)
        {

            Logger.Info("Application end");
            ISTAT.WebClient.CacheManager.CacheManager.StopManager();

            var runtime =
                (HttpRuntime)
                typeof(HttpRuntime).InvokeMember(
                    "_theRuntime",
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                    null,
                    null,
                    null,
                    CultureInfo.InvariantCulture);

            if (runtime == null)
            {
                return;
            }

            var shutDownMessage =
                (string)
                runtime.GetType().InvokeMember(
                    "_shutDownMessage",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                    null,
                    runtime,
                    null,
                    CultureInfo.InvariantCulture);

            var shutDownStack =
                (string)
                runtime.GetType().InvokeMember(
                    "_shutDownStack",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                    null,
                    runtime,
                    null,
                    CultureInfo.InvariantCulture);

            Logger.WarnFormat(
                    CultureInfo.InvariantCulture,
                    "\r\n\r\n_shutDownMessage={0}\r\n\r\n_shutDownStack={1}",
                    shutDownMessage,
                    shutDownStack);
        }

        /// <summary>
        /// Handle application error
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The parameter is not used.
        /// </param>
        protected void Application_Error(object sender, EventArgs e)
        {
            string message;
            Exception ex = this.Context.Server.GetLastError();
            if (ex != null)
            {
                message = ex.ToString();
                this.Context.Server.ClearError();
            }
            else if (this.Context.Error != null)
            {
                message = this.Context.Error.ToString();
                this.Context.ClearError();
            }
            else
            {
                message = Resources.ErrorApplicationError;
            }

            Logger.ErrorFormat(
                    CultureInfo.InvariantCulture, Resources.ErrorApplicationErrorFormat2, this.Request.RawUrl, message);
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo culture = LocaleResolver.GetCookie(HttpContext.Current);
            CultureInfo c = ISTAT.WebClient.WidgetComplements.Model.App_GlobalResources.Messages.SetLocale((string)culture.TwoLetterISOLanguageName);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        /// <summary>
        /// Initialize the session, get a cookie for language
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The parameter is not used.
        /// </param>
        protected void Session_Start(object sender, EventArgs e)
        {
            Logger.Info("starting session ...");
            try
            {
                RemoveJsTreeCookie(this.Context);
                CultureInfo culture = LocaleResolver.GetCookie(this.Context);
                Thread.CurrentThread.CurrentUICulture = culture;
                Logger.Info("starting session success");
            }
            catch (HttpException ex)
            {
                Logger.Error("starting session failled");
                Logger.Error(ex.Message, ex);
            }
            catch (SecurityException ex)
            {
                Logger.Error("starting session failled");
                Logger.Error(ex.Message, ex);
            }
            catch (IOException ex)
            {
                Logger.Error("starting session failled");
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Reset the session
        /// </summary>
        /// <param name="sender">
        /// The sender
        /// </param>
        /// <param name="e">
        /// The parameter is not used
        /// </param>
        protected void Session_End(object sender, EventArgs e)
        {
            Logger.Info("ending session ...");
            try
            {
                foreach (string SessionName in Session.Keys)
                {
                    if (Session[SessionName] != null && Session[SessionName] is SessionImplObject)
                        ((SessionImplObject)Session[SessionName]).ClearCache();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove the JSTree cookie
        /// </summary>
        /// <param name="context">
        /// The current <see cref="HttpContext"/>
        /// </param>
        private static void RemoveJsTreeCookie(HttpContext context)
        {
            var jstreeCookies = new[] { "jstree_open", "jstree_load" };

            for (int i = 0; i < jstreeCookies.Length; i++)
            {
                HttpCookie cookie = context.Request.Cookies[jstreeCookies[i]];
                if (cookie != null)
                {
                    cookie.Value = null;
                    cookie.Path = context.Request.ApplicationPath + "/";
                    cookie.Expires = DateTime.Now.AddDays(-1d);
                    context.Response.Cookies.Add(cookie);
                }
            }
        }
        
        /// <summary>
        /// Handle log error event
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The provided <see cref="LogErrorEventArgs"/>
        /// </param>
        private static void HandleLogErrorEvent(object sender, LogErrorEventArgs e)
        {
            Logger.Error(e.Message);
        }

    }
}
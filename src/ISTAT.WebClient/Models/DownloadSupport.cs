using ISTAT.WebClient.Complements.Model;
using ISTAT.WebClient.Complements.Model.App_GlobalResources;
using ISTAT.WebClient.Engine.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ISTAT.WebClient.Models
{
    public class DownloadSupport : IDownloadSupport
    {


        /// <summary>
        /// The session expired.
        /// </summary>
        public void SessionExpired()
        {
            HttpContext context = HttpContext.Current;
            context.Response.StatusCode = 599;
            context.Response.ContentType = "text/html";
            context.Response.WriteFile(context.Server.MapPath("~/errors/session-expired.htm"));
            context.Response.Write(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.error_invalid_session,
                    HttpContext.Current.Request.ApplicationPath));
            context.Response.Write("</body></html>");

            // context.Response.Redirect( "~/errors/session-expired.htm",true);
            return;
        }


        public void SetContentType(string MimeType)
        {
            HttpContext.Current.Response.ContentType = MimeType;
        }


        public void SetContentEncoding(Encoding encoding)
        {
            HttpContext.Current.Response.ContentEncoding = encoding;
        }


        public void SetHeader(string contentDisposition)
        {
            HttpContext.Current.Response.AddHeader(Constants.ContentDispositionHttpHeader, contentDisposition);
        }


        public Stream GetResponseOutputStream()
        {
            return HttpContext.Current.Response.OutputStream;
        }

        public TextWriter GetResponseOutput()
        {
            return HttpContext.Current.Response.Output;
        }

        public void ResponseOutputFlush()
        {
            HttpContext.Current.Response.Output.Flush();
        }



        public string GetFormRequest(string NameObj)
        {
            return HttpContext.Current.Request.Form[NameObj];
        }

        public string GetParams(string NameParams)
        {
            return HttpContext.Current.Request.Params[NameParams];
        }
    }
}

using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.Models
{
    public class ControllerSupport
    {

        public struct StringResult {
            public string Msg { get; set; }
        }

        #region Constants and Fields

        /// <summary>
        /// JSON containing the error occurred flag
        /// </summary>
        public const string ErrorOccured = "{\"error\" : true }";

        /// <summary>
        /// Error no dataflow
        /// </summary>
        public const string ErrorNoDataflow = "{\"error\":true, \"dataflowError\":true, \"message\":{0} }";

        /// <summary>
        /// JSON containing the session expired flag
        /// </summary>
        public const string InvalidSession = "{\"sessionExpired\" : true }";

        /// <summary>
        /// JSON containing the no data was found flag
        /// </summary>
        public const string NoData = "{\"nodata\" : true }";

      

        /// <summary>
        /// Empty string serialized to JSON
        /// </summary>
        public static readonly string _emptyJSON = new JavaScriptSerializer().Serialize(string.Empty);

        public readonly ILog Logger = LogManager.GetLogger(typeof(ControllerSupport));

        #endregion

        internal ActionResult ReturnForJQuery(object obj)
        {
            string deserializedObject = string.Empty;

            if (obj == null) { 
                deserializedObject = _emptyJSON;
            }
            else if (obj is string)
            {
                deserializedObject = obj.ToString();
            }
            else deserializedObject = new JavaScriptSerializer().Serialize(obj);
            
            JsonResult jr = new JsonResult();
            jr.Data = deserializedObject;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }


        internal dynamic GetPostData(System.Web.HttpRequestBase Req)
        {
            dynamic myObject;
            using (Stream receiveStream = Req.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Req.ContentEncoding))
                {
                    myObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(readStream.ReadToEnd());
                }
            }
            return myObject;
        }

        internal T GetPostData<T>(System.Web.HttpRequestBase Req)
        {
            using (Stream receiveStream = Req.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Req.ContentEncoding))
                {
                    string eee = readStream.ReadToEnd();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(eee);
                }
            }
        }
    }
}

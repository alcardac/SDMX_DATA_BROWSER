using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ISTAT.WebClient.Models
{
    public class StreamResponseAction : ActionResult
    {
        public string RetResp { get; set; }

        public StreamResponseAction()
        {
            this.RetResp = null;
        }
        public StreamResponseAction(string Error)
        {
            this.RetResp = Error;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!string.IsNullOrEmpty(this.RetResp))
            {
                context.HttpContext.Response.Write(this.RetResp);
            }
        }
    }
}

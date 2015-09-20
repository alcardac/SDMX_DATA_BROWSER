using ISTAT.SingleSignON.LoginWidget.ServiceActivaction;
using ISTAT.SingleSignON.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace ISTAT.SingleSignON.LoginWidget
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add(new ServiceRoute("service", new RestHostFactory(typeof(ISingleSignONService)), typeof(SingleSignONService)));
        }

        
    }
}
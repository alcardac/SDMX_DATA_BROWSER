using ISTAT.WebClient.WidgetComplements.Model;
using ISTAT.WebClient.WidgetComplements.Model.CallWS;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetEngine.Model
{
    public class WebServiceSelector
    {
        public static IGetSDMX GetSdmxImplementation(EndpointSettings endpoint)
        {
            IGetSDMX obj = null;
            try
            {
                switch (endpoint._TypeEndpoint)
                {
                    case ISTAT.WebClient.WidgetComplements.Model.Enum.EndpointType.V20:
                        obj = new GetSDMX_WSV20(endpoint);
                        break;
                    case ISTAT.WebClient.WidgetComplements.Model.Enum.EndpointType.V21:
                        obj = new GetSDMX_WSV21(endpoint);
                        break;
                    case ISTAT.WebClient.WidgetComplements.Model.Enum.EndpointType.REST:
                        obj = new GetSDMX_WSRest(endpoint);
                        break;
                }
            }
            catch { }
                
            return obj;
            
        }
    }
}

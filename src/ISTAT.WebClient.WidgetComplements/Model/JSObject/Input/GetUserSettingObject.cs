using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{

    public class GetUserSettingObject
    {
        public string userCode { get; set; }
        public int userRole { get; set; }
        public UserSettingResponseObject setting { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class TextLocalised
    {
        public string title { get; set; }
        public string content { get; set; }
        public string locale { get; set; }
        
    }

    public class WidgetObject
    {
        public int id { get; set; }
        public string cssClass { get; set; }
        public int rowID { get; set; }
        public int cell { get; set; }
        public int order { get; set; }
        public List<TextLocalised> text { get; set; }
        public string type { get; set; }
        public string chartype { get; set; }
        public bool v { get; set; }
        public bool vt { get; set; }
        public bool vc { get; set; }
        public string endPoint { get; set; }
        public string endPointType { get; set; }
        public string endPointV20 { get; set; }   
        public string endPointSource { get; set; }
        public string endPointDecimalSeparator { get; set; }
        public string dataflow_id { get; set; }
        public string dataflow_agency_id { get; set; }
        public string dataflow_version { get; set; }
        public string criteria { get; set; }
        public string layout { get; set; }
        public string labels { get; set; }
    }

    public class DashboardRow
    {
        public int id { get; set; }
        public bool splitted { get; set; }
        public int order { get; set; }
        public List<WidgetObject> widgets { get; set; }

    }

    public class GetDashboardObject
    {
        public int id { get; set; }
        public string usercode { get; set; }
        public int numrow { get; set; }
        public bool active { get; set; }
        public string date { get; set; }
        public List<TextLocalised> text { get; set; }
        public List<DashboardRow> rows { get; set; }

    }

    public class DashboardHeader {

        public string title { get; set; }

    }

    public class SavedWidget
    {
        public int savedWidgetID { get; set; }
        public int widgetID { get; set; }
        public string widgetData { get; set; }
        public string locale { get; set; }
        public DateTime dtUpdate { get; set; }

    }
}

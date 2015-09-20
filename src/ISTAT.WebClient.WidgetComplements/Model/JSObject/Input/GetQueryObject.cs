using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class GetQueryObject
    {
        public string UserCode { get; set; }
        public string QueryId { get; set; }

        public QueryObject Query { get; set; }
    }
    public class GetTemplateObject
    {
        public string UserCode { get; set; }
        public string TemplateId { get; set; }

        public TemplateObject Template { get; set; }
    }
}

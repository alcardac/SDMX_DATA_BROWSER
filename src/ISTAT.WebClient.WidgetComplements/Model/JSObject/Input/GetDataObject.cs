﻿using ISTAT.WebClient.WidgetComplements.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class GetDataObject
    {
        public int WidgetId { get; set; }
        public EndpointSettings Configuration { get; set; }
        public MaintenableObj Dataflow { get; set; }
        public Dictionary<string, List<string>> Criteria { get; set; }
        public LayoutObj Layout { get; set; }
    }
}

using Newtonsoft.Json;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class LayoutObj: IComparable
    {
        public LayoutObj()
        {
            axis_x = new List<string>();
            axis_y = new List<string>();
            axis_z = new List<string>();
        }

        public List<string> axis_x { get; set; }
        public List<string> axis_y { get; set; }
        public List<string> axis_z { get; set; }

        public bool block_axis_x { get; set; }
        public bool block_axis_y { get; set; }
        public bool block_axis_z { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is LayoutObj))
                return -1;
            LayoutObj otherLay = (LayoutObj)obj;
            if (!this.axis_x.SequenceEqual(otherLay.axis_x)) return 1;
            if (!this.axis_y.SequenceEqual(otherLay.axis_y)) return 1;
            if (!this.axis_z.SequenceEqual(otherLay.axis_z)) return 1;
            return 0;
        }
    }
}

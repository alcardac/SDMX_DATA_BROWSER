using ISTAT.WebClient.WidgetComplements.Model.DataRender;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using System.Globalization;

namespace ISTAT.WebClient.WidgetEngine.Model.DataRender
{
    public class DataRender
    {
        private IDataSetStore store { get; set; }
        private List<DataCriteria> Criterias { get; set; }
        private LayoutObj layObj { get; set; }
        private ISdmxObjects Structure { get; set; }
        private  ComponentCodeDescriptionDictionary codemap { get; set; }

        bool _useAttr;

        private CultureInfo cFrom;
        private CultureInfo cTo;

        public DataRender(IDataSetStore store, List<DataCriteria> Criterias, LayoutObj layObj, ISdmxObjects structure, ComponentCodeDescriptionDictionary codemap, bool useAttr, CultureInfo cFrom, CultureInfo cTo)
        {
            this.store = store;
            this.Criterias = Criterias;
            this.layObj = layObj;
            this.Structure = structure;
            this.codemap = codemap;
            this._useAttr=useAttr;

            this.cFrom=cFrom;
            this.cTo=cTo;
        }

        internal void render(TextWriter writer)
        {
            IDataSetModel l = new DataSetModelStore(Structure, store);

            l.Initialize();
            l.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y);

            HtmlRenderer htmlRenderer = new HtmlRenderer(this.codemap, true, _useAttr, cFrom, cTo);

            htmlRenderer.Render(l, writer);
                                      
            //new HtmlRenderer(query.GetComponentCodeDescriptionMap(), true).Render(
            //   query.DatasetModel,
            //   context.Response.Output);
        }
    }
}

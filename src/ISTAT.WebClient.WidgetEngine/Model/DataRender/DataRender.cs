using ISTAT.WebClient.WidgetComplements.Model.DataRender;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetEngine.Model.DataReader;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;
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

        internal void render(TextWriter writer,SessionQuery query)
        {

            IDataSetModel l = new DataSetModelStore(Structure, store);

            /*
            if (query._dataSetModel != null)
            {
                l = query._dataSetModel;
                l.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y);
                query._store.SetCriteria(this.Criterias);
                query.DatasetModel = l;
            }
            else
            {
                l.Initialize(this.Criterias);
                l.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y);
                query.DatasetModel = l;
            }
            */

            if (query.DatasetModel != null)
            {

                //query.DatasetModel.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y);
                query.DatasetModel.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y, this.Criterias);
                query._store.SetCriteria(this.Criterias);
            }
            else
            {
                query.DatasetModel = new DataSetModelStore(Structure, store);
                query.DatasetModel.Initialize(this.Criterias);
                //query.DatasetModel.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y);
                query.DatasetModel.UpdateAxis(layObj.axis_z, layObj.axis_x, layObj.axis_y, this.Criterias);
            }

            HtmlRenderer htmlRenderer = new HtmlRenderer(this.codemap, true, _useAttr, cFrom, cTo);


              //  { if (!DataStream.store.ExistsColumn(axisX)) DataStream.layObj.axis_x.Remove(axisX); });
            //this.Criterias.ForEach(c => l.UpdateSliceKeyValue(c.component, c.values.FirstOrDefault()));
            /*
            for(int i=0; i<layObj.axis_z.Count; i++) {
                string criterio=layObj.axis_z[i];
                this.Criterias.ForEach(c => {if (c.component==criterio) {l.UpdateSliceKeyValue(c.component, c.values.FirstOrDefault());}});
            }
            */
            //htmlRenderer.Render(l, writer);
            htmlRenderer.Render(query._dataSetModel, writer);
                                      
            //new HtmlRenderer(query.GetComponentCodeDescriptionMap(), true).Render(
            //   query.DatasetModel,
            //   context.Response.Output);
        }
    }
}

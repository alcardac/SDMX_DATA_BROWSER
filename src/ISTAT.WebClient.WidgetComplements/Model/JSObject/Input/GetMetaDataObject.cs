using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject.Input
{
    public class GetMetaDataObject
    {
        public EndpointSettings Configuration { get; set; }
        public MaintenableObj Artefact { get; set; }
        public string ArtefactType{ get; set; }
        
    }
}

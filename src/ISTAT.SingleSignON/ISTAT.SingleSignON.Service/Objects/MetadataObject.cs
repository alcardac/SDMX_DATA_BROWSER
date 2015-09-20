using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ISTAT.SingleSignON.Service
{
    [DataContract]
    public class MetadataObject
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

   
}

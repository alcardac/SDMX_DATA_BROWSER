using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ISTAT.SingleSignON.Service
{
    [DataContract]
    public class PasswordObj
    {
        [DataMember]
        public string UserCode { get; set; }
        [DataMember]
        public string OldPassword { get; set; }
        [DataMember]
        public string NewPassword { get; set; }
       
    }
}

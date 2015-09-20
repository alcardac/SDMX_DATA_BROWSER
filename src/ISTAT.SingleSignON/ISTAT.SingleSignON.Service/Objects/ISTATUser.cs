using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ISTAT.SingleSignON.Service
{
    [DataContract]
    public class ISTATUser
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string UserCode { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string Sex { get; set; }
        [DataMember]
        public string Age { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Study { get; set; }
        [DataMember]
        public string Position { get; set; }
        [DataMember]
        public string Agency { get; set; }
        
        [DataMember]
        public bool IsSA { get; set; }

        [DataMember]
        public string Lang { get; set; }
        [DataMember]
        public List<string> Themes { get; set; }


    }
}

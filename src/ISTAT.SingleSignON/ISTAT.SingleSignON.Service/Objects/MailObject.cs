using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ISTAT.SingleSignON.Service
{
    [DataContract]
    public class MailObject
    {
        [DataMember]
        public string EmailUser { get; set; }

        [DataMember]
        public string MailSender { get; set; }

        [DataMember]
        public string MailSubject { get; set; }

        [DataMember]
        public string MailTemplate { get; set; }

        [DataMember]
        public string MailSMTPServer { get; set; }

    }
}

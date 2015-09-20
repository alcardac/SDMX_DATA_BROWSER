using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject
{
    public class ISTATUser
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string UserCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Country { get; set; }
        public string Study { get; set; }
        public string Position { get; set; }
        public string Agency { get; set; }

        public bool IsSA { get; set; }

        public string Lang { get; set; }
        public List<string> Themes { get; set; }
    }
}

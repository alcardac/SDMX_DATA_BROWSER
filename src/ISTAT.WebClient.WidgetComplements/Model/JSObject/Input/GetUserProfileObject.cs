using ISTAT.WebClient.WidgetComplements.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject.Input
{
    public class GetUserProfileObject
    {
        public string UserCode { get; set; }
        public bool IsSuperAdmin { get; set; }

        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }

        public UserRoleObject UserRole { get; set; }
    }

    public class UserRoleObject
    {
        public int RoleId { get; set; }
        public string Role { get; set; }
    }

}

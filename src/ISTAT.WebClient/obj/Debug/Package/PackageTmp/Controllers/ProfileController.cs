using ISTAT.WebClient.Models;
using ISTAT.WebClient.WidgetComplements.Model.Enum;
using ISTAT.WebClient.WidgetComplements.Model.JSObject.Input;
using ISTAT.WebClient.WidgetEngine.WidgetBuild;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.Controllers
{
    public class ProfileController : Controller
    {
        private ControllerSupport CS = new ControllerSupport();
        public const string ProfileSession = "ProfileSession";

        public ActionResult Login()
        {
            try
            {
                GetUserProfileObject PostDataArrived = CS.GetPostData<GetUserProfileObject>(this.Request);

                ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
                if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                    throw new Exception("ConnectionString not set");

                if (PostDataArrived.IsSuperAdmin)
                {
                    UserRolesEnum ruolo = UserRolesEnum.Administrator;
                    PostDataArrived.UserRole = new UserRoleObject() { RoleId = (int)ruolo, Role = ruolo.ToString() };
                }
                else
                {
                    ProfileWidget pw = new ProfileWidget(connectionStringSetting.ConnectionString);
                    PostDataArrived.UserRole = pw.GetRole(PostDataArrived);
                }

                Session[ProfileSession] = PostDataArrived;
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }
        public ActionResult Logout()
        {
            Session[ProfileSession] = null;
            return CS.ReturnForJQuery("{\"logout\" : true }");
        }
        public ActionResult IsLogin()
        {
            try
            {
                GetUserProfileObject PostDataArrived = CS.GetPostData<GetUserProfileObject>(this.Request);
                Session[ProfileSession] = PostDataArrived;
                return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(PostDataArrived));
            }
            catch (Exception ex)
            {
                return CS.ReturnForJQuery(ex.Message);
            }
        }
         
        public ActionResult GetUserList()
        {
            string SingleSignOnConf;
            using (Stream receiveStream = this.Request.InputStream)
                using (StreamReader readStream = new StreamReader(receiveStream, this.Request.ContentEncoding))
                {
                    SingleSignOnConf = readStream.ReadToEnd();
                }

            if (Session[ProfileSession] == null)
                throw new Exception("No logged user");
            GetUserProfileObject LoggedUser = (GetUserProfileObject)Session[ProfileSession];
            if (LoggedUser.UserRole != null && (UserRolesEnum)LoggedUser.UserRole.RoleId != WidgetComplements.Model.Enum.UserRolesEnum.Administrator)
                throw new Exception("No Administration user");

            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
            if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                throw new Exception("ConnectionString not set");

            ProfileWidget pw = new ProfileWidget(connectionStringSetting.ConnectionString);

            //Prendo tutti gli utenti su SingleSignON
            //Prendo tutti i Ruoli dal localDB
            var JsonRet = new { UserList = pw.GetUserList(SingleSignOnConf), Roles = pw.GetRoles() };

            return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(JsonRet));
        }

        public ActionResult ModUserRole()
        {
            GetUserProfileObject PostDataArrived = CS.GetPostData<GetUserProfileObject>(this.Request);
            if (Session[ProfileSession] == null)
                throw new Exception("No logged user");
            GetUserProfileObject LoggedUser = (GetUserProfileObject)Session[ProfileSession];
            if (LoggedUser.UserRole != null && (UserRolesEnum)LoggedUser.UserRole.RoleId != WidgetComplements.Model.Enum.UserRolesEnum.Administrator)
                throw new Exception("No Administration user");

            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["ISTATWebClientConnection"];
            if (connectionStringSetting == null || string.IsNullOrEmpty(connectionStringSetting.ConnectionString))
                throw new Exception("ConnectionString not set");

            ProfileWidget pw = new ProfileWidget(connectionStringSetting.ConnectionString);
            PostDataArrived = pw.ChangeRole(PostDataArrived);

            return CS.ReturnForJQuery(new JavaScriptSerializer().Serialize(PostDataArrived));
        }


    }
}

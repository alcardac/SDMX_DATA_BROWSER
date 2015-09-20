using ISTAT.WebClient.WidgetComplements.Model.Enum;
using ISTAT.WebClient.WidgetComplements.Model.JSObject;
using ISTAT.WebClient.WidgetComplements.Model.JSObject.Input;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace ISTAT.WebClient.WidgetEngine.WidgetBuild
{
    public class ProfileWidget
    {
        public SqlConnection Sqlconn { get; set; }
        private const string ErrorOccured = "{\"error\" : true }";
        private const string ErrorOccuredMess = "{\"error\" : true, \"message\" : {0} }";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProfileWidget));

        public ProfileWidget(string connectionString)
        {
            Sqlconn = new SqlConnection(connectionString);
        }

        public UserRoleObject GetRole(GetUserProfileObject PostDataArrived)
        {
            try
            {
                if (PostDataArrived == null || string.IsNullOrEmpty(PostDataArrived.UserCode))
                    throw new Exception("Input Error");

                string sqlquery = string.Format("Select * from UserRoles where UserCode='{0}'", PostDataArrived.UserCode.Replace("'", "''"));

                Sqlconn.Open();
                try
                {
                    DataTable dtres = new DataTable();
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            da.Fill(dtres);
                        }
                    }
                    UserRolesEnum ruolo = UserRolesEnum.User;
                    if (dtres != null && dtres.Rows.Count > 0)
                    {
                        int RoleCode = Convert.ToInt32(dtres.Rows[0]["RoleId"].ToString());
                        ruolo = (UserRolesEnum)RoleCode;
                    }

                    return new UserRoleObject() { RoleId = (int)ruolo, Role = ruolo.ToString() };

                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
        }


        public List<GetUserProfileObject> GetUserList(string SingleSignOnConf)
        {
            List<GetUserProfileObject> utentiSSON = GetSingleSignONUsers(SingleSignOnConf);
            try
            {
                string sqlquery = string.Format("Select * from UserRoles");
                Sqlconn.Open();
                try
                {
                    DataTable dtres = new DataTable();
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            da.Fill(dtres);
                        }
                    }

                    foreach (DataRow userrow in dtres.Rows)
                    {
                        GetUserProfileObject user = utentiSSON.Find(u => u.UserCode == userrow["UserCode"].ToString());
                        if (user == null)
                        {
                            DeleteUserforSynk(userrow["UserCode"].ToString());
                            continue;
                        }
                        UserRolesEnum ruolo = UserRolesEnum.User;
                        if (user.IsSuperAdmin)
                            ruolo = UserRolesEnum.Administrator;
                        else
                            ruolo = (UserRolesEnum)Convert.ToInt32(userrow["RoleId"].ToString());

                        user.UserRole = new UserRoleObject() { RoleId = (int)ruolo, Role = ruolo.ToString() };
                    }
                    utentiSSON.FindAll(u => u.UserRole == null).ForEach(u =>
                        {
                            UserRolesEnum ruolo = UserRolesEnum.User;
                            if (u.IsSuperAdmin)
                                ruolo = UserRolesEnum.Administrator;
                            u.UserRole = new UserRoleObject() { RoleId = (int)ruolo, Role = ruolo.ToString() };
                        });

                    return utentiSSON;

                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
        }

        private List<GetUserProfileObject> GetSingleSignONUsers(string SingleSignOnConf)
        {
            string endpoint = SingleSignOnConf.Clone().ToString();
            if (!endpoint.EndsWith("/"))
                endpoint += "/";
            endpoint += "service/GetUsers";

            List<GetUserProfileObject> SSONUser = new List<GetUserProfileObject>();
            WebRequest req = HttpWebRequest.Create(endpoint);
            req.Method = "GET";
            string Risposta = "";
            using (WebResponse response = req.GetResponse())
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(receiveStream))
                    {
                        Risposta = readStream.ReadToEnd();
                    }
                }
            }
            List<ISTATUser> jsonObj = new JavaScriptSerializer().Deserialize<List<ISTATUser>>(Risposta);

            if (jsonObj != null)
                foreach (var item in jsonObj)
                {
                    SSONUser.Add(new GetUserProfileObject()
                    {
                        UserCode = item.UserCode,
                        Nome = item.Name,
                        Cognome = item.Surname,
                        Email = item.Email,
                        IsSuperAdmin = item.IsSA
                    });
                }
            return SSONUser;
        }

        public List<UserRoleObject> GetRoles()
        {
            List<UserRoleObject> listRole = new List<UserRoleObject>();
            foreach (UserRolesEnum role in Enum.GetValues(typeof(UserRolesEnum)))
                listRole.Add(new UserRoleObject() { RoleId = (int)role, Role = role.ToString() });
            return listRole;
        }

        private void DeleteUserforSynk(string UserCode)
        {
            try
            {
                //Solo a connessione Gia aperta
                string sqlquery = string.Format("Delete from UserRoles where UserCode='{0}'", UserCode.Replace("'", "''"));
                using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                {
                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
        }

        public GetUserProfileObject ChangeRole(GetUserProfileObject PostDataArrived)
        {
            try
            {
                if (PostDataArrived == null || string.IsNullOrEmpty(PostDataArrived.UserCode))
                    throw new Exception("Input Error");


                string sqlquery = string.Format("select count(*) from UserRoles where UserCode='{0}'", PostDataArrived.UserCode.Replace("'", "''"), PostDataArrived.UserRole.RoleId);
                //string 

                Sqlconn.Open();
                try
                {

                    DataTable dtres = new DataTable();
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        int conta = Convert.ToInt32(comm.ExecuteScalar());
                        if (conta > 0)
                            sqlquery = string.Format("Update UserRoles set RoleId={1} where UserCode='{0}'", PostDataArrived.UserCode.Replace("'", "''"), PostDataArrived.UserRole.RoleId);
                        else
                            sqlquery = string.Format("INSERT INTO UserRoles (UserCode, RoleId) VALUES ('{0}',{1})", PostDataArrived.UserCode.Replace("'", "''"), PostDataArrived.UserRole.RoleId);
                    }
                    using (SqlCommand comm = new SqlCommand(sqlquery, Sqlconn))
                    {
                        int mod = comm.ExecuteNonQuery();
                        if (mod <= 0)
                            throw new Exception("User not modified");
                    }

                    return PostDataArrived;
                }
                catch (Exception) { throw; }
                finally
                {
                    Sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                throw new Exception(string.Format(ErrorOccuredMess, ex.Message));
            }
        }
    }
}

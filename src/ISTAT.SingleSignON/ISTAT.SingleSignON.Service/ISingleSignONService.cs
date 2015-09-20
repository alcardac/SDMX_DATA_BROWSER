using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace ISTAT.SingleSignON.Service
{
    [ServiceContract]
    public interface ISingleSignONService
    {
      
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Login", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ISTATUser Login(LoginObj login);
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "ChangePassword", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool ChangePassword(PasswordObj passobj);
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "ResetPassword", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool ResetPassword(MailObject passobj);

        
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetUsers", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<ISTATUser> GetUsers();
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetUser", ResponseFormat = WebMessageFormat.Json)]
        ISTATUser GetUser(string UserCode);
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "AddUser", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ISTATUser AddUser(ISTATUser user);
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "ModUser", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ISTATUser ModUser(ISTATUser user);
        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "DelUser", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        bool DelUser(ISTATUser user);

        [OperationContract]
        [WebInvoke(Method = "*", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetMetadata", ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, List<MetadataObject>> GetMetadata(string Lang);



    }
   
}

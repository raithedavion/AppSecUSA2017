using Models;
using Soap_Basic.Classes.Utilities;
using Soap_Basic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Soap_Basic
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RestService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RestService.svc or RestService.svc.cs at the Solution Explorer and start debugging.
    public class RestService : IRestUser
    {
        [WebInvoke(Method = "POST", UriTemplate = "/CreateUser", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public int CreateUser(User user)
        {
            return user.CreateUser();
        }

        [WebInvoke(Method = "POST", UriTemplate = "/UpdateUser", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public bool UpdateUser(User user)
        {
            WebOperationContext context = WebOperationContext.Current;
            if (HashAuthorization.IsAuthorized(context.IncomingRequest))
                return user.UpdateUser();
            else
            {
                OutgoingWebResponseContext response = context.OutgoingResponse;
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return false;
            }
        }

        [WebInvoke(Method = "GET", UriTemplate = "/Users/{id}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public User GetUser(string id)
        {
            int userID;
            if (int.TryParse(id, out userID))
            {
                return User.GetUser(userID);
            }
            else
                throw new Exception("Friendly Message");
        }

        [WebInvoke(Method = "GET", UriTemplate = "/Users", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public List<User> ListUsers()
        {
            return User.ListUsers();
        }
    }
}

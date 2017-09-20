using Models;
using Soap_Basic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;

namespace Soap_Basic
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSService.svc or WSService.svc.cs at the Solution Explorer and start debugging.
    public class WSService : IUser
    {
        [PrincipalPermission(SecurityAction.Demand, Name = "ServiceUser1")]
        public int CreateUser(User user)
        {
            return user.CreateUser();
        }

        [PrincipalPermission(SecurityAction.Demand, Name = "ServiceUser1")]
        public bool UpdateUser(User user)
        {
            return user.UpdateUser();
        }

        [PrincipalPermission(SecurityAction.Demand, Name = "ServiceUser1")]
        public User GetUser(int id)
        {
            return User.GetUser(id);
        }

        [PrincipalPermission(SecurityAction.Demand, Name = "ServiceUser1")]
        public List<User> ListUsers()
        {
            return User.ListUsers();
        }
    }
}

using Models;
using Soap_Basic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Soap_Basic
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BasicService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BasicService.svc or BasicService.svc.cs at the Solution Explorer and start debugging.
    public class BasicService : IUser
    {
        public int CreateUser(User user)
        {
            return user.CreateUser();
        }
        public bool UpdateUser(User user)
        {
            return user.UpdateUser();
        }
        public User GetUser(int id)
        {
            return User.GetUser(id);
        }

        public List<User> ListUsers()
        {
            return User.ListUsers();
        }
    }
}

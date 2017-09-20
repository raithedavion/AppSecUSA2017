using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Soap_Basic.Interfaces
{
    [ServiceContract]
    public interface IRestUser
    {
        [OperationContract]
        int CreateUser(User user);

        [OperationContract]
        bool UpdateUser(User user);

        [OperationContract]
        User GetUser(string id);

        [OperationContract]
        List<User> ListUsers();
    }
}

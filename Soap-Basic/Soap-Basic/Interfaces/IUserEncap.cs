using Models;
using Soap_Basic.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Soap_Basic.Interfaces
{
    [ServiceContract]
    public interface IUserEncap
    {
        [OperationContract]
        UserResponse CreateUser(User user);

        [OperationContract]
        UserResponse UpdateUser(User user);

        [OperationContract]
        UserResponse GetUser(int id);

        [OperationContract]
        UserResponse ListUsers();
    }
}

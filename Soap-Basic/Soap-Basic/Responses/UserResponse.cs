using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Soap_Basic.Responses
{
    [DataContract]
    public class UserResponse
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public bool WasSuccessful { get; set; }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public List<User> UserList { get; set; }
    }
}
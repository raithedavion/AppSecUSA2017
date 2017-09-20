using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Security.Hmac
{
    public class HmacIdentity : GenericIdentity
    {
        public HmacIdentity(string clientId) : base(clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; set; }
    }
}

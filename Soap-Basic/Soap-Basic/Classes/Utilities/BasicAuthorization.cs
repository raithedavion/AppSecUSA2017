using System;
using System.Collections.Generic;
using System.IO;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Xml.Linq;

namespace Soap_Basic.Classes.Utilities
{
    public class BasicAuthorization : ServiceAuthorizationManager
    {
        private const string Realm = "SampleApp";

        const string ErrorHtml = @"
<html>
<head>
    <title>Request Error - No Basic Authentication</title>
    <style type=""text/css"">
        body
        {
            font-family: Verdana;
            font-size: large;
        }
    </style>
</head>
<body>
    <h1>
        Request Error
    </h1>
    <p>
        This service requires basic authentication.
    </p>
</body>
</html>
";
        protected override bool CheckAccessCore(OperationContext operationContext)
        {

            string[] credentials = ExtractCredentials(operationContext.RequestContext.RequestMessage);
            if (credentials.Length > 0 && AuthenticateUser(credentials[0], credentials[1]))
            {
                InitializeSecurityContext(operationContext.RequestContext.RequestMessage, credentials[0]);
                return true;
            }

            var requestContext = operationContext.RequestContext;
            CreateErrorReply(ref requestContext);
            return false;
        }

        private string[] ExtractCredentials(Message requestMessage)
        {
            var request = (HttpRequestMessageProperty)requestMessage.Properties[HttpRequestMessageProperty.Name];

            string authHeader = request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUserPass = authHeader.Substring(6).Trim();

                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string userPass = encoding.GetString(Convert.FromBase64String(encodedUserPass));
                int separator = userPass.IndexOf(':');

                var credentials = new string[2];
                credentials[0] = userPass.Substring(0, separator);
                credentials[1] = userPass.Substring(separator + 1);

                return credentials;
            }

            return new string[] { };
        }

        private bool AuthenticateUser(string username, string password)
        {
            if (username == "user1" && password == "test")
            {
                return true;
            }

            return false;
        }

        private void InitializeSecurityContext(Message request, string username)
        {
            var principal = new GenericPrincipal(new GenericIdentity(username), new string[] { });

            var policies = new List<IAuthorizationPolicy> { new PrincipalAuthorizationPolicy(principal) };
            var securityContext = new ServiceSecurityContext(policies.AsReadOnly());

            if (request.Properties.Security != null)
            {
                request.Properties.Security.ServiceSecurityContext = securityContext;
            }
            else
            {
                request.Properties.Security = new SecurityMessageProperty { ServiceSecurityContext = securityContext };
            }
        }

        private void CreateErrorReply(ref RequestContext requestContext)
        {
            // The error message is padded so that IE shows the response by default
            using (var sr = new StringReader("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + ErrorHtml))
            {
                XElement response = XElement.Load(sr);
                using (Message reply = Message.CreateMessage(MessageVersion.None, null, response))
                {
                    var responseProp = new HttpResponseMessageProperty
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        StatusDescription = String.Format("Forbidden")
                    };
                    responseProp.Headers.Add("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", Realm));
                    responseProp.Headers[HttpResponseHeader.ContentType] = "text/html";
                    reply.Properties[HttpResponseMessageProperty.Name] = responseProp;
                    requestContext.Reply(reply);
                    // set the request context to null to terminate processing of this request
                    requestContext = null;
                }
            }
        }


        class PrincipalAuthorizationPolicy : AuthorizationPolicy
        {
            readonly string _id = Guid.NewGuid().ToString();
            readonly IPrincipal _user;

            public PrincipalAuthorizationPolicy(IPrincipal user)
            {
                _user = user;
            }

            public ClaimSet Issuer
            {
                get { return ClaimSet.System; }
            }

            public string Id
            {
                get { return _id; }
            }

            public bool Evaluate(EvaluationContext evaluationContext, ref object state)
            {
                evaluationContext.AddClaimSet(this, new DefaultClaimSet(Claim.CreateNameClaim(_user.Identity.Name)));
                evaluationContext.Properties["Identities"] = new List<IIdentity>(new[] { _user.Identity });
                evaluationContext.Properties["Principal"] = _user;
                return true;
            }
        }
    }
}
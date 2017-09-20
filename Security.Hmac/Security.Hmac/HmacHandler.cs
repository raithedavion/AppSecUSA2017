using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.Security.Cryptography;

namespace Security.Hmac
{
    public class HmacHandler : AuthenticationHandler<HmacOptions>
    {
        private static string AccessID;
        private static string Signature;
        private static string TimeStamp;
        private static Encoding Encoder { get { return Encoding.UTF8; } set { } }


        public HmacHandler(IOptionsMonitor<HmacOptions> options, ILoggerFactory logger, UrlEncoder encoder, IDataProtectionProvider dataProtection, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new HmacEvents Events
        {
            get { return (HmacEvents)base.Events; }
            set { base.Events = value; }
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new HmacEvents());

        /// <summary>
        /// Searches the 'Authorization' header for a 'appsec' signature. If the 'appsec' signature is found.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string signature = null;
            try
            {
                // Give application opportunity to find from a different location, adjust, or reject signature
                var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);

                //// event can set the signature
                await Events.MessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                {
                    return messageReceivedContext.Result;
                }

                // If application retrieved signature from somewhere else, use that.
                signature = messageReceivedContext.Signature;

                if (string.IsNullOrEmpty(signature))
                {
                    string authorization = Request.Headers["Authorization"];

                    // If no authorization header found, nothing to process further
                    if (string.IsNullOrEmpty(authorization))
                    {
                        return AuthenticateResult.NoResult();
                    }

                    if (authorization.StartsWith("appsec ", StringComparison.OrdinalIgnoreCase))
                    {
                        string fullAuth = authorization.Substring("appsec ".Length).Trim();
                        string[] Auth = fullAuth.Replace("appsec ", "").Split(':');
                        AccessID = Auth[0];
                        signature = Auth[1];
                        if(string.IsNullOrEmpty(signature))
                            return AuthenticateResult.NoResult();
                        Options.PublicKey = AccessID;
                        Signature = signature;

                    }

                    // If no token found, no further work possible
                    if (string.IsNullOrEmpty(signature))
                    {
                        return AuthenticateResult.NoResult();
                    }
                }

                List<Exception> validationFailures = null;
                if (Authorize(Request, AccessID, Signature))
                {
                    var signatureValidatedContext = new SignatureValidatedContext(Context, Scheme, Options);
                    HmacIdentity identityUser = null;
                    if (identityUser == null)
                    {
                        identityUser = new HmacIdentity(AccessID);
                    }
                    var principal = new ClaimsPrincipal(identityUser);
                    var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), HmacDefaults.AuthenticationScheme);
                    return AuthenticateResult.Success(ticket);
                }

                if (validationFailures != null)
                {
                    var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                    {
                        Exception = (validationFailures.Count == 1) ? validationFailures[0] : new AggregateException(validationFailures)
                    };

                    await Events.AuthenticationFailed(authenticationFailedContext);
                    if (authenticationFailedContext.Result != null)
                    {
                        return authenticationFailedContext.Result;
                    }
                    System.IO.File.WriteAllLines(@"Error1.txt", new string[] { "um.." });
                    return AuthenticateResult.Fail(authenticationFailedContext.Exception);
                }
                System.IO.File.WriteAllLines(@"Failed.txt", new string[] { "I don't know." });
                return AuthenticateResult.Fail("No SignatureValidator available for signature: " + signature ?? "[null]");
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllLines(@"Error.txt", new string[] { ex.Message });
                var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                throw;
            }
        }

        private bool Authorize(HttpRequest context, string publicKey, string signature)
        {
            AccessID = publicKey;
            Signature = signature;
            SetTimeStamp(context.Headers["Date"]);
            return ValidateWithQuery(WebUtility.HtmlDecode(Signature), AccessID, ConstructStringToSign(context));
        }

        private void SetTimeStamp(string header)
        {
            if (header != null && header != string.Empty)
            {
                TimeStamp = header;
                CheckTimeStamp();
            }
        }

        private void CheckTimeStamp()
        {
            DateTime CallDate = DateTime.Parse(TimeStamp);
            DateTime Current = DateTime.Now;
            if (CallDate < Current.AddMinutes(-15) || CallDate > Current.AddMinutes(15))
                throw new Exception("Expired request");
        }

        private string GetQueryValue(string key, HttpRequest woc)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(woc.QueryString.Value);
            if (query.Count != 0)
            {
                return System.Net.WebUtility.HtmlDecode(query[key]);
            }
            return null;
        }

        private static string GetQueryString(HttpRequest woc)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(woc.QueryString.Value);
            if (query.Count != 0)
            {
                string value = string.Empty;
                string QueryName;
                var enumQ = query.GetEnumerator();
                while (enumQ.MoveNext())
                {
                    QueryName = enumQ.Current.ToString();
                    value += query[QueryName];
                }
                return value;
            }
            return null;
        }

        private string ConstructStringToSign(HttpRequest context)
        {
            string signature = string.Empty;
            signature += context.Method + "\n";
            signature += context.Headers[HttpRequestHeader.ContentMd5.ToString()] + "\n";
            signature += context.Headers[HttpRequestHeader.ContentType.ToString()] + "\n";
            signature += context.Headers["Date"] + "\n";
            signature += GetQueryString(context);
            return signature;
        }

        public bool ValidateWithQuery(string hash, string publicKey, string query)
        {
            try
            {
                //Lookup private key here.
                string Key = PrivateKeySelect(publicKey);
                string Combined = query;
                string LocalHash = Hash(Combined, Key);
                return hash == LocalHash;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string PrivateKeySelect(string publicKey)
        {
            try
            {
                return Options.PrivateKey;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Hash(string plainText, string privateKey)
        {
            int cipherStrength = Convert.ToInt32(Options.CipherStrength);
            try
            {
                byte[] KeyBytes = Encoder.GetBytes(privateKey);
                HMAC Cipher = null;
                if (cipherStrength == 1)
                    Cipher = new HMACSHA1(KeyBytes);
                else if (cipherStrength == 256)
                    Cipher = new HMACSHA256(KeyBytes);
                else if (cipherStrength == 384)
                    Cipher = new HMACSHA384(KeyBytes);
                else if (cipherStrength == 512)
                    Cipher = new HMACSHA512(KeyBytes);
                else
                    throw new Exception("Enter a valid cipher strength.");
                byte[] PlainBytes = Encoder.GetBytes(plainText);
                byte[] HashedBytes = Cipher.ComputeHash(PlainBytes);
                return Convert.ToBase64String(HashedBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

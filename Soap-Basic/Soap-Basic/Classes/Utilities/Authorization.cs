using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace Soap_Basic.Classes.Utilities
{
    public class HashAuthorization
    {
        private static string AccessID;
        private static string Signature;
        private static string TimeStamp;

        public static bool IsAuthorized(IncomingWebRequestContext context)
        {
            SetAuthorizationFromHeader(context);
            SetDateFromheader(context.Headers["Date"]);
            return Hmac.ValidateWithQuery(WebUtility.HtmlDecode(Signature), AccessID, ConstructStringToSign(context));
        }

        private static void SetAuthorizationFromHeader(IncomingWebRequestContext context)
        {
            if (context.Headers["Authorization"] != null && context.Headers["Authorization"] != string.Empty)
            {
                string[] Auth = context.Headers["Authorization"].Split(':');
                AccessID = Auth[0];
                Signature = Auth[1];
            }
            else
            {
                AccessID = GetQueryValue("accessID", context);
                Signature = GetQueryValue("signature", context);
                if (AccessID == null || Signature == null)
                    throw new WebFaultException<string>("Authentication data not included in request.", HttpStatusCode.Unauthorized);
            }
        }

        private static void SetDateFromheader(string header)
        {
            if (header != null && header != string.Empty)
            {
                TimeStamp = header;
                CheckTimeStamp();
            }
            else
                throw new WebFaultException<string>("Date header not included.", HttpStatusCode.BadRequest);
        }

        private static string ConstructStringToSign(IncomingWebRequestContext context)
        {
            string signature = string.Empty;
            signature += context.Method + "\n";
            signature += context.Headers[HttpRequestHeader.ContentMd5] + "\n";
            signature += context.Headers[HttpRequestHeader.ContentType] + "\n";
            signature += context.Headers["Date"] + "\n";
            signature += GetQueryString(context);
            return signature;
        }

        private static string GetQueryValue(string key, IncomingWebRequestContext woc)
        {
            NameValueCollection query = woc.UriTemplateMatch.QueryParameters;
            if (query.Count != 0)
            {
                return System.Net.WebUtility.HtmlDecode(query[key]);
            }
            return null;
        }

        private static string GetQueryString(IncomingWebRequestContext woc)
        {
            NameValueCollection query = woc.UriTemplateMatch.QueryParameters;
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

        private static void CheckTimeStamp()
        {
            DateTime CallDate = DateTime.Parse(TimeStamp);
            DateTime Current = DateTime.Now;
            if (CallDate < Current.AddMinutes(-15) || CallDate > Current.AddMinutes(15))
                throw new WebFaultException<string>("Invalid date.  Request DENIED!", HttpStatusCode.BadRequest);
        }
    }
}
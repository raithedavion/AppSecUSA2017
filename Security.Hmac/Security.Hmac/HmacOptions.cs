using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Security.Hmac
{
    public class HmacOptions : AuthenticationSchemeOptions
    {
        private string _privateKey { get; set; }
        public delegate string SqlMethod(string publicKey);
        public event SqlMethod GetPrivateKey;

        public string PrivateKey
        {
            set { _privateKey = value;  }
            get
            {
                if (!string.IsNullOrEmpty(_privateKey))
                    return _privateKey;
                if (string.IsNullOrEmpty(PublicKey))
                    throw new Exception("No Public Key");
                return GetPrivateKey(PublicKey);
            }
        }

        /// <summary>
        /// 1, 256, 384, 512
        /// </summary>
        public HmacCipherStrength CipherStrength { get; set; }
        public string PublicKey { get; set; }

        /// <summary>
        /// The object provided by the application to process events raised by the bearer authentication handler.
        /// The application may implement the interface fully, or it may create an instance of JwtBearerEvents
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new HmacEvents Events
        {
            get { return (HmacEvents)base.Events; }
            set { base.Events = value; }
        }
    }
}

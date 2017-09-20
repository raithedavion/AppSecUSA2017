using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Security.Hmac
{
    public class HmacPostConfigureOptions : IPostConfigureOptions<HmacOptions>
    {
        public void PostConfigure(string name, HmacOptions options)
        {
            //do nothing.  because we are lazy.
        }
    }
}

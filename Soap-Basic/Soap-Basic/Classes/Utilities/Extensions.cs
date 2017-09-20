using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Soap_Basic.Classes.Utilities
{
    public static class ExceptionExtensions
    {
        public static string ToXml(this Exception ex)
        {
            ExceptionToXml xml = new ExceptionToXml(ex, false);
            return xml.ToString();
        }
    }
}
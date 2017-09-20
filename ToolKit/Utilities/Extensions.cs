using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.Utilities
{
    public static class Extensions
    {
        public static string GetDescription<T>(this T value)
        {
            var objectType = value.GetType();
            var fieldInfo = objectType.GetField(value.ToString());

            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes.Cast<DescriptionAttribute>().First().Description;
            
            return value.ToString();
        }
    }
}

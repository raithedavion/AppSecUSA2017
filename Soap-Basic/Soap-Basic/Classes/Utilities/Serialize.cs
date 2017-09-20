using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Soap_Basic.Classes.Utilities
{
    public class Serialize
    {
        public static string SerializeToXml<T>(List<T> objects) where T : class
        {
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(sw, objects);
                return sw.ToString();
            }
        }

        public static string SerializeToXml<T>(T objects) where T : class
        {
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sw, objects);
                return sw.ToString();
            }
        }

        protected void serializer_UnknownNode
        (object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        protected void serializer_UnknownAttribute
        (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }

        public static string SerializeChipotleResponse<T>(T response) where T : class
        {
            XmlSerializer writer = new XmlSerializer(typeof(T));
            StringWriter sw = new StringWriter();
            writer.Serialize(sw, response);
            XDocument xDoc = XDocument.Parse(sw.ToString());
            return xDoc.ToString();
        }
    }
}
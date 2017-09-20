using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

namespace ToolKit.Utilities
{
  public class Serializer
  {
    #region serialize
    public static byte[] SerializeObject<T>(T objectToSerialize) where T : class
    {
      MemoryStream ms = new MemoryStream();
      BinaryFormatter bFormatter = new BinaryFormatter();
      bFormatter.Serialize(ms, objectToSerialize);
      byte[] binary = ms.ToArray();
      ms.Close();
      return binary;
    }

    public static string SerializeObjectToString<T>(T objectToSerialize) where T : class
    {
      byte[] bytes = SerializeObject<T>(objectToSerialize);
      return GetString(bytes);
    }

    public static string XmlSerializeObject<T>(T objectToSerialize) where T : class
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      StringWriter writer = new StringWriter();
      xs.Serialize(writer, objectToSerialize);
      return writer.ToString();
    }

    #endregion

    #region deserialize

    public static T DeSerializeObject<T>(byte[] bytes) where T : class
    {
      MemoryStream ms = new MemoryStream(bytes);
      BinaryFormatter bFormatter = new BinaryFormatter();
      T deserialized = (T)bFormatter.Deserialize(ms);
      ms.Close();
      return deserialized;
    }

    public static T DeserializeObject<T>(string data) where T : class
    {
      byte[] bytes = GetBytes(data);
      return DeSerializeObject<T>(bytes);
    }

    public static T XmlDeserializeObject<T>(string xml) where T : class
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      StringReader sr = new StringReader(xml);
      return xs.Deserialize(sr) as T;
    }

    #endregion

    #region utilities

    private static byte[] GetBytes(string str)
    {
      return Convert.FromBase64String(str);
    }

    private static string GetString(byte[] b)
    {
      return Convert.ToBase64String(b);
    }

    #endregion
  }
}

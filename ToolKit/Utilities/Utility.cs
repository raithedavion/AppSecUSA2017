using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolKit.Utilities
{
    public class Utility
    {
        public static DateTime DateOrDefault(DateTime value)
        {
            if (value.Year == 1)
                return new DateTime(1800, 01, 01);
            return value;
        }

        public static DateTime DateOrDefault(DateTime? value)
        {
            if (value.HasValue)
            {
                if (value.Value.Year > 1)
                    return value.Value;
            }
            return new DateTime(1800, 01, 01);
        }

        public static X509Certificate2 GetAPNCertificate()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var sslCertificateSerialNumber = ToolKit.Configuration.Config.GetAppSetting("APNCertSerialNumber");
            // Remove all non allowed characters that entered the value while copy/paste
            var rgx = new Regex("[^a-fA-F0-9]");
            var serial = rgx.Replace(sslCertificateSerialNumber, string.Empty).ToUpper();
            X509Certificate2Collection certs = store.Certificates;//store.Certificates.Find(X509FindType.FindBySerialNumber, serial, true);
            X509Certificate2Collection certs2 = certs.Find(X509FindType.FindBySerialNumber, serial, false);
            store.Close();
            return certs2[0];
        }

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890=!@#$%&*_-+,./?".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GetUniqueKey(int maxSize, string characters)
        {
            char[] chars = new char[characters.Length];
            chars =
            characters.ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            var regex = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
            Match m2 = regex.Match(phone);
            return m2.Success;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ToolKit.Database;

namespace Soap_Basic.Classes.Utilities
{
    public class Hmac
    {
        private static Encoding Encoder { get { return Encoding.UTF8; } set { } }

        public static string Hash(string plainText, string privateKey, int cipherStrength)
        {
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
                //Don't log here.  Logging should be down in calling method.  
                throw ex;
            }
        }

        public static bool Checksum(string hash, string publicKey, params string[] list)
        {
            try
            {
                //Lookup private key here.
                string Key = "blahfornow";
                string Combined = string.Empty;
                foreach (string s in list)
                {
                    Combined += s;
                }
                string LocalHash = Hash(Combined, Key, 256);
                return hash == LocalHash;
            }
            catch (Exception ex)
            {
                Error.InsertException(ex.ToXml(), -1);
                throw ex;
            }
        }

        public static bool ValidateWithQuery(string hash, string publicKey, string query)
        {
            try
            {
                //Lookup private key here.
                string Key = PrivateKeySelect(publicKey);
                string Combined = query;
                string LocalHash = Hash(Combined, Key, 1);
                return hash == LocalHash;
            }
            catch (Exception ex)
            {
                Error.InsertException(ex.ToXml(), -1);
                throw ex;
            }
        }

        private static string PrivateKeySelect(string publicKey)
        {
            try
            {
                SqlParameter[] param = {
                                           new SqlParameter{ ParameterName = "@PrivateKey", Direction = System.Data.ParameterDirection.Output, SqlDbType = System.Data.SqlDbType.VarChar },
                                           DatabaseInteraction.CreateOutputParameter("@ReturnCode")
                                       };
                SqlParameterCollection collection = DatabaseInteraction.ExecuteCommand("uspWebServicePrivateKeySelect", param);
                int ReturnCode = Convert.ToInt32(collection["@ReturnCode"]);
                if (ReturnCode == 0)
                    return collection["@PrivateKey"].ToString();
                return null;
            }
            catch (Exception ex)
            {
                Error.InsertException(ex.ToXml(), -1);
                throw ex;
            }
        }
    }
}
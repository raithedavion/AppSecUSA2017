using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //GlobalProxySelection.Select = new WebProxy("127.0.0.1", 8888);
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            Basic();
            //WS();
            //EncapM();
            Console.Read();
            
        }

        static void Basic()
        {
            try
            {
                BasicService.User user = new BasicService.User();
                user.UserID = 0;
                user.UserName = "User2-Basic";
                user.UserPassword = "e7cf3ef4f17c3999a94f2c6f612e8a888e5b1026878e4e19398b23bd38ec221a";
                user.IsActive = true;
                user.LockedOut = false;

                using (BasicService.UserClient client = new BasicService.UserClient())
                {
                    int id = client.CreateUser(user);
                    Console.WriteLine("UserID: " + id);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void WS()
        {
            WSService.User user = new WSService.User();
            user.UserID = 0;
            user.UserName = "User2-WS";
            user.UserPassword = "e7cf3ef4f17c3999a94f2c6f612e8a888e5b1026878e4e19398b23bd38ec221a";
            user.IsActive = true;
            user.LockedOut = false;

            try
            {
                using (WSService.UserClient client = new WSService.UserClient())
                {
                    client.ClientCredentials.UserName.UserName = "ServiceUser1";
                    client.ClientCredentials.UserName.Password = "MyPassword#1234";
                    int id = client.CreateUser(user);
                    Console.WriteLine("UserID: " + id);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void EncapM()
        {
            Encap.User user = new Encap.User();
            user.UserID = 0;
            user.UserName = "User1-Encap";
            user.UserPassword = "e7cf3ef4f17c3999a94f2c6f612e8a888e5b1026878e4e19398b23bd38ec221a";
            user.IsActive = true;
            user.LockedOut = false;

            try
            {
                using (Encap.UserEncapClient client = new SampleClient.Encap.UserEncapClient())
                {
                    client.ClientCredentials.UserName.UserName = "ServiceUser1";
                    client.ClientCredentials.UserName.Password = "MyPassword#1234";
                    Encap.UserResponse response = client.CreateUser(user);
                    string msg;
                    if (response.WasSuccessful) 
                        msg = string.Format("Was Successful: {0}, ID: {1}", response.WasSuccessful, response.ID);
                    else
                        msg = string.Format("Was Successful: {0}, Error: {1}", response.WasSuccessful, response.Error);
                    Console.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void MakeHash()
        {
            string Pass = "MyPassword#1234";
            string Hashed = CalculateHashedPassword(Pass);
            Console.WriteLine(Hashed);
        }

        private static string CalculateHashedPassword(string clearpwd)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(clearpwd));
                return Convert.ToBase64String(computedHash);
            }
        }
    }
}

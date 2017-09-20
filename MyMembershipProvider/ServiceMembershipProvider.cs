using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using ToolKit.Configuration;
using ToolKit.Database;

namespace MyMembershipProvider
{
    public class ServiceMembershipProvider : MembershipProvider
    {
        #region private properties

        private int pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue("maxInvalidPasswordAttempts", "3"));
        private int pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue("passwordAttemptWindow", "15"));

        #endregion

        #region implemented

        public override bool ValidateUser(string userName, string password)
        {
            //return true;
            try
            {
                string Hash = string.Empty;
                Hash = CalculateHashedPassword(password);
                SqlParameter[] param = {
                                        DatabaseInteraction.CreateOutputParameter("@ReturnCode"),
                                        new SqlParameter("@UserName", userName),
                                        new SqlParameter("@Password", Hash)
                                    };
                SqlParameterCollection results = DatabaseInteraction.ExecuteCommand("uspServiceUserValidate", param);
                int ReturnCode = Convert.ToInt32(results["@ReturnCode"].Value);
                //System.IO.File.WriteAllLines(@"C:\inetpub\wwwroot\ReturnCode.txt", new string[] { ReturnCode.ToString() });
                if (ReturnCode == 0)
                {
                    //System.IO.File.WriteAllLines(@"C:\inetpub\wwwroot\ReturnCode2.txt", new string[] { ReturnCode.ToString() });
                    return true;
                }
                //System.IO.File.WriteAllLines(@"C:\inetpub\wwwroot\ReturnCode3.txt", new string[] { ReturnCode.ToString() });
                return false;
            }
            catch (Exception ex)
            {
                //System.IO.File.WriteAllLines(@"C:\inetpub\wwwroot\Error.txt", new string[] { ex.Message, ex.StackTrace });
                return false;
            }
        }

        private static string CalculateHashedPassword(string clearpwd)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(clearpwd));
                return Convert.ToBase64String(computedHash);
            }
        }



        private static string GetConfigValue(string key, string defaultValue)
        {
            string value = Config.GetAppSetting(key);
            if (value != null)
                return value;
            else
                return defaultValue;
        }

        #endregion

        #region Not Implemented

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string userName, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

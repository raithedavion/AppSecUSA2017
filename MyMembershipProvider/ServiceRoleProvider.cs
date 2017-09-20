using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using ToolKit.Database;

namespace MyMembershipProvider
{
    public class ServiceRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                SqlParameter[] param = {
                                           new SqlParameter("@UserName", username),
                                           new SqlParameter("@RoleName", roleName)
                                       };
                DataSet ds = DatabaseInteraction.ExecuteStoredProc("uspRoleSelectByServiceUserName", param);
                return ds.Tables[0].Rows.Count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                SqlParameter[] param = {
                                           new SqlParameter("@UserName", username)
                                       };
                DataSet ds = DatabaseInteraction.ExecuteStoredProc("uspServiceUserRoleSelect", param);
                IEnumerable<string> list = ds.Tables[0].AsEnumerable().Select(r => r.Field<string>("RoleName"));
                return list.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override string[] GetAllRoles()
        {
            try
            {
                DataSet ds = DatabaseInteraction.ExecuteStoredProc("uspServiceRoleSelect");
                IEnumerable<string> list = ds.Tables[0].AsEnumerable().Select(r => r.Field<string>("RoleName"));
                return list.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override void CreateRole(string roleName)
        {
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return false;
        }

        public override bool RoleExists(string roleName)
        {
            return false;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return null;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return null;
        }

        public override string ApplicationName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

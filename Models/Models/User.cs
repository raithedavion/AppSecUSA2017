using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using ToolKit.Database;

namespace Models
{
    [DataContract]
    public class User
    {
        #region properties

        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string UserPassword { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool LockedOut { get; set; }

        #endregion

        #region constructors

        public User() { }

        #endregion

        #region Conversions

        //I LIKE TO DO THIS AS A STATIC METHOD, BECAUSE IT DOESN'T REQUIRE ME CREATING A NEW OBJECT JUST TO CALL IT
        private static List<User> ConvertToObject(DataTable table)
        {
            IEnumerable<User> list = table.AsEnumerable()
                                          .Select(r => new User
                                          {
                                              UserID = r.Field<int>("UserID"),
                                              UserName = r.Field<string>("UserName"),
                                              IsActive = r.Field<bool>("IsActive"),
                                              LockedOut = r.Field<bool>("LockedOut")
                                          });
            return list.ToList();
        }

        #endregion

        #region CRUD

        //TWO WAYS TO DO THIS.  STATIC OR BY OBJECT CREATION THEN CALLING THE METHOD.  

        //                @UserID int output,
        //@UserName varchar(100),
        //@UserPass varchar(256) = null,
        //@IsActive bit,
        //@LockedOut bit
        public int CreateUser()
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter{ ParameterName = "@UserID", SqlDbType = SqlDbType.Int ,Direction = ParameterDirection.InputOutput, Value = Convert.ToInt32(0) },
                    new SqlParameter("@UserName", UserName),
                    new SqlParameter("@UserPass", UserPassword),
                    new SqlParameter("@IsActive", IsActive),
                    new SqlParameter("@LockedOut", LockedOut)
                };
                return DatabaseInteraction.ExecuteInsertStoredProc("uspUserSave", param);
            }
            catch(Exception ex)
            {
                //You would do error handling here.  
                //throw new Exception("Your friendly neighborhood error message.");
                throw ex;
            }
        }

        public bool UpdateUser()
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter{ ParameterName = "@UserID", SqlDbType = SqlDbType.Int ,Direction = ParameterDirection.InputOutput, Value = UserID },
                    new SqlParameter("@UserName", UserName),
                    new SqlParameter("@UserPass", UserPassword),
                    new SqlParameter("@IsActive", IsActive),
                    new SqlParameter("@LockedOut", LockedOut)
                };
                DatabaseInteraction.ExecuteInsertStoredProc("uspUserSave", param);
                return true;
            }
            catch (Exception ex)
            {
                //You would do error handling here.  
                //throw new Exception("Your friendly neighborhood error message.");
                throw ex;
            }
        }

        public static User GetUser(int id)
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@UserID", id)
                };
                DataSet ds = DatabaseInteraction.ExecuteStoredProc("uspUserSelect", param);
                return ConvertToObject(ds.Tables[0]).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //You would do error handling here.  
                //throw new Exception("Your friendly neighborhood error message.");
                throw ex;
            }
        }

        public static List<User> ListUsers()
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@UserID", Convert.ToInt32(0))
                };
                DataSet ds = DatabaseInteraction.ExecuteStoredProc("uspUserSelect", param);
                return ConvertToObject(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                //You would do error handling here.  
                //throw new Exception("Your friendly neighborhood error message.");
                throw ex;
            }
        }

        #endregion
    }
}
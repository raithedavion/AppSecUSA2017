using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ToolKit.Database;

namespace Soap_Basic.Classes.Utilities
{
    public class Error
    {
        public int ExceptionID { get; set; }
        public string ErrorMsg { get; set; }
        public int CreateByID { get; set; }

        public Error() { }

        public static int InsertException(string exception, int createByID)
        {
            try
            {
                SqlParameter[] param = {
                                           new SqlParameter("@Exception", exception),
                                           new SqlParameter("@CreateByID", createByID)
                                       };
                //return DatabaseInteraction.ExecuteInsertStoredProc("", param);
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
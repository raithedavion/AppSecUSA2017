using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class User
    {
        #region properties

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool IsActive { get; set; }
        public bool LockedOut { get; set; }

        public IConfiguration _ConnectionString;

        #endregion

        public int CreateUser()
        {
            using (SqlConnection con = new SqlConnection(_ConnectionString.GetSection("Data").GetSection("ConnectionString").Value))
            {
                //int id = con.Q
            }
        }
    }
}

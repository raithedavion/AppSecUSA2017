using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.Database;

namespace ToolKit.Utilities
{
    public class Log
    {
        public int LogID { get; set; }
        public string EventType { get; set; }
        public string Event { get; set; }

        public Log() { }

        public void Save()
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@EventType", EventType),
                    new SqlParameter("@Event", Event)
                };
                DatabaseInteraction.ExecuteStoredProc("uspLogInsert", param);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to save log");
            }
        }
    }
}

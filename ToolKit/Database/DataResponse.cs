using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.Database
{
  public class DataResponse
  {
    public SqlParameterCollection Parameters { get; set; }
    public DataSet DataResult { get; set; }
  }
}

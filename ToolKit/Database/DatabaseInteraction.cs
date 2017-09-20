using System;
using System.Data.SqlClient;
using System.Data;
using ToolKit.Configuration;

namespace ToolKit.Database
{
  /// <summary>
  /// Summary description for DatabaseInteraction
  /// </summary>
  public class DatabaseInteraction
  {
    public static string WarningMg { get; set; }

    #region ExecuteBulkCopy

    /// <summary>
    /// Used to Insert an entire datatable.
    /// </summary>
    /// <param name="destinationTableName">destination table name</param>
    /// <param name="table">Data table to insert</param>
    /// <returns>Empty Dataset</returns>
    public static DataSet ExecuteBulkCopy(string destinationTableName, DataTable table)
    {
      return ExecuteBulkCopy(Config.ConnectionString, destinationTableName, table);
    }

    /// <summary>
    /// Used to Insert an entire datatable.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the database</param>
    /// <param name="destinationTableName">destination table name</param>
    /// <param name="table">Data table to insert</param>
    /// <returns>Empty Dataset</returns>
    public static DataSet ExecuteBulkCopy(string connectionString, string destinationTableName, DataTable table)
    {
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        try
        {
          connection.Open();

          using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
          {
            foreach (DataColumn c in table.Columns)
            {
              bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
            }

            bulkCopy.DestinationTableName = destinationTableName;
            bulkCopy.WriteToServer(table);
          }
        }
        catch (SqlException ex)
        {
          throw new Exception(ex.Message);
        }
      }
      return new DataSet();
    }

    #endregion

    #region ExecuteStoredProc

    /// <summary>
    ///Use to return a dataset
    /// </summary>      
    /// <param name="spName">The name of the stored procedure to execute.</param>
    /// <param name="sqlParams">Optional parameters for the stored procedure.</param>
    /// <returns>A DataSet containing the result set of the stored procedure.</returns>
    public static DataSet ExecuteStoredProc(string spName, params SqlParameter[] sqlParams)
    {
      DataSet ds = ExecuteStoredProc(Config.ConnectionString, spName, sqlParams);
      return ds;
    }

    /// <summary>
    ///Use to return a dataset
    /// </summary>
    /// <param name="connectionString">The connection string to connect to the database.</param>
    /// <param name="spName">The name of the stored procedure to execute.</param>
    /// <param name="sqlParams">Optional parameters for the stored procedure.</param>
    /// <returns>A DataSet containing the result set of the stored procedure.</returns>
    public static DataSet ExecuteStoredProc(string connectionstring, string spName, params SqlParameter[] sqlParams)
    {
      DataSet dataSet = new DataSet();
      
      using (SqlConnection connection = new SqlConnection(connectionstring))
      {
        try
        {
          connection.Open();
          connection.InfoMessage += delegate(object sender, SqlInfoMessageEventArgs e)
          {
            WarningMg = "\n" + e.Message;
          };
          SqlCommand cmd = connection.CreateCommand();
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = spName;
          foreach (SqlParameter param in sqlParams)
          {
            if (param.Value == null)
              param.Value = DBNull.Value;
            else
            {
              if (param.SqlDbType == SqlDbType.VarChar || param.SqlDbType == SqlDbType.NVarChar)
              {
                param.Value = ((string)param.Value).Trim();
              }
            }
            cmd.Parameters.Add(param);
          }
          cmd.CommandTimeout = Config.DatabaseTimeout;
          SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

          dataAdapter.Fill(dataSet);
        }
        catch (SqlException ex)
        {
          throw new Exception(ex.Message);
        }
      }

      return dataSet;
    }

    #endregion

    #region ExecuteInsertStoredProc
    /// <summary>
    /// Use this method to insert, update or delete. Method uses default connection string
    /// </summary>       
    /// <param name="spName">Name of the Stored procedures</param>
    /// <param name="sqlParams">Optional paramaters.  Add an output parameter to the sqlParams array for a return value</param>
    /// <returns>returns an integer</returns>     
    public static int ExecuteInsertStoredProc(string spName, params SqlParameter[] sqlParams)
    {
      int i = ExecuteInsertStoredProc(Config.ConnectionString, spName, sqlParams);
      return i;
    }

    /// <summary>
    /// Use this method to insert, update or delete.  
    /// </summary>
    /// <param name="connectionstring">Use to switch datasource</param>
    /// <param name="spName">Name of the Stored procedures</param>
    /// <param name="sqlParams">Optional paramaters.  Add an output parameter to the sqlParams array for a return value</param>
    /// <returns>returns an integer</returns>     
    public static int ExecuteInsertStoredProc(string connectionstring, string spName, params SqlParameter[] sqlParams)
    {
      int rowsAffected = 0;
      using (SqlConnection connection = new SqlConnection(connectionstring))
      {
        SqlCommand cmd = null;
        try
        {
          connection.Open();
          cmd = connection.CreateCommand();
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = spName;
          cmd.CommandTimeout = Config.DatabaseTimeout;


          foreach (SqlParameter param in sqlParams)
          {
            if (param.Value == null)
              param.Value = DBNull.Value;
            else
            {
              if (param.SqlDbType == SqlDbType.VarChar || param.SqlDbType == SqlDbType.NVarChar)
              {
                param.Value = ((string)param.Value).Trim();
              }
            }
            cmd.Parameters.Add(param);
          }

          rowsAffected = cmd.ExecuteNonQuery();
          foreach (SqlParameter param in sqlParams)
            if ((param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput) && (param.Value != null) && (param.Value != DBNull.Value))
            {
              int result = Convert.ToInt32(param.Value);
              return result;
            }
        }
        catch (SqlException ex)
        {
          throw new Exception(ex.Message);
        }
      }
      return rowsAffected;
    }

    #endregion

    #region ExecuteScalarStoredProc

    public static object ExecuteScalarStoredProc(string spName, params SqlParameter[] sqlParams)
    {
      object result = ExecuteScalarStoredProc(Config.ConnectionString, spName, sqlParams);
      return result;
    }


    /// <summary>
    /// Executes the stored procedure and returns the first column of the first row.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="spName"></param>
    /// <param name="sqlParams"></param>
    /// <returns></returns>
    public static object ExecuteScalarStoredProc(string connectionstring, string spName, params SqlParameter[] sqlParams)
    {
      object result = null;
      using (SqlConnection connection = new SqlConnection(connectionstring))
      {
        try
        {
          connection.Open();
          SqlCommand cmd = connection.CreateCommand();
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = spName;
          cmd.CommandTimeout = Config.DatabaseTimeout;

          foreach (SqlParameter param in sqlParams)
          {
            if (param.Value == null)
              param.Value = DBNull.Value;
            else
            {
              if (param.SqlDbType == SqlDbType.VarChar || param.SqlDbType == SqlDbType.NVarChar)
              {
                param.Value = ((string)param.Value).Trim();
              }
            }
            cmd.Parameters.Add(param);
          }

          result = cmd.ExecuteScalar();

        }
        catch (SqlException ex)
        {
          throw new Exception(ex.Message);
        }
      }

      return result;
    }

    #endregion // ExecuteScalar

    #region ExecuteInsertTextCommand

    /// <summary>
    /// Protected method to execute an insert commandText command.
    /// </summary>
    /// <param name="connectionString">The connection string to connect to the database.</param>
    /// <param name="commandText">The commandText of the command.</param>
    /// <returns>An int representing the number of rows affected by the command.</returns>
    public static int ExecuteInsertTextCommand(string commandText)
    {
      using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
      {
        try
        {
          connection.Open();
          SqlCommand cmd = connection.CreateCommand();
          cmd.Connection = connection;
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = commandText;
          cmd.CommandTimeout = Config.DatabaseTimeout;
          int result = cmd.ExecuteNonQuery();
          return result;
        }
        catch (SqlException ex)
        {
          throw new Exception(ex.Message);
        }
      }
    }

    #endregion

    #region ExecuteTextCommand

    /// <summary>
    /// Executes a commandText command.
    /// </summary>
    public static DataSet ExecuteTextCommand(string commandText)
    {
      using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
      {
        try
        {
          connection.Open();
          SqlCommand cmd = new SqlCommand(commandText, connection);
          cmd.CommandTimeout = Config.DatabaseTimeout;

          SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
          DataSet dataSet = new DataSet();

          dataAdapter.Fill(dataSet);

          return dataSet;
        }
        catch (SqlException ex)
        {
          throw new Exception(ex.Message);
        }
      }
    }

    #endregion

    #region ExecuteCommand

    /// <summary>
    ///Use to a set of output parameters
    /// </summary>      
    /// <param name="spName">The name of the stored procedure to execute.</param>
    /// <param name="sqlParams">Optional parameters for the stored procedure.</param>
    /// <returns>A SqlParameterCollection.</returns>
    public static SqlParameterCollection ExecuteCommand(string spName, params SqlParameter[] sqlParams)
    {
      SqlParameterCollection ds = ExecuteCommand(Config.ConnectionString, spName, sqlParams);
      return ds;
    }

    /// <summary>
    ///Use to a set of output parameters
    /// </summary>
    /// <param name="connectionString">The connection string to connect to the database.</param>
    /// <param name="spName">The name of the stored procedure to execute.</param>
    /// <param name="sqlParams">Optional parameters for the stored procedure.</param>
    /// <returns>A SqlParameterCollection.</returns>
    public static SqlParameterCollection ExecuteCommand(string connectionstring, string spName, params SqlParameter[] sqlParams)
    {
      DataSet dataSet = new DataSet();
      SqlParameterCollection collection;

      using (SqlConnection connection = new SqlConnection(connectionstring))
      {
        try
        {
          connection.Open();
          SqlCommand cmd = connection.CreateCommand();
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = spName;
          foreach (SqlParameter param in sqlParams)
          {
            if (param.Value == null)
              param.Value = DBNull.Value;
            else
            {
              if (param.SqlDbType == SqlDbType.VarChar || param.SqlDbType == SqlDbType.NVarChar)
              {
                param.Value = ((string)param.Value).Trim();
              }
            }
            cmd.Parameters.Add(param);
          }
          cmd.CommandTimeout = Config.DatabaseTimeout;
          SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

          dataAdapter.Fill(dataSet);
          collection = cmd.Parameters;
        }
        catch (SqlException ex)
        {
          throw ex;
        }
      }

      return collection;
    }

    #endregion

    #region CreateOutputParameter

    /// <summary>
    /// Creates an output parameter of the specified name and int32 datatype.
    /// </summary>
    /// <param name="paramName"></param>
    /// <returns></returns>
    public static SqlParameter CreateOutputParameter(string paramName)
    {
      SqlParameter sqlOutput = new SqlParameter(paramName, 0);
      sqlOutput.Direction = ParameterDirection.Output;
      sqlOutput.SqlDbType = SqlDbType.Int;
      return sqlOutput;
    }

    /// <summary>
    /// Creates an output parameter of the specified name and int32 datatype.
    /// </summary>
    /// <param name="paramName"></param>
    /// <returns></returns>
    public static SqlParameter CreateOutputParameter(string paramName, SqlDbType type)
    {
      SqlParameter sqlOutput = new SqlParameter(paramName, 0);
      sqlOutput.Direction = ParameterDirection.Output;
      sqlOutput.SqlDbType = type;
      return sqlOutput;
    }

    /// <summary>
    /// Creates an output parameter of the specified name and int32 datatype.
    /// </summary>
    /// <param name="paramName"></param>
    /// <returns></returns>
    public static SqlParameter CreateOutputParameter(string paramName, SqlDbType type, int size)
    {
      SqlParameter sqlOutput = new SqlParameter(paramName, 0);
      sqlOutput.Direction = ParameterDirection.Output;
      sqlOutput.SqlDbType = type;
      sqlOutput.Size = size;
      return sqlOutput;
    }

    public static SqlParameter CreateInputOutputParameter(string paramName, SqlDbType type, int size, object value)
    {
      SqlParameter sqlOutput = new SqlParameter(paramName, 0);
      sqlOutput.Direction = ParameterDirection.InputOutput;
      sqlOutput.SqlDbType = type;
      sqlOutput.Size = size;
      sqlOutput.Value = value;
      return sqlOutput;
    }

    #endregion
  }
}

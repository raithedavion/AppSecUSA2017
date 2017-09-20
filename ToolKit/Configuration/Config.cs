using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.ComponentModel;

namespace ToolKit.Configuration
{
  /// <summary>
  /// Summary description for Config
  /// </summary>
  public class Config
  {
    #region properties

    private static string databaseTimeout;
    private static string connectionString;
    private static string codeStage;
    private static string connectionName;

    #endregion

    #region to int

    public static int ToInt(string input, int defaultValue)
    {
      if (string.IsNullOrEmpty(input))
        return defaultValue;
      int result;
      if (!Int32.TryParse(input, out result))
        result = defaultValue;
      return result;
    }

    #endregion

    #region application settings

    public static string GetAppSetting(string key)
    {
      ConfigurationManager.RefreshSection("appSettings");
      string result = string.Empty;
      try
      {
        result = ConfigurationManager.AppSettings[key];
      }
      catch (Exception)
      {
        //do nothing.  just return the emtpy string
      }
      return result;
    }

    public static int CacheTimeoutInHours
    {
      get
      {
        return Convert.ToInt32(GetAppSetting("CacheTimeoutInHours"));
      }
    }

    #endregion

    #region Code Stage/Environment

    [Description("Determines specificed flags in the system, as well as which database the app will point to.  Set to Local, Stage, or Prod.")]
    public static string CodeStage
    {
      get
      {
        if (codeStage == null)
          codeStage = ConfigurationManager.AppSettings["CodeStage"];
        return codeStage;
      }
    }

    #endregion

    #region connection string and timeouts

    [Description("Sets the Connection Name Based on the specified CodeStage")]
    public static string ConnectionName
    {
      get
      {
        connectionName = CodeStage;
        return connectionName;
        ////return "HUB2ConnectionString";
        //if (CodeStage.Equals("Prod"))
        //  connectionName = "Prod";
        //else
        //else
        //  connectionName = "Stage";
        //return connectionName;
      }
    }

    [Description("Indicated the command timeout for database access")]
    [DefaultValue(30)]
    public static int DatabaseTimeout
    {
      get
      {
        if (databaseTimeout == null)
          databaseTimeout = GetAppSetting("DatabaseTimeout");
        return ToInt(databaseTimeout, 30);
      }
    }

    [Description("Connection string")]
    public static string ConnectionString
    {
      get
      {
        if (string.IsNullOrEmpty(connectionString))
          connectionString = ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
        return connectionString;
      }
    }

    #endregion

    /// <summary>
    /// CAN BE REMOVED
    /// </summary>
    #region SMTP

    public static string SmtpHost
    {
      get { return ConfigurationManager.AppSettings["SmtpHost"]; }
    }

    #endregion

    #region Authentication Settings

    public static int AuthTimeout
    {
      get { return Convert.ToInt32(GetAppSetting("AuthExpiration")); }
    }

    public static string AuthCookieName
    {
      get { return GetAppSetting("AuthCookieName"); }
    }

    #endregion
  }
}

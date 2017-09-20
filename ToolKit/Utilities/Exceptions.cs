using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using ToolKit.Database;
using System.Data.SqlClient;

namespace ToolKit.Utilities
{
  public class Exceptions : DatabaseInteraction
  {
    public static void Add(Exception ex, bool includeStackTrace, int createdByID = 0)
    {
      if (ex != null)
      {
        try
        {
          SqlParameter[] param = {
                                   new SqlParameter("@Exception", GetExceptionElement(ex, includeStackTrace).ToString()),
                                   new SqlParameter("@CreateByID", createdByID)
                                 };
          DatabaseInteraction.ExecuteStoredProc("uspUtilityExceptionInsert", param);
        }
        catch
        {
          //do nothing!
        }
      }
    }

    private static XElement GetExceptionElement(Exception ex, bool includeStackTrace)
    {
      XElement root = new XElement(ex.GetType().ToString());
      if (ex.Message != null)
      {
        root.Add(GetMessage(ex));
      }

      if (includeStackTrace && ex.StackTrace != null)
      {
        root.Add(GetStackTrace(ex));
      }

      if (ex.Data.Count > 0)
      {
        root.Add(GetData(ex));
      }

      if (ex.InnerException != null)
      {
        root.Add(GetExceptionElement(ex.InnerException, includeStackTrace));
      }
      return root;
    }

    private static XElement GetMessage(Exception ex)
    {
      return new XElement("Message", ex.Message);
    }

    private static XElement GetStackTrace(Exception ex)
    {
      return new XElement
      (
        "StackTrace",
        ex.StackTrace.Split('\n')
                     .Select(xe => xe.Substring(6).Trim())
      );
    }

    private static XElement GetData(Exception ex)
    {
      return new XElement
      (
        "Data",
        ex.Data.Cast<DictionaryEntry>()
               .Select(xe => new XElement(xe.Key.ToString(), (xe.Value == null) ? "null" : xe.Value.ToString()))
      );
    }
  }
}

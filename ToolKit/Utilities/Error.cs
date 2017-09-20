using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.Utilities
{
  public class Error
  {
    public Exception Exception { get; set; }
    public string ErrorMessage { get; set; }

    public Error() { }

    public static string ReturnFriendlyErrorMessage(Exception ex)
    {
      if (ex.Message.Contains('~'))
      {
        string[] messageParts = ex.Message.Split('~');
        string msg = messageParts[1];
        return msg;
      }
      else
        return ex.Message;
    }

    public static string ReturnFriendlyErrorMessage(string ex)
    {
      string msg = string.Empty;
      if (ex.Contains('~'))
      {
        string[] messageParts = ex.Split('~');
        msg = messageParts[1];
      }
      if (msg.Contains('|'))
      {
        msg = msg.Replace("|", "<br />");
      }
      return msg;
    } 
  }
}

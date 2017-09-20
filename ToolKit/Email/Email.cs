using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ToolKit.Email
{
  public class Email
  {
    #region properties

    private MailMessage message;
    private string userName;
    private string password;

    public string From
    {
      get { return message.From.Address; }
      set { message.From = new MailAddress(value); }
    }

    public string To
    {
      get { return message.To[0].Address; }
      set
      {
        if (message.To.Count < 1)
          message.To.Add(value);
        else
        {
          message.To.Clear();
          message.To.Add(value);
        }
      }
    }

    public string Subject
    {
      get { return message.Subject; }
      set { message.Subject = value; }
    }

    public string Body
    {
      get { return message.Body; }
      set { message.Body = value; }
    }

    /// <summary>
    /// Determines whether or not the specified email has HTML code.  Default is False.
    /// </summary>
    public bool IsHTML
    {
      get { return message.IsBodyHtml; }
      set { message.IsBodyHtml = value; }
    }

    public string UserName
    {
      get { return userName; }
      set { userName = value; }
    }

    public string Password
    {
      get { return password; }
      set { password = value; }
    }

    #endregion

    #region constructors

    public Email() { message = new MailMessage(); IsHTML = false; }

    public Email(bool isHTML) { message = new MailMessage(); IsHTML = isHTML; }

    public Email(bool isHTML, string body, string subject) 
    {
      message = new MailMessage();
      IsHTML = isHTML;
      Body = body;
      Subject = subject;
    }

    public Email(string body, string subject)
    {
      message = new MailMessage();
      IsHTML = false;
      Body = body;
      Subject = subject;
    }

    #endregion

    #region methods

    /// <summary>
    /// Uses the Default paramters Set forth by the config file.
    /// </summary>
    public void Send()
    {
      using (SmtpClient client = new SmtpClient())
      {
        string bcc = Configuration.Config.GetAppSetting("BCC");
        if (!String.IsNullOrEmpty(bcc) && message.Subject != "LearingRx Hub Password Reset")
          message.Bcc.Add(bcc);
        client.Send(message);
      }
    }

    public void Send(int port)
    {
      using (SmtpClient client = new SmtpClient())
      {
        string bcc = Configuration.Config.GetAppSetting("BCC");
        if (!String.IsNullOrEmpty(bcc) && message.Subject != "LearingRx Hub Password Reset")
          message.Bcc.Add(bcc);
        client.Port = port;
        client.Send(message);
      }
    }

    public void Send(string from, string to)
    {
      string bcc = Configuration.Config.GetAppSetting("BCC");
      if (!String.IsNullOrEmpty(bcc) && message.Subject != "LearingRx Hub Password Reset")
        message.Bcc.Add(bcc);
      From = from;
      To = to;
      using (SmtpClient client = new SmtpClient())
      {
        client.Send(message);
      }
    }

    public void Send(int port, string from, string to)
    {
      string bcc = Configuration.Config.GetAppSetting("BCC");
      if (!String.IsNullOrEmpty(bcc) && message.Subject != "LearingRx Hub Password Reset")
        message.Bcc.Add(bcc);
      From = from;
      To = to;
      using (SmtpClient client = new SmtpClient())
      {
        client.Port = port;
        client.Send(message);
      }
    }

    #endregion
  }
}

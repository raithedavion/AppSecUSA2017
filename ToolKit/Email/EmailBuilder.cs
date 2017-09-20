using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolKit.Email
{
  public abstract class EmailBuilder
  {
    protected Email email { get; set; }

    public EmailBuilder() { email = new Email(); }

    public EmailBuilder(bool isHtml) { email = new Email(isHtml); }

    public EmailBuilder(string body, string subject) { email = new Email(body, subject); }

    public EmailBuilder(bool isHtml, string body, string subject) { email = new Email(isHtml, body, subject); }

    public Email GetEmail() { return email; }

    public abstract void SetEmailParameters(string subject, string body);
    public abstract void SetToAddress(string address);
  }
}

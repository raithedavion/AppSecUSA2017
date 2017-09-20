using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Soap_Basic
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            System.Web.ApplicationServices.AuthenticationService.Authenticating +=
            new EventHandler<System.Web.ApplicationServices.AuthenticatingEventArgs>(AuthenticationService_Authenticating);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Exception ex = Server.GetLastError();
            //System.IO.File.WriteAllLines(@"C:\inetpub\wwwroot\global.txt", new string[] { ex.Message });
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void AuthenticationService_Authenticating(object sender, System.Web.ApplicationServices.AuthenticatingEventArgs e)
        {
            e.Authenticated = Membership.Providers["AppSecMembershipProvider"].ValidateUser(e.UserName, e.Password);
            e.AuthenticationIsComplete = true;
        }
    }
}
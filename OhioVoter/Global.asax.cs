using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OhioVoter
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        
        protected void Application_Error(object sender, EventArgs e)
        {
            
            // Get the error details
            HttpException lastErrorWrapper = Server.GetLastError() as HttpException;
            Exception lastError = lastErrorWrapper;

            string lastErrorTypeName = "";
            string lastErrorMessage = "";
            string lastErrorStackTrace = "";

            if (lastErrorWrapper != null && lastErrorWrapper.InnerException != null)
            {
                lastError = lastErrorWrapper.InnerException;
                lastErrorTypeName = lastError.GetType().ToString();
                lastErrorMessage = lastError.Message;
                lastErrorStackTrace = lastError.StackTrace;
            }
            else if (Context != null)
            {
                lastErrorTypeName = Context.Error.GetType().ToString();
                lastErrorMessage = Context.Error.ToString();
                lastErrorStackTrace = Context.Error.StackTrace;
            }

            // send email with details
            string toAddress = "ohiovoter.org@yahoo.com";
            string subject = "An Error Has Occurred!";
            string body = string.Format("URL: {0}\r\n\r\nUser: {1}\r\n\r\nException Type: {2}\r\n\r\nMessage: {3}\r\n\r\nStack Trace: {4}", 
                Request.RawUrl, 
                User.Identity.Name,
                lastErrorTypeName,
                lastErrorMessage,
                lastErrorStackTrace.Replace(Environment.NewLine, "\r\n"));
            Services.Email email = new Services.Email();
            bool emailSent = email.SendEmail(toAddress, subject, body);

            // clear error
            //Context.Server.ClearError();
            
        }
        
    }
}
